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

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set;}
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    public Player(PlayerStats pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;

        // Generate player spells
        Spells = new List<PlayerSpell>();
        foreach (var spell in Base.LearnableSpells)
        {
            if (spell.Level <= Level) {
                Spells.Add(new PlayerSpell(spell.Base));
            }

            if (Spells.Count >= 4)
                break;
        }

        CalculateStats();
        HP = MaxHp;
        MANA = MaxMana;

        ResetStatBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.ATK * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.DEF * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SPA * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SPD * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.SPE * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.HP * Level) / 100f) + 10 + Level;
        MaxMana = Mathf.FloorToInt((Base.MANA * Level) / 100f) + 10 + Level;
    }

    public void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int MaxHp { get; private set; }
    public int MaxMana { get; private set; }

    public int Attack {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed {
        get { return GetStat(Stat.Speed); }
    }

    public int Charisma {
        get { return Base.CHA; }
    }

    public bool TakeDamage(MonsterMove move, EnemyBase attacker)
    {
        float attack = (move.Base.Category == MoveCategory.Special)? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special)? SpDefense : Defense;

        // formula for damage
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attack / defense) + 2;        
        int damage = Mathf.FloorToInt(d * modifiers);

        Debug.Log("Player takes: " + damage + " damage");
        Debug.Log("Player HP: " + HP);

        UpdateHP(damage);

        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;

        Status = ConditionDB.Conditions[conditionId];
        Status?.OnStartPlayer?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}!");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionDB.Conditions[conditionId];
        VolatileStatus?.OnStartPlayer?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}!");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeSpellPlayer != null)
        {
            if(!Status.OnBeforeSpellPlayer(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeSpellPlayer != null)
        {
            if(!VolatileStatus.OnBeforeSpellPlayer(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurnPlayer?.Invoke(this);
        VolatileStatus?.OnAfterTurnPlayer?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}