using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Player/Create new spell")]

public class PlayerSpells : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] int levelLearned;
    [TextArea]
    [SerializeField] string description;

    [SerializeField] bool isLearned;
    [SerializeField] bool hasBranchingEvo;
    [SerializeField] PlayerSpells nextEvolveSpell;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int mp;

    public string Name {
        get { return name; }
    }

    public int LevelLearned {
        get { return levelLearned; }
    }

    public string Description {
        get { return description; }
    }

    public bool IsLearned {
        get { return isLearned; }
    }

    public bool HasBranchingEvo {
        get { return hasBranchingEvo; }
    }

    public PlayerSpells nextEvo {
        get { return nextEvolveSpell; }
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

    public int ManaPoints {
        get { return mp; }
    }
}
