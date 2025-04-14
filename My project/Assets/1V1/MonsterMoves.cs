using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new move")]

public class MonsterMoves : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] int levelLearned;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterMoves nextEvolveMove;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;

    public string Name {
        get { return name; }
    }

    public int LevelLearned {
        get { return levelLearned; }
    }

    public string Description {
        get { return description; }
    }

    public MonsterMoves NextEvo {
        get { return nextEvolveMove; }
    }

    public MoveCategory Category {
        get { return category; }
    }

    public MoveEffects Effects {
        get { return effects; }
    }

    public List<SecondaryEffects> SecondaryEffects {
        get { return secondaryEffects; }
    }

    public MoveTarget Target {
        get { return target; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public bool AlwaysHits {
        get { return alwaysHits; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] NumHit numHit;
    [SerializeField] int restoreHP;
    [SerializeField] int restoreMP;
    [SerializeField] bool removeAllStatusChanges;
    [SerializeField] bool removeAllStatChanges;
    [SerializeField] ConditionID status;

    public List<StatBoost> Boosts {
        get { return boosts; }
    }

    public NumHit NumberOfHits {
        get { return numHit; }
    }

    public int RestoreHP {
        get { return restoreHP; }
    }

    public int RestoreMP {
        get { return restoreMP; }
    }

    public bool RemoveAllStatusChanges {
        get { return removeAllStatusChanges; }
    }

    public bool RemoveAllStatChanges {
        get { return removeAllStatChanges; }
    }

    public ConditionID Status {
        get { return status; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get { return chance; }
    }

    public MoveTarget Target {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

[System.Serializable]
public class NumHit
{
    public int minNum;
    public int maxNum;

    public int MinNum {
        get { return minNum; }
    }

    public int MaxNum {
        get { return maxNum; }
    }
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}
