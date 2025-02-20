using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    //Author: Jaxon Schauer
    /// <summary>
    /// Allows unity events with an inventoryItem input variable
    /// </summary>
    [System.Serializable]
    public class InventoryItemEvent : UnityEvent<InventoryItem> { }
    /// <summary>
    /// Allows unity events with an inventoryItem and vector3 input variable
    /// </summary>
    [System.Serializable]
    public class InventoryItemPosEvent : UnityEvent<Vector3, InventoryItem> { }

    [System.Serializable]
    public class InventoryItemSwapEvent : UnityEvent<InventoryItem, InventoryItem> { }
}

