// This is not an actual script but a guide for creating the MonsterEntryPrefab

/*
To create the MonsterEntryPrefab:

1. Create a new UI Button GameObject:
   - Right-click in Hierarchy → UI → Button
   - Size it appropriately (e.g., 350x80)

2. Adjust the Button component:
   - Set Transition to ColorTint
   - Adjust Colors for Normal, Highlighted, Pressed states

3. Add Text (TextMeshProUGUI):
   - Add a TextMeshProUGUI child to the button
   - Name it "MonsterNameText"
   - Set position to left side
   - Set alignment to left
   - Set font, size, and color as desired

4. Add Monster Icon:
   - Add an Image child to the button
   - Name it "MonsterIcon"
   - Position it on the left before the text
   - Size it appropriately (e.g., 64x64)
   - Set its Source Image to a default icon or leave blank

5. Save as Prefab:
   - Drag from Hierarchy to Project view
   - Save in UI/Book/Prefabs folder as "MonsterEntryPrefab"

This prefab will be instantiated for each monster in the bestiary.
*/ 