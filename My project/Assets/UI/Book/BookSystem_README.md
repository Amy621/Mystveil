# Mystveil BookSystem

A persistent book UI system that works across scenes for displaying player information, lore, monsters, spells, items, and quests. This system is designed to work like the SimpleSaveSystem, requiring minimal setup.

## Simple Setup Instructions

1. Create a new GameObject in your starting scene (e.g., "BookSystem").
2. Add the `BookSystem` component to this GameObject.
3. Assign the following prefabs in the inspector:
   - Book Canvas Prefab - The main UI canvas for the book
   - Spell Button Prefab - Button for individual spells in the list
   - Equipped Spell Slot Prefab - Slot for equipped spells
   - Spell Details Panel Prefab - Panel for viewing spell details

That's it! The BookSystem will automatically handle:
- Loading and initializing the book UI
- Persisting across scene changes
- Setting up all required connections
- Handling spell management
- Proper tab navigation and locking/unlocking

## Usage

### Opening/Closing the Book
```csharp
// Toggle the book open/closed
BookSystem.Instance.ToggleBook();

// Open the book directly
BookSystem.Instance.OpenBook();

// Close the book
BookSystem.Instance.CloseBook();
```

### Unlocking Tabs
```csharp
// Unlock a tab by name
BookSystem.Instance.UnlockTab("Spells");
BookSystem.Instance.UnlockTab("Monsters");
BookSystem.Instance.UnlockTab("Lore");
```

## Prefab Setup

### BookCanvas Prefab
- Should contain a BookManager component
- Should have panels for: Introduction, Lore, Monsters, Spells, Items, Quests
- Should have tab buttons for each panel
- All panels should have their respective manager components (e.g., SpellPanelManager on the Spells panel)

### SpellButton Prefab
- Button with TextMeshProUGUI component for displaying spell name
- Optional Image component for spell icon

### EquippedSpellSlot Prefab
- Button with TextMeshProUGUI components for slot label and spell name
- Optional Image component for spell icon

### SpellDetailsPanel Prefab
- Panel with all necessary UI elements for displaying spell details
- Should include: 
  - SpellName text
  - SpellDescription text
  - SpellCost text
  - SpellType text
  - SpellIcon image
  - Equip/Unequip buttons
  - Close button

## Integration with SpellManager

The system will automatically find the SpellManager in your scene and connect to it. Make sure your SpellManager is properly set up to:

- Track unlocked spells
- Handle equipping/unequipping
- Manage spell slots
- Provide spell details

Any changes made via the book will be saved through the SimpleSaveSystem when a save occurs.

## Example Setup in Scene

1. Create GameObject named "BookSystem"
2. Add BookSystem component
3. Assign the pre-configured prefabs:
   - BookCanvasPrefab
   - SpellButtonPrefab
   - EquippedSpellSlotPrefab
   - SpellDetailsPanelPrefab
4. Configure settings:
   - Book Toggle Key (default: B)
   - Start With All Tabs Locked (default: true)
   - Introduction Unlocked By Default (default: true)
5. That's it! The system will handle everything else automatically. 