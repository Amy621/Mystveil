using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Player/Create new player")]

public class PlayerStats : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    // player 2D image for the 1v1
    [SerializeField] Sprite playerImage;

    [SerializeField] GameObject playerObject;

    // Stats
    [SerializeField] int healthPoints;
    [SerializeField] int manaPoints;
    [SerializeField] int attackPoints;
    [SerializeField] int defensePoints;
    [SerializeField] int specialAttackPoints;
    [SerializeField] int specialDefensePoints;
    [SerializeField] int speed;
    [SerializeField] int charisma;

    [SerializeField] List<LearnableSpell> spells;

    public string Name { get {return name;}  }
    public string Description { get { return description; }}
    public Sprite Image { get { return playerImage; }}

    public int HP { get { return healthPoints; }}
    public int MANA { get { return manaPoints; }}
    public int ATK { get { return attackPoints; }}
    public int DEF { get { return defensePoints; }}
    public int SPA { get { return specialAttackPoints; }}
    public int SPD { get { return specialDefensePoints; }}
    public int SPE { get { return speed; }}
    public int CHA { get { return charisma; }}

    public List<LearnableSpell> LearnableSpells { get { return spells; }}
}

[System.Serializable]
public class LearnableSpell
{
    [SerializeField] PlayerSpells moveBase;

    public PlayerSpells Base {
        get { return moveBase; }
    }

    public int Level {
        get { return moveBase.LevelLearned; }
    }
}
