// This is not an actual script but a guide for creating the QuestEntryPrefab

/*
To create the QuestEntryPrefab:

1. Create a new UI Button GameObject:
   - Right-click in Hierarchy → UI → Button
   - Size it appropriately (e.g., 400x60)

2. Adjust the Button component:
   - Set Transition to ColorTint
   - Adjust Colors for Normal, Highlighted, Pressed states
   - Set Normal color to a warm color like (1.0, 0.9, 0.6, 1.0) for active quests

3. Add Text (TextMeshProUGUI) for Quest Title:
   - Add a TextMeshProUGUI child to the button
   - Name it "QuestTitleText"
   - Set position to fill most of the button (leave space for icons)
   - Set alignment to left and middle
   - Set font, size (e.g., 16), and color as desired
   - Add Content Size Fitter if needed for longer titles

4. Optional - Add Quest Icon:
   - Add an Image child to the button
   - Name it "QuestIcon"
   - Position it on the left before the text
   - Size it appropriately (e.g., 40x40)
   - Set its Source Image to a scroll/task icon

5. Optional - Add Quest Status Icon:
   - Add an Image child to the button
   - Name it "StatusIcon" 
   - Position it on the right side
   - This can be used to show if a quest is tracked or has updates

6. Save as Prefab:
   - Drag from Hierarchy to Project view
   - Save in UI/Book/Prefabs folder as "QuestEntryPrefab"

This prefab will be instantiated for each quest in the journal.
*/ 