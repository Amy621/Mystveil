using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create new move")]

public class MonsterMoves : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterMoves nextEvolveMove;

    // Physical Attack or Special Attack
    [SerializeField] string type;
    [SerializeField] int power;
    [SerializeField] int accuracy;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public MonsterMoves NextEvo {
        get { return nextEvolveMove; }
    }

    public string Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public bool IsSpecial {
        get {
            if (type == "SPECIAL")
                return true;
            else 
                return false;
        }
    }
}
