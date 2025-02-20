using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    
    /// <summary>
    /// This class creates a slot gameObject that displays an image of the item when notified by the assigned inventory
    /// </summary>
    
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private int position;//The position if the inventories items list
        [SerializeField]
        private GameObject slotChildPrefab;//This holds the prefab for the image allowing it to be instantiated when the child object is dragged to a new location
        [SerializeField]
        private GameObject SlotItemHolder;//This is a child object that is used to display an image of the object

        private InventoryItem item;//This is the current item in the inventory, there is always an item however item.GetIsNull() determines if the object contains a real item
        private UnityEngine.Color color;//This is the color of the slot
        private Image slotImage;//This is the image of the slot
        private InventoryUIManager inventoryUIManager;
        private Vector3 initialChildScale;//holds the scale for the slot child to allow for it to be instantiated with the correct size
        private Vector3 initialSlotChildPosition;//This holds the position of the slot child so it can be instantiated with the correct location
        private float textSize;
        private Vector2 SlotItemHolderSize;
        private bool returnOnMiss = false;//checks whether or not item should return to inventory when the user misses



        /// <summary>
        /// Sets essential variables for the inventory slot
        /// </summary>
        private void Awake()
        {
            slotImage = GetComponent<Image>();
            color = slotImage.color;

            inventoryUIManager = transform.parent.GetComponent<InventoryUIManager>();

            initialChildScale = SlotItemHolder.transform.localScale;


        }
        /// <summary>
        /// Initializes slot child, calling <see cref="UpdateSlot"/>
        /// </summary>
        private void Start()
        {
            SlotItemHolder.SetActive(true);

            item = inventoryUIManager.GetInventoryItem(position);
            initialSlotChildPosition = SlotItemHolder.transform.position;

            UpdateSlot();

        }
        /// <summary>
        /// Updates the slot to display the item in the slots associated position
        /// </summary>
        public void UpdateSlot()
        {
            item = inventoryUIManager.GetInventoryItem(position);
            if (item != null)
            {
                if (!item.GetIsNull())
                {
                    DragItem dragItem = SlotItemHolder.GetComponent<DragItem>();
                    dragItem.SetItem(item);
                    dragItem.SetText();
                    SlotItemHolder.GetComponent<Image>().sprite = item.GetItemImage();
                    SlotItemHolder.SetActive(true);
                }
                else
                {

                    SlotItemHolder.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Item is null");
            }

        }
        /// <summary>
        /// Adds a new slotchild when slot child is dragged away, and resets the slot to empty
        /// </summary>
        public void ResetSlot()
        {
            GameObject newInstance = Instantiate(slotChildPrefab, initialSlotChildPosition, Quaternion.identity);
            newInstance.transform.SetParent(transform);
            newInstance.transform.localScale = initialChildScale;
            Vector2 prevTextPos = SlotItemHolder.GetComponent<DragItem>().GetTextPosition();

            SlotItemHolder = newInstance;
            DragItem slotDragItem= SlotItemHolder.GetComponent<DragItem>();
            slotDragItem.Initiailize();
            slotDragItem.SetTextPosition(prevTextPos);
            inventoryUIManager.GetInventory().EraseItemInPosition(position);
            SetChildImageSize(SlotItemHolderSize);
            SetTextSize(textSize);
            slotDragItem.SetReturnOnMiss(returnOnMiss);
            SlotItemHolder.SetActive(false);
        }
        /// <summary>
        /// Adds a new slotchild when slot child is dragged away, and resets the slot to empty
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            inventoryUIManager.SetPressed(gameObject);
            inventoryUIManager.MoveOnPress(gameObject);
        }
        public void SetTextSize(float size)
        {
            textSize = size;
            if (SlotItemHolder != null)
            {
                SlotItemHolder.GetComponent<DragItem>().SetTextSize(size);

            }
            else
            {
                Debug.LogError("Slot Child Null");
            }
        }
        public void SetTextOffset(Vector3 offset)
        {
            if (SlotItemHolder != null)
            {
                SlotItemHolder.GetComponent<DragItem>().SetTextPositionOffset(offset);

            }
            else
            {
                Debug.LogError("Slot Child Null");

            }
        }
        public void SetImageOffSet(Vector3 offset)
        {
            if (SlotItemHolder != null)
            {
                SlotItemHolder.GetComponent<DragItem>().SetImagePositionOffset(offset);

            }
            else
            {
                Debug.LogError("Slot Child Null");

            }
        }
        public void SetChildImageSize(Vector2 size)
        {
            SlotItemHolder.GetComponent<DragItem>().SetImageSize(size);
            SlotItemHolderSize = size;
        }
        public float GetTextSize()
        {
            return SlotItemHolder.GetComponent<DragItem>().GetTextSize();
        }
        public Image GetSlotImage()
        {
            return slotImage;
        }
        public void SetSlotImage(Image newImage)
        {
            slotImage = newImage;
        }
        public GameObject GetItemHolder()
        {
            return SlotItemHolder;
        }
        public InventoryUIManager GetInventoryUI()
        {
            return inventoryUIManager;
        }
        public UnityEngine.Color GetColor()
        {
            return color;
        }
        public InventoryItem GetItem()
        {
            return item;
        }
        public void SetPosition(int position)
        {
            this.position = position;
        }
        public int GetPosition()
        {
            return position;
        }
        public void SetReturnOnMiss(bool destroyOnMiss)
        {
            if (destroyOnMiss)
            {
                returnOnMiss = false;
                SlotItemHolder.GetComponent<DragItem>().SetReturnOnMiss(returnOnMiss);
            }
            else
            {
                returnOnMiss = true;
                SlotItemHolder.GetComponent<DragItem>().SetReturnOnMiss(returnOnMiss);
            }
        }
    }
}
