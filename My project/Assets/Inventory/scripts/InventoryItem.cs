using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
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
            amountText = GetComponent<TextMeshProUGUI>();
        amount = 1;
    }

    //left mouse click carries item
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("clicked on item " + myItem);
            Inventory.Singleton.SetCarriedItem(this); /////////////////
        }
    }
    public void SetText()
    {
        if(myItem != null && myItem.stackable)
        {
            //amountText.gameObject.SetActive(true);
            //amountText.SetText(amount.ToString());
        }
    }
}


