// This is not an actual script but a guide for creating the SpellEntryPrefab

/*
To create the SpellEntryPrefab:

1. Create a new UI Button GameObject:
   - Right-click in Hierarchy → UI → Button
   - Size it appropriately (e.g., 350x70)

2. Adjust the Button component:
   - Set Transition to ColorTint
   - Adjust Colors for Normal, Highlighted, Pressed states
   - Set Normal color based on spell rarity (e.g., common: (0.8, 0.8, 0.8, 1.0))

3. Add Text (TextMeshProUGUI):
   - Add a TextMeshProUGUI child to the button
   - Name it "SpellNameText"
   - Set position to leave space for the icon (e.g., position on right side)
   - Set alignment to left and middle
   - Set font, size, and color as desired

4. Add Spell Icon:
   - Add an Image child to the button
   - Name it "SpellIcon"
   - Position it on the left side
   - Size it appropriately (e.g., 60x60)
   - Set its Source Image to a default spell icon or leave blank

5. Optional - Add mana cost indicator:
   - Add a small UI element (e.g., TextMeshProUGUI) to show the mana cost
   - Position it in a corner of the button
   - This can be useful for quickly identifying spell costs

6. Save as Prefab:
   - Drag from Hierarchy to Project view
   - Save in UI/Book/Prefabs folder as "SpellEntryPrefab"

This prefab will be instantiated for each spell in the spellbook.
*/ 