# PlayerSaveLink Component

This component connects the `Player` class (from `Player.cs`) to the save system, allowing you to save and load player health, mana, and spells.

## Setup Instructions

1. **Add the component to your player GameObject**:
   - Either use the menu: Tools → Save System → Setup Player Save Adapters
   - Or manually add the `PlayerSaveLink` component to your player GameObject

2. **Configure the component**:
   - If your `Player` instance is directly accessible, drag it into the `Player Reference` field
   - Otherwise, the component will try to find it automatically through:
     - A `BattleSystem` in the scene
     - As a child component of the player GameObject

3. **Ensure SpellManager is properly set up**:
   - If you're using `SpellManager`, make sure it's on the same GameObject as `PlayerSaveLink`
   - The system will save spells from either the `SpellManager` or directly from the `Player.Spells` list

## How It Works

The `PlayerSaveLink` component:
- Connects to the `SimpleSaveSystem` events (`OnSave` and `OnLoad`)
- When saving, it pulls data from the `Player` instance:
  - Basic stats (health, mana, level)
  - Combat stats (attack, defense, etc.)
  - Spells (either from `SpellManager` or `Player.Spells`)
- When loading, it applies the saved data back to the `Player` instance

## Troubleshooting

If player data isn't being saved:
1. Check the console for error messages
2. Verify the `Player` instance is being found (try assigning it directly)
3. Make sure `SimpleSaveSystem` is properly initialized before any save attempts
4. For spell saving issues, check that either `SpellManager` is accessible or `Player.Spells` is properly populated

## Integration with Existing System

This component works alongside the other adapter components like:
- `PlayerHealth`
- `PlayerMana`
- `PlayerLevel`
- etc.

The system will use data from whichever component it finds first. 