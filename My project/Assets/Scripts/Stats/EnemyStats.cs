using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new monster")]

public class EnemyStats : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    // icon image used for the spellbook
    [SerializeField] Sprite monsterIconImage;

    // boss 2D image for the 1v1
    [SerializeField] Sprite bossImage;

    // Stats
    [SerializeField] int healthPoints;
    [SerializeField] int attackPoints;
    [SerializeField] int defensePoints;
    [SerializeField] int specialAttackPoints;
    [SerializeField] int specialDefensePoints;
    [SerializeField] int speed;

    // Moves
    [SerializeField] List<LearnableMove> moves;

    // Stat on drop, will be a string with [NAME: INT]
    [SerializeField] string dropStat;

    // Monster drop on defeat, will be a string with [NAME: INT]
    // else N/A
    [SerializeField] string monsterDrop1;
    [SerializeField] string monsterDrop2;
    [SerializeField] string monsterDrop3;
    [SerializeField] string monsterDrop4;

    public string Name { get {return name;}  }
    public string Description { get { return description; }}
    public Sprite MonsterIcon { get { return monsterIconImage; }}
    public Sprite BossImage { get { return bossImage; }}

    public int HP { get { return healthPoints; }}
    public int ATK { get { return attackPoints; }}
    public int DEF { get { return defensePoints; }}
    public int SPA { get { return specialAttackPoints; }}
    public int SPD { get { return specialDefensePoints; }}
    public int SPE { get { return speed; }}
    public List<LearnableMove> LearnableMoves { get { return moves; }}

    public string DropStat { get { return dropStat; }}

    public string MonsterDrop1 { get { return monsterDrop1; }}
    public string MonsterDrop2 { get { return monsterDrop2; }}
    public string MonsterDrop3 { get { return monsterDrop3; }}
    public string MonsterDrop4 { get { return monsterDrop4; }}
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MonsterMoves moveBase;
    int level;

    public MonsterMoves Base {
        get { return moveBase; }
    }

    public int Level {
        get { return level; }
    }
}