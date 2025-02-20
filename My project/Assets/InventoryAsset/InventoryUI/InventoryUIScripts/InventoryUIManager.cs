using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    
    /// <summary>
    /// This class uses information given by the inventory controller to build a UI interface. This interface is linked with the <see cref="Inventory"/> class
    /// displaying the items contained inside the associated object.
    /// </summary>
    
    public class InventoryUIManager : MonoBehaviour
    {
        private GameObject previouslyHighlighted;

        // Inventory UI Configuration
        [Header("========[ Inventory UI Setup ]========")]
        [Tooltip("Name of the inventory for identification purposes.")]
        [SerializeField]
        private string inventoryName;

        [SerializeField, HideInInspector]
        private Inventory inventory; // Reference to the associated inventory object.

        [Tooltip("Number of rows for the inventory layout.")]
        [SerializeField]
        private int rows;

        [Tooltip("Number of columns for the inventory layout.")]
        [SerializeField]
        private int cols;

        [Tooltip("Prefab representing an individual item slot.")]
        [SerializeField]
        private GameObject slot;

        [Tooltip("Sets the slots image. Selected image is optional.")]
        [SerializeField]
        private slotImages SlotImage;

        [Tooltip("Size dimensions of each slot.")]
        [SerializeField]
        private Vector2 slotSize;

        [Tooltip("Spacing between individual slots in the inventory. It is recommended to use the same dimensions as slot size.")]
        [SerializeField]
        private Vector2 slotGap;

        [Tooltip("Size factor for the item image displayed in a slot. Gets multiplied by the slot size.")]
        [SerializeField]
        private Vector2 ItemImageSizeFactor;
        [SerializeField]
        private Vector2 ItemImageOffset;

        [Tooltip("Size of the text displaying item quantity, if chosen to display.")]
        [SerializeField] private float textSize;

        [Tooltip("Position of the item quantity text relative to the slot.")]
        [SerializeField] private Vector2 textPosition;

        [Header("========[Background Settings]========")]

        [Tooltip("Prefab for the background behind the inventory slots.")]
        [SerializeField]
        private GameObject background;

        [Tooltip("Activate and deactivate background.")]
        [SerializeField] bool activeBackground;

        [Tooltip("Dimensions for adjusting the background size.")]
        [SerializeField]
        private Vector2 backgroundBorder;

        [Tooltip("Offset for positioning slots with respect to the background.")]
        [SerializeField]
        private Vector2 slotOffsetToBackground;

        [Header("========[ Item Acceptance Configuration ]========")]

        [Tooltip("Specify the general rules for item acceptance in this inventory.")]
        [SerializeField]
        private ItemAcceptance acceptance;

        [Tooltip("Define any exceptions to the general item acceptance rules.")]
        [SerializeField]
        private List<string> exceptions;

        // Additional Configuration Options
        [Header("========[ Additional Options ]========")]

        [Tooltip("Determines the starting point for slot arrangement.")]
        [SerializeField]
        StartPositions slotStartPosition;

        [Tooltip("Enable dragging items within the inventory. NOTE: Must also be enabled in item.")]
        [SerializeField]
        private bool draggable;

        [Tooltip("Allow items to be highlighted when selected. NOTE: Pressable must also be enabled in item.")]
        [SerializeField]
        private bool clickable;
        [Tooltip("Enable saving the state of the inventory.")]
        [SerializeField]
        private bool saveInventory;

        [Space(10)]
        [Header("***Allows you to choose keys that")]
        [Space(-10)]
        [Header("enable and disable the inventory.***")]
        [Space(5)]
        [Tooltip("Key that toggles the visibility/enabling of the inventory. Must be a char.")]
        [SerializeField]
        private List<char> toggleOnButtonPress;

        [Space(10)]
        [Header("***Selects a specific slot when playing the")]
        [Space(-10)]
        [Header("item action on button press.***")]
        [Space(5)]
        [Tooltip("Defines which keys should highlight/press their given slots.")]
        [SerializeField]
        List<PressableSlot> SelectSlotOnButtonPress;

        [Space(10)]
        [Header("***Performs a given action when an")]
        [Space(-10)]
        [Header("item exits/enters inventory.***")]
        [Space(5)]
        [Tooltip("Sets actions for when an item enter or exits a specific inventory slot.")]
        [SerializeField]
        List<invokeOnExit> itemEntryExitAction;

        [Space(10)]
        [Header("***Moves an item to a specified inventory")]
        [Space(-10)]
        [Header("when clicked while holding specified button.***")]
        [Space(5)]
        [SerializeField]
        List<ItemMoveOnPress> moveItemOnButtonPress;

        [Space(10)]
        [Header("***Adds test items to inventory in editor")]
        [Space(-10)]
        [Header("helping design the inventory UI.***")]
        [Space(5)]
        [Tooltip("Makes test items for seeing how items will appear in the inventory. NOTE: these items will not persist once you press play.")]
        [SerializeField]
        List<TestItem> testItems;

        [Space(10)]
        [Header("***Invokes actions when no valid slot is")]
        [Space(-10)]
        [Header("found for an item.***")]
        [Space(5)]
        [Tooltip("Sets actions for when an item enter or exits a specific inventory slot.")]
        [SerializeField]
        private ItemMissInfo invokeOnMiss;

        Dictionary<string, int> slotPress = new Dictionary<string, int>(); //Links the string and the position to highlight on press.
        Dictionary<KeyCode, string> movePress; //Links the string and the position to highlight on press.

        private Transform UI;

        private RectTransform rectTransform;

        // Holds and organizes slots
        [SerializeField, HideInInspector]
        private List<GameObject> slots = new List<GameObject>();
        private List<Vector2> AllSlotVectorPos = new List<Vector2>();
        private Dictionary<Vector2, GameObject> VectorPositionToSlotDict = new Dictionary<Vector2, GameObject>();
        private Dictionary<int, GameObject> positionToSlotDict = new Dictionary<int, GameObject>();

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            UpdateInventoryUI();
        }

        public void Start()
        {
            if (!TestSetup()) return;

            UpdateInventoryUI();
            UI = InventoryController.instance.GetUI();


        }

        /// <summary>
        ///  Checks for input to pass to <see cref="HighLightOnButtonPress"/> on each frame
        /// </summary>
        private void Update()
        {
            HighLightOnButtonPress();
        }
        /// <summary>
        /// Allows background even when not child object to align activation with slots
        /// </summary>
        private void OnEnable()
        {
            if(background!=null&&activeBackground)
            {
                background.SetActive(true);

            }
        }
        /// <summary>
        /// Allows background even when not child object to align activation with slots
        /// </summary>
        private void OnDisable()
        {
            if (background != null && activeBackground)
            {
                background.SetActive(false);
            }
        }
        /// <summary>
        /// Initializes essential variables only when the Initialize Inventory button is pressed. Sets up a default inventoryUI
        /// </summary>
        public void SetVarsOnInit()
        {
            InventoryUIManager manager = InventoryController.instance.GetInventoryManagerPrefab().GetComponent<InventoryUIManager>();

            SlotImage.regular = manager.GetSlotImage();
            slotSize = manager.GetSlotSize();
            slotGap = manager.GetSlotGap();


            GameObject slotChild = slot.GetComponent<Slot>().GetItemHolder();
            RectTransform slotChildRect = slotChild.GetComponent<RectTransform>();
            ItemImageSizeFactor = new Vector2(slotChildRect.sizeDelta.x / slot.GetComponent<RectTransform>().sizeDelta.x, slotChildRect.sizeDelta.y / slot.GetComponent<RectTransform>().sizeDelta.y);
            slot.GetComponent<Slot>().GetItemHolder().GetComponent<DragItem>().Initiailize();

            textSize = slot.GetComponent<Slot>().GetItemHolder().GetComponent<DragItem>().GetTextSize();
        }

        /// <summary>
        /// Checks if any meaningful variables have been changed and, if so, calls the functions, creating the expected UI
        /// </summary>
        public void UpdateInventoryUI()
        {
            inventory.Init();

            SetSaveInventory();
            inventory.Resize(rows * cols);

            InventoryUIReset();

            createSlots();
            SetSlotOrder();
            SetBackground();
            UpdateInventory();
            InitSlotEnterExitDict();
            BackgroundActivity();
            initializeTestItems();
        }

        /// <summary>
        /// Resets any values that may carry over after changing the UI manager
        /// </summary>
        private void InventoryUIReset()
        {
            if (slot == null) return;
            if (Application.isPlaying)
            {
                foreach (GameObject slot in slots)
                {
                    Destroy(slot);
                }
            }
            else
            {
                foreach (GameObject slot in slots)
                {
                    DestroyImmediate(slot);
                }
            }
            slots.Clear();
            AllSlotVectorPos.Clear();
            VectorPositionToSlotDict.Clear();
            positionToSlotDict.Clear();
            slotPress.Clear();
        }

        /// <summary>
        /// Aligns the background with the slots while accounting for other user-defined offsets for the background
        /// </summary>
        private void SetBackground()
        {
            if (background == null)
            {
                return;
            }
            if (AllSlotVectorPos.Count <= 0)
            {
                return;
            }
            RectTransform rectTransform = background.GetComponent<RectTransform>();
            Vector2 backGroundArea = AllSlotVectorPos[0] - AllSlotVectorPos[AllSlotVectorPos.Count - 1];
            rectTransform.sizeDelta = new Vector2((Mathf.Abs(backGroundArea.x) + backgroundBorder.x), Mathf.Abs(backGroundArea.y) + backgroundBorder.y);
            background.transform.position = new Vector2(transform.position.x + slotOffsetToBackground.x, transform.position.y + slotOffsetToBackground.y);
        }

        /// <summary>
        /// Creates all slot UI on the inventoryUI based on the order created by <see cref="SetSlotPosition"/>, applying the slot gap and other offsets accordingly.
        /// Adds the slots into dictionaries and lists to track them
        /// </summary>

        private void createSlots()
        {
            InventoryUIReset();
            InitSlotPressDict();
            InitMovePressDict();
            rectTransform = GetComponent<RectTransform>();

            // Start from the top-left corner of the InventoryUI
            Vector2 startPlacementPos = rectTransform.right;

            for (int curRow = 0; curRow < rows; curRow++)
            {
                for (int curCol = 0; curCol < cols; curCol++)
                {
                    // Calculate the x and y position for each slot using the original slotGap and slotOffset values
                    float placeMentPosX = startPlacementPos.x + (curCol * slotGap.x);
                    float placeMentPosY = startPlacementPos.y - (curRow * slotGap.y);

                    Vector2 placeMentPos = new Vector2(placeMentPosX, placeMentPosY);

                    GameObject slotObjectInstance = Instantiate(slot, rectTransform);
                    slotObjectInstance.GetComponent<RectTransform>().localPosition = placeMentPos;
                    slotObjectInstance.GetComponent<RectTransform>().sizeDelta = slotSize;
                    slotObjectInstance.GetComponent<Image>().sprite = SlotImage.regular;
                    Slot slotInstance = slotObjectInstance.GetComponent<Slot>();
                    slotInstance.GetItemHolder().GetComponent<DragItem>().Initiailize();
                    slotInstance.SetChildImageSize(new Vector2(slotSize.x * ItemImageSizeFactor.x, slotSize.y * ItemImageSizeFactor.y));
                    slotInstance.SetTextSize(textSize);
                    slotInstance.SetTextOffset(textPosition);
                    slotInstance.SetReturnOnMiss(invokeOnMiss.destroyOnMiss);
                    slotInstance.SetImageOffSet(ItemImageOffset);
                    VectorPositionToSlotDict.Add(new Vector2(curRow, curCol), slotObjectInstance);
                    slots.Add(slotObjectInstance);
                    AllSlotVectorPos.Add(placeMentPos);
                }
            }
        }

        /// <summary>
        /// Checks chosen StartPosition and uses <see cref="SetSlotPosition"/> to numerically order each slot, ordering in the given format
        /// </summary>
        public void SetSlotOrder()
        {
            switch (slotStartPosition)
            {
                case StartPositions.TopRight:
                    SetSlotPosition(new Vector2(0, cols - 1), "down", "left");
                    break;
                case StartPositions.TopLeft:
                    SetSlotPosition(new Vector2(0, 0), "down", "right");
                    break;
                case StartPositions.BottomLeft:
                    SetSlotPosition(new Vector2(rows - 1, 0), "up", "right");
                    break;
                case StartPositions.BottomRight:
                    SetSlotPosition(new Vector2(rows - 1, cols - 1), "up", "left");
                    break;
                default:
                    SetSlotPosition(new Vector2(rows - 1, 0), "up", "right");
                    break;
            }

        }
        /// <summary>
        /// Determines item ordering based on the ordering option that was chosen. Takes as input a startPosition and the ordering directions.
        /// </summary>
        private void SetSlotPosition(Vector2 startPosition, string moveVerticle, string moveHorizontal)
        {
            if (positionToSlotDict == null)
                Debug.LogError("SlotPos Null");


            if (VectorPositionToSlotDict == null)
                Debug.LogError("SlotPositionVec Null");

            positionToSlotDict.Clear();

            int rowChange = 0;
            int colChange = 0;

            switch (moveVerticle.ToLower())
            {
                case "up":
                    rowChange = -1;
                    break;
                case "down":
                    rowChange = 1;
                    break;
            }

            switch (moveHorizontal.ToLower())
            {
                case "right":
                    colChange = 1;
                    break;
                case "left":
                    colChange = -1;
                    break;
            }

            Vector2 currentPos = startPosition;
            int currentPosition = 0;

            for (int curRow = 0; curRow < rows; curRow++)
            {
                for (int curCol = 0; curCol < cols; curCol++)
                {
                    if (VectorPositionToSlotDict.ContainsKey(currentPos))
                    {
                        GameObject slot = VectorPositionToSlotDict[currentPos];
                        slot.GetComponent<Slot>().SetPosition(currentPosition);
                        positionToSlotDict[currentPosition] = slot;
                        currentPosition++;
                    }
                    currentPos += new Vector2(0, colChange);  // Move horizontally first
                }
                currentPos = new Vector2(currentPos.x + rowChange, startPosition.y); // Reset the horizontal position and move vertically
            }
        }
        /// <summary>
        /// Loads information into the <see cref="Inventory"/> class about what items to accept or reject
        /// </summary>
        private void UpdateInventory()
        {
            if (acceptance == ItemAcceptance.AcceptAll)
            {
                inventory.SetupItemAcceptance(true, false, exceptions);
            }
            else
            {
                inventory.SetupItemAcceptance(false, true, exceptions);
            }
        }



        /// <summary>
        /// Sets slot as pressed, calling <see cref="InventoryItem.Selected"/>
        /// </summary>
        public void SetPressed(GameObject slot, bool overRide = false)
        {
            Slot slotInstance = slot.GetComponent<Slot>();
            InventoryItem item = slotInstance.GetItem();
            if (item.GetPressable() && clickable || item.GetIsNull() && clickable || item.GetPressable() && overRide || overRide && item.GetIsNull())
            {
                if (previouslyHighlighted != null)
                {
                    if (previouslyHighlighted == slot)
                    {
                        slotInstance.GetItem().Selected();
                        return;
                    }
                    UnHighlight(previouslyHighlighted);
                }
                Highlight(slot);
                slotInstance.GetItem().Selected();
                previouslyHighlighted = slot;
            }
        }

        /// <summary>
        /// Highlights the given slot and unhighlights the previous slot. Assigns selected image for slot or makes slot grey if selected image is null
        /// </summary>
        public void Highlight(GameObject slot)
        {
            Slot slotInstance = slot.GetComponent<Slot>();
            UnHighlight(previouslyHighlighted);
            if (SlotImage.selected == null)
            {
                slotInstance.GetSlotImage().color = Color.grey;
            }
            else
            {
                slotInstance.GetComponent<Image>().sprite = SlotImage.selected;
            }
            previouslyHighlighted = slot;

        }

        /// <summary>
        /// Unhighlights slots, applying regular image for slots.
        /// </summary>
        public void UnHighlight(GameObject slot)
        {
            if (slot != null)
            {
                Slot prevSlotInstance = slot.GetComponent<Slot>();

                if (SlotImage.selected == null)
                {
                    prevSlotInstance.GetSlotImage().color = prevSlotInstance.GetColor();
                }
                else
                {
                    slot.GetComponent<Image>().sprite = SlotImage.regular;
                }

            }
        }

        /// <summary>
        /// Resets the slot in rare cases where the previously highlighted slot should be disregarded
        /// </summary>
        public void ResetHighlight()
        {
            UnHighlight(previouslyHighlighted);
            previouslyHighlighted = null;
        }

        /// <summary>
        /// Uses <see cref="SetPressed"/> to highlight a user-defined slot on the press of a user-defined button.
        /// </summary>
        private void HighLightOnButtonPress()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ResetHighlight();
            }
            if (Input.anyKeyDown)
            {
                string input = Input.inputString;
                if (slotPress.ContainsKey(input))
                {
                    int position = slotPress[input];
                    SetPressed(positionToSlotDict[position], true);
                }
            }
        }
        public void MoveOnPress(GameObject slot)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MoveOnPressHelper(KeyCode.LeftShift,slot);

            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                MoveOnPressHelper(KeyCode.LeftControl, slot);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                MoveOnPressHelper(KeyCode.Space, slot);
            }
            else if (Input.GetKey(KeyCode.Tab))
            {
                MoveOnPressHelper(KeyCode.Tab, slot);
            }
        }
        private void MoveOnPressHelper(KeyCode key, GameObject slot)
        {
            if (movePress.ContainsKey(key))
            {
                InventoryItem item = slot.GetComponent<Slot>().GetItem();
                if (item == null || item.GetIsNull())
                {
                    return;
                }
                if (InventoryController.instance.checkEnabled(movePress[key]) &&
                    !InventoryController.instance.InventoryFull(movePress[key], item.GetItemType()))
                {
                    InventoryController.instance.RemoveItem(inventoryName, item, item.GetAmount());
                    InventoryController.instance.AddItem(movePress[key], item.GetItemType(), item.GetAmount());
                }
                else
                {
                    Debug.Log("Inventory cannot fit item or Inventory disabled");
                }
            }
        }
        /// <summary>
        /// Defines the <see cref="slotPress"/> dictionary, allowing for <see cref="HighLightOnButtonPress"/> to efficiently detect a button click
        /// </summary>
        private void InitSlotPressDict()
        {
            foreach (PressableSlot press in SelectSlotOnButtonPress)
            {
                if (!slotPress.ContainsKey(press.buttonPress.ToString()))
                {
                    slotPress.Add(press.buttonPress.ToString(), press.position);

                }
                else
                {
                    Debug.LogWarning("Each press position must be unique");
                }
            }
        }
        public void InitMovePressDict()
        {
            movePress = new Dictionary<KeyCode, string>();
            foreach (ItemMoveOnPress move in moveItemOnButtonPress)
            {
                switch (move.moveButton)
                {
                    case MoveOptions.Ctrl:
                        if (!movePress.ContainsKey(KeyCode.LeftControl))
                        {
                            movePress.Add(KeyCode.LeftControl, move.inventoryName);
                        }
                        else
                        {
                            Debug.LogWarning("movePress already contains LeftControl");
                        }
                        break;
                    case MoveOptions.Shift:
                        if (!movePress.ContainsKey(KeyCode.LeftShift))
                        {
                            movePress.Add(KeyCode.LeftShift, move.inventoryName);
                        }
                        else
                        {
                            Debug.LogWarning("movePress already contains LeftControl");
                        }
                        break;
                    case MoveOptions.Tab:
                        if (!movePress.ContainsKey(KeyCode.Tab))
                        {
                            movePress.Add(KeyCode.Tab, move.inventoryName);
                        }
                        else
                        {
                            Debug.LogWarning("movePress already contains LeftControl");
                        }
                        break;
                    case MoveOptions.Space:
                        if (!movePress.ContainsKey(KeyCode.Space))
                        {
                            movePress.Add(KeyCode.Space, move.inventoryName);
                        }
                        else
                        {
                            Debug.LogWarning("movePress already contains LeftControl");
                        }
                        break;

                }

            }
        }
        /// <summary>
        /// Creates test reference items to help design UI.
        /// NOTE: Only for reference, will not be added at runtime.
        /// </summary>
        public void initializeTestItems()
        {
            if (testItems != null && !Application.isPlaying)
            {
                foreach (TestItem item in testItems)
                {
                    if (positionToSlotDict.ContainsKey(item.position))
                    {
                        Slot slot = positionToSlotDict[item.position].GetComponent<Slot>();
                        slot.GetItemHolder().GetComponent<DragItem>().SetImage(item.image);
                        slot.GetItemHolder().GetComponent<DragItem>().SetTextTestImage(item.amount);
                        slot.SetImageOffSet(ItemImageOffset);
                        slot.GetItemHolder().SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("Test Position does not exist");
                    }
                }
            }
        }
        /// <summary>
        /// Allows brackground activity to make all inventory activity
        /// </summary>
        public void BackgroundActivity()
        {
            if (background != null && !activeBackground)
            {
                background.SetActive(false);
            }
            else if (background != null && activeBackground)
            {
                background.SetActive(true);
            }
        }
        /// <summary>
        /// Passes needed information to map enter/exit actions for items 
        /// into <see cref="Inventory.SetExitEntranceDict(Dictionary{int, InventoryItemEvent}, Dictionary{int, InventoryItemEvent}, Dictionary{int, bool})"/>
        /// </summary>
        private void InitSlotEnterExitDict()
        {

            if (itemEntryExitAction.Count != 0)
            {
                Dictionary<int, InventoryItemEvent> enter = new Dictionary<int, InventoryItemEvent>();
                Dictionary<int, InventoryItemEvent> exit = new Dictionary<int, InventoryItemEvent>();
                Dictionary<int, bool> actItem = new Dictionary<int, bool>();
                foreach (invokeOnExit enterExit in itemEntryExitAction)
                {
                    if (enterExit.enterExit == EnterExit.Exit)
                    {
                        if (!exit.ContainsKey(enterExit.slotpos))
                        {
                            exit.Add(enterExit.slotpos, enterExit.action);
                            inventory.SetExitEntranceDict(enter, exit, actItem);
                        }
                        else
                        {
                            Debug.LogWarning("Each slot can only have one Exit action, ignoring: " + enterExit.slotpos);
                        }
                    }
                    else
                    {
                        if (!enter.ContainsKey(enterExit.slotpos))
                        {
                            enter.Add(enterExit.slotpos, enterExit.action);
                            actItem.Add(enterExit.slotpos, enterExit.ItemActOnEnter);
                            inventory.SetExitEntranceDict(enter, exit, actItem);

                        }
                        else
                        {
                            Debug.LogWarning("Each slot can only have one Enter action, ignoring: " + enterExit.slotpos);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Tests that InventoryController is Setup to support the InventoryUIManager, destroying the InventoryUIManager if not
        /// </summary>
        public bool TestSetup()
        {
            if (!InventoryController.instance.checkUI(gameObject))
            {
                Debug.LogError("inventoryUIDict does not contain object, this can be caused by not initializing through InventoryController or not unpacking InventoryController. Destroying: " + gameObject.name);
                Destroy(gameObject);
                return false;
            }
            if (InventoryController.instance == null)
            {
                Debug.LogError("Inventory Controller instance is null. Destroying inventory: " + inventoryName);
                Destroy(gameObject);
                return false;
            }
            if (!InventoryController.instance.TestinventoryManagerObjSetup())
            {
                Debug.LogError("Inventory object setup is incorrect. Destroying inventory: " + inventoryName);
                Destroy(gameObject);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Connects the button press and position values.
        /// </summary>
        [System.Serializable]
        private struct PressableSlot
        {
            public int position;

            public char buttonPress;
        }

        /// <summary>
        /// Connects the highlighted image with the regular image.
        /// </summary>
        [System.Serializable]
        private struct slotImages
        {
            [Tooltip("Displays regular slot image.")]
            public Sprite regular;
            [Tooltip("OPTIONAL: Displays slot Image when selected.")]
            public Sprite selected;
        }

        /// <summary>
        /// Connects values and invokes enter/exit actions
        /// </summary>
        [System.Serializable]
        private struct invokeOnExit
        {
            public EnterExit enterExit;

            public int slotpos;

            public InventoryItemEvent action;

            public bool ItemActOnEnter;
        }
        /// <summary>
        /// Takes information for test items
        /// </summary>
        [System.Serializable]
        private struct TestItem
        {
            public Sprite image;

            public int amount;

            public int position;
        }
        [System.Serializable]
        private struct ItemMoveOnPress
        {
            public MoveOptions moveButton;

            public string inventoryName;
        }
        /// <summary>
        /// holds information for when item misses
        /// </summary>
        [System.Serializable]
        private struct ItemMissInfo
        {
            public bool destroyOnMiss;

            public InventoryItemPosEvent missAction;

            public InventoryItemSwapEvent missOverSlotAction;
        }

        public void InvokeMiss(Vector3 pos, InventoryItem item)
        {
            invokeOnMiss.missAction.Invoke(pos, item);
        }

        public bool InvokeMissOverSlot(InventoryItem item1, InventoryItem item2)
        {
            if(invokeOnMiss.missOverSlotAction!=null && !(invokeOnMiss.missOverSlotAction.GetPersistentEventCount()==0))
            {
                invokeOnMiss.missOverSlotAction.Invoke(item1, item2);
                return true;
            }
            return false;
        }

        public InventoryItem GetInventoryItem(int index)
        {
            return inventory.InventoryGetItem(index);
        }

        public void SetSave(bool save)
        {
            saveInventory = save;
        }

        public void SetSaveInventory()
        {
            inventory.SetSave(saveInventory);
        }

        public void SetRowCol(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            UpdateInventoryUI();
        }

        public void SetInventoryName(string inventoryName)
        {
            this.inventoryName = inventoryName;
        }

        public string GetInventoryName()
        {
            return inventory.GetName();

        }

        public ref Inventory GetInventory()
        {
            return ref inventory;
        }

        public void SetInventory(ref Inventory inventory)
        {
            this.inventory = inventory;
        }

        public void UpdateSlot(int location)
        {
            if (positionToSlotDict.ContainsKey(location))
            {
                positionToSlotDict[location].GetComponent<Slot>().UpdateSlot();

            }
            else
            {
                Debug.LogError("Dictionary does not contain slot at: " + location);
            }
        }

        public Transform GetUI()
        {
            return UI;
        }

        public void SetDraggable(bool draggable)
        {
            this.draggable = draggable;
        }

        public void SetHighlightable(bool highlightable)
        {
            this.clickable = highlightable;
        }

        public bool GetDraggable()
        {
            return draggable;
        }

        private void OnDestroy()
        {
            inventory = null;
        }

        public void SetInvToggle(List<char> EnableDisableOnPress)
        {
            this.toggleOnButtonPress = EnableDisableOnPress;
        }

        public List<char> GetEnableDisable()
        {
            return toggleOnButtonPress;
        }
        public Vector3 GetSlotSize()
        {
            return slotSize;
        }
        public Vector3 GetSlotGap()
        {
            return slotGap;
        }
        public Sprite GetSlotImage()
        {
            return SlotImage.regular;
        }
        private enum StartPositions
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight,
        }

        private enum ItemAcceptance
        {
            AcceptAll,
            RejectAll
        }

        private enum EnterExit
        {
            Enter,
            Exit
        }
        private enum MoveOptions
        {
            Ctrl,
            Shift,
            Space,
            Tab,
        }

    }
}
