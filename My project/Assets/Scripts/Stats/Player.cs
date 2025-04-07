using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    PlayerStats _base;
    int level;

    public int HP {get; set;}
    public List<PlayerSpell> Spells { get; set; }

    public Player(PlayerStats pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        HP = MaxHp;

        // Generate player spells
        Spells = new List<PlayerSpell>();
        foreach (var spell in _base.LearnableSpells)
        {
            if (spell.Level <= level)
                Spells.Add(new PlayerSpell(spell.Base));

            if (Spells.Count >= 3)
                break;
        }
    }

    public int MaxHp {
        get { return Mathf.FloorToInt((_base.HP * level) / 100f) + 10; }
    }

    public int MaxMana {
        get { return Mathf.FloorToInt((_base.MANA * level) / 100f) + 10; }
    }

    public int Attack {
        get { return Mathf.FloorToInt((_base.ATK * level) / 100f) + 5; }
    }

    public int Defense {
        get { return Mathf.FloorToInt((_base.DEF * level) / 100f) + 5; }
    }

    public int SpAttack {
        get { return Mathf.FloorToInt((_base.SPA * level) / 100f) + 5; }
    }

    public int SpDefense {
        get { return Mathf.FloorToInt((_base.SPD * level) / 100f) + 5; }
    }

    public int Speed {
        get { return Mathf.FloorToInt((_base.SPE * level) / 100f) + 5; }
    }

    public int Charisma {
        get { return _base.CHA; }
    }
}
