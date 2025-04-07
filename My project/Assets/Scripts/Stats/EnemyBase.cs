using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase
{
    EnemyStats _base;

    public int HP { get; set; }
    public List<MonsterMove> Moves { get; set; }

    // Generate enemy moves
    public EnemyBase(EnemyStats pBase) 
    {
        _base = pBase;
        HP = MaxHp;
        foreach (var move in _base.LearnableMoves) {
            Moves.Add(new MonsterMove(move.Base));
        }
    }
    
    public int MaxHp {
        get { return _base.HP; }
    }

    public int Attack {
        get { return _base.ATK; }
    }

    public int Defense {
        get { return _base.DEF; }
    }

    public int SpAttack {
        get { return _base.SPA; }
    }

    public int SpDefense {
        get { return _base.SPD; }
    }

    public int Speed {
        get { return _base.SPE; }
    }
}
