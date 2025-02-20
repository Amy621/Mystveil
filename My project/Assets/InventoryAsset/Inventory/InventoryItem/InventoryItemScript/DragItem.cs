using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    
    /// <summary>
    /// Controls the dragging of an item.
    /// </summary>
    
    internal class DragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// The current slot associated with the drag item
        private Slot CurrentSlot;

        /// The inventory item being dragged
        private InventoryItem item;

        /// The text UI element for displaying item information
        [SerializeField, HideInInspector]
        private TextMeshProUGUI text;

        //determines how item acts on miss;
        private bool returnOnMiss = false;
        private bool dropped = true;
        /// The text UI element for displaying item information
        private GameObject prevslot;
        /// Initializes the CurrentSlot on start
        private void Start()
        {
            prevslot = null;

            CurrentSlot = transform.parent.GetComponent<Slot>();
        }
        public void Initiailize()
        {
            if (transform.GetChild(0) != null)
            {
                if (transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>() != null)
                {
                    text = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                }
                else
                {
                    Debug.LogError("TextMeshPro is not null on slot child object");
                }

            }
            else
            {
                Debug.LogError("Slot child null");
            }
        }
        /// <summary>
        /// Handles the drag event
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (Draggable()) return;

            // Get the canvas and its RectTransform
            Canvas canvas = InventoryController.instance.GetUI().GetComponent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // Convert the screen point to a point relative to the canvas
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                // Update the item's position relative to the canvas
                transform.localPosition = localPointerPosition;
            }

            transform.parent.gameObject.transform.SetAsLastSibling(); // Ensure the dragged item is always on top

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            bool foundSlot = false;


            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Slot"))
                {
                    Slot slot = result.gameObject.GetComponent<Slot>();
                    if (slot.GetItem().GetIsNull() && slot.GetInventoryUI().GetInventory().CheckAcceptance(item.GetItemType()))
                    {
                        slot.GetInventoryUI().Highlight(result.gameObject);

                        foundSlot = true;
                    }
                    if (prevslot != null && prevslot != result.gameObject)
                    {
                        prevslot.GetComponent<Slot>().GetInventoryUI().UnHighlight(prevslot);
                        prevslot.GetComponent<Slot>().GetInventoryUI().ResetHighlight();
                    }
                    prevslot = result.gameObject;

                    break;
                }
            }

            if (!foundSlot)
            {
                if (prevslot != null)
                {
                    prevslot.GetComponent<Slot>().GetInventoryUI().UnHighlight(prevslot);
                    prevslot.GetComponent<Slot>().GetInventoryUI().ResetHighlight();


                }

            }
        }

        /// <summary>
        /// Handles the beginning of the drag event
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Draggable()) return;
            if (CurrentSlot != null && dropped)
            {
                dropped = false;
                CurrentSlot.ResetSlot();
                transform.SetParent(CurrentSlot.GetInventoryUI().GetUI());
            }
            else
            {
                Debug.LogWarning("No Slot");
            }
        }

        /// <summary>
        /// Handles the end of the drag event
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (Draggable()) return;

            HandleEndDrag(eventData);
        }

        /// <summary>
        /// Processes the end of the drag event and checks for valid drop targets.
        /// </summary>
        private void HandleEndDrag(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            bool foundSlot = false;

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Slot"))
                {
                    HandleSlot(result);
                    foundSlot = true;
                    break;
                }
            }

            if (!foundSlot)
            {
                HandleInvalidPlacement();
            }
        }

        /// <summary>
        /// Processes the slot result after dragging
        /// </summary>
        private void HandleSlot(RaycastResult result)
        {
            Slot slot = result.gameObject.GetComponent<Slot>();
            bool slotNull = slot.GetItem().GetIsNull();
            bool itemStackable = !slot.GetItem().GetIsNull() && (slot.GetItem().GetItemType() == item.GetItemType()) && (slot.GetItem().GetAmount() + item.GetAmount()) <= slot.GetItem().GetItemStackAmount();
            bool itemAcceptedInInventory = slot.GetInventoryUI().GetInventory().CheckAcceptance(item.GetItemType());

            if ((slotNull || itemStackable) && itemAcceptedInInventory)
            {
                InventoryController.instance.AddItemPos(slot.GetInventoryUI().GetInventoryName(), item, slot.GetPosition());
                slot.GetInventoryUI().UnHighlight(result.gameObject);
                prevslot.GetComponent<Slot>().GetInventoryUI().ResetHighlight();
                Destroy(gameObject);
            }
            else
            {
                if (itemAcceptedInInventory)
                {
                    HandleInvalidPlacementOverInv(slot.GetItem());

                }
                else
                {
                    HandleInvalidPlacement(true);
                }
            }
        }

        /// <summary>
        /// Handles invalid placements based on user input. Can destroy the item, return item to original position, and invoke a user given function with <see cref="InventoryUIManager.InvokeMiss(Vector3, InventoryItem)"/>  
        /// </summary>
        private void HandleInvalidPlacement(bool isOverride = false)
        {
            Vector3 mousePosition = Input.mousePosition;
            Camera cam = Camera.main;
            Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
            if (returnOnMiss || isOverride)
            {
                InventoryController.instance.AddItemPos(CurrentSlot.GetInventoryUI().GetInventoryName(), item, CurrentSlot.GetPosition());
                CurrentSlot.GetInventoryUI().UnHighlight(CurrentSlot.gameObject);
                if (!isOverride)
                {
                    CurrentSlot.GetInventoryUI().InvokeMiss(worldPosition, item);
                }
                dropped = true;
                Destroy(gameObject);
            }
            else
            {
                CurrentSlot.GetInventoryUI().InvokeMiss(worldPosition, item);
                dropped = true;
                Destroy(gameObject);
            }

        }
        private void HandleInvalidPlacementOverInv(InventoryItem inSlot)
        {
            if (CurrentSlot.GetInventoryUI().InvokeMissOverSlot(item, inSlot))
            {
                Destroy(gameObject);
            }
            else
            {
                HandleInvalidPlacement(true);
            }
        }

        /// <summary>
        /// Checks if the item is draggable
        /// </summary>
        private bool Draggable()
        {
            return CurrentSlot != null && (!item.GetDraggable() || !CurrentSlot.GetInventoryUI().GetDraggable());
        }

        /// <summary>
        /// Sets the inventory item for the drag item
        /// </summary>
        public void SetItem(InventoryItem newItem)
        {
            item = newItem;
        }

        /// <summary>
        /// Updates the text UI based on the item's properties
        /// </summary>
        public void SetText()
        {
            if (!item.GetIsNull())
            {
                text.gameObject.SetActive(item.GetDisplayAmount());
                if (item.GetDisplayAmount())
                {
                    text.SetText(item.GetAmount().ToString());
                }
            }
        }
        public void SetTextTestImage(int amount)
        {
            text.SetText(amount.ToString());
        }

        /// <summary>
        /// Sets the offset for the text position
        /// </summary>
        public void SetTextPositionOffset(Vector3 offset)
        {
            text.gameObject.transform.position += offset;
        }

        /// <summary>
        /// Sets the font size for the text UI
        /// </summary>
        public void SetTextSize(float size)
        {
            text.fontSize = size;
        }

        public void SetImageSize(Vector2 size)
        {
            RectTransform imageRect = GetComponent<RectTransform>();
            imageRect.sizeDelta = size;
        }

        public void SetImage(Sprite image)
        {
            GetComponent<Image>().sprite = image;
        }
        public float GetTextSize()
        {
            return text.fontSize;
        }

        public Vector2 GetTextPosition()
        {
            return text.transform.localPosition;
        }

        public void SetTextPosition(Vector2 textposition)
        {
            text.transform.localPosition = textposition;
        }
        public void SetImagePositionOffset(Vector3 imagePostionOffset)
        {
            transform.position += imagePostionOffset;
        }
        public void SetReturnOnMiss(bool returnOnMiss)
        {
            this.returnOnMiss = returnOnMiss;
        }
    }
}
