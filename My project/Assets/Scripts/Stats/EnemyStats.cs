using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new monster")]

public class EnemyStats : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] bool isSpecialBoss;

    // icon image used for the spellbook
    [SerializeField] Sprite monsterIconImage;

    // boss 2D image for the 1v1
    [SerializeField] Sprite bossImage;

    [SerializeField] GameObject monster;

    // Stats
    [SerializeField] int healthPoints;
    [SerializeField] int attackPoints;
    [SerializeField] int defensePoints;
    [SerializeField] int specialAttackPoints;
    [SerializeField] int specialDefensePoints;
    [SerializeField] int speed;

    [SerializeField] int expYield;

    // Moves
    [SerializeField] List<LearnableMove> moves;
    [SerializeField] List<DropStat> dropStats;
    [SerializeField] List<Item> monsterDrops;
    [SerializeField] List<double> dropRates;

    public string Name { get {return name;}  }
    public string Description { get { return description; }}
    public bool IsSpecialBoss { get { return isSpecialBoss; }}
    public Sprite MonsterIcon { get { return monsterIconImage; }}
    public Sprite BossImage { get { return bossImage; }}

    public int HP { get { return healthPoints; }}
    public int ATK { get { return attackPoints; }}
    public int DEF { get { return defensePoints; }}
    public int SPA { get { return specialAttackPoints; }}
    public int SPD { get { return specialDefensePoints; }}
    public int SPE { get { return speed; }}
    public List<LearnableMove> LearnableMoves { get { return moves; }}

    public int ExpYield => expYield;

    public List<DropStat> DropStats { get { return dropStats; }}

    public List<Item> MonsterDrops { get { return monsterDrops; }}
    public List<double> DropRates { get { return dropRates; }}
}

public enum DropStat
{
    HP,
    ATK,
    DEF,
    SPA,
    SPD,
    SPE
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    Accuracy
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MonsterMoves moveBase;

    public MonsterMoves Base {
        get { return moveBase; }
    }

    public int Level {
        get { return moveBase.LevelLearned; }
    }
}