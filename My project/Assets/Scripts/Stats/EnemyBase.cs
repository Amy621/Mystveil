using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase
{
    public EnemyStats Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<MonsterMove> Moves { get; set; }

    // Generate enemy moves
    public EnemyBase(EnemyStats pBase, int plevel) 
    {
        Base = pBase;
        Level = plevel;

        HP = MaxHp;
        Moves = new List<MonsterMove>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new MonsterMove(move.Base));

            if (Moves.Count >= 4)
                break;
        }
    }
    
    public int MaxHp {
        get { return Mathf.FloorToInt((Base.HP * Level) / 100f) + 10; }
    }

    public int Attack {
        get { return Mathf.FloorToInt((Base.ATK * Level) / 100f) + 5; }
    }

    public int Defense {
        get { return Mathf.FloorToInt((Base.DEF * Level) / 100f) + 5; }
    }

    public int SpAttack {
        get { return Mathf.FloorToInt((Base.SPA * Level) / 100f) + 5; }
    }

    public int SpDefense {
        get { return Mathf.FloorToInt((Base.SPD * Level) / 100f) + 5; }
    }

    public int Speed {
        get { return Mathf.FloorToInt((Base.SPE * Level) / 100f) + 5; }
    }

    public bool TakeDamage(PlayerSpell spell, Player attacker)
    {
        attacker.MANA -= spell.Base.ManaPoints;

        float attack = (spell.Base.IsSpecial)? attacker.SpAttack : attacker.Attack;
        float defense = (spell.Base.IsSpecial)? SpDefense : Defense;

        // formula for damage
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * spell.Base.Power * ((float) attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        Debug.Log("Monster takes: " + damage + " damage");

        HP -= damage;

        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public MonsterMove GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
