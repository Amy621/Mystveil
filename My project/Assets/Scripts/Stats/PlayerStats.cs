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

    [SerializeField] GrowthRate growthRate;

    [SerializeField] List<LearnableSpell> spells;

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return (level * level * level);
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return 5 * (level * level * level) / 4;
        }
        else if (growthRate == GrowthRate.Fluctuating)
        {
            return GetFluctuating(level);
        }

        return -1;
    }

    public int GetFluctuating(int level)
    {
        if (level <= 15)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3) + 24) / 50));
        }
        else if (level >= 15 && level <= 36)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50));
        }
        else
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2) + 32) / 50));
        }
    }

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

    public GrowthRate GrowthRate => growthRate;
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

// will only be using medium slow
public enum GrowthRate
{
    Fast, MediumFast, MediumSlow, Fluctuating
}
