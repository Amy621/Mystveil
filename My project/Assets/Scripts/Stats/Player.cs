using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public PlayerStats Base {get; set; }
    public int Level {get; set; }

    public int HP {get; set;}
    public int MANA {get; set;}
    public List<PlayerSpell> Spells { get; set; }

    public Player(PlayerStats pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;
        MANA = MaxMana;

        // Generate player spells
        Spells = new List<PlayerSpell>();
        foreach (var spell in Base.LearnableSpells)
        {
            if (spell.Level <= Level)
                Spells.Add(new PlayerSpell(spell.Base));

            if (Spells.Count >= 3)
                break;
        }
    }

    public int MaxHp {
        get { return Mathf.FloorToInt((Base.HP * Level) / 100f) + 10; }
    }

    public int MaxMana {
        get { return Base.MANA; }
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

    public int Charisma {
        get { return Base.CHA; }
    }

    public bool TakeDamage(MonsterMove move, EnemyBase attacker)
    {
        float attack = (move.Base.IsSpecial)? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial)? SpDefense : Defense;

        // formula for damage
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attack / defense) + 2;        
        int damage = Mathf.FloorToInt(d * modifiers);

        Debug.Log("Player takes: " + damage + " damage");

        HP -= damage;

        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }
}
