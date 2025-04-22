using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    public int amount {get; set;}
    public TextMeshProUGUI amountText; //text to display amount of item
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
    }

    //takes item type and destination slot to initialize the item
    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        itemIcon.sprite = item.sprite;
        if(myItem.stackable)
            amountText = GetComponentInChildren<TextMeshProUGUI>();
        amount = 1;
        SetText();
    }

    //left mouse click carries item
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.Singleton.isIn1v1)
            {
                Inventory.Singleton.battle.useItem = myItem;
                Debug.Log("Inventory Item my item: " + myItem);
                Inventory.Singleton.removeItems(null, 1, activeSlot);
                Inventory.Singleton.view1v1();
                
                // Invoke the action to signal Battle to use the item
                Inventory.Singleton.battle.onUseItemRequested?.Invoke();
            }
            else 
            {
                Debug.Log("clicked on item " + myItem);
                if(Input.GetKey(KeyCode.LeftShift)){
                    //shift click
                    Inventory.Singleton.Combine(activeSlot);
                }
                else{
                    Inventory.Singleton.SetCarriedItem(this); 
                }
            }
        }
    }
    public void SetText()
    {
        if(myItem != null)
        {
            amountText.gameObject.SetActive(true);
            amountText.SetText(amount.ToString());
        }
    }
}


