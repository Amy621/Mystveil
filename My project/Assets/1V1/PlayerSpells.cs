using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Player/Create new spell")]

public class PlayerSpells : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PlayerSpells nextEvolveSpell;

    // Physical Attack or Special Attack or Status
    [SerializeField] string type;

    [SerializeField] int power;
    [SerializeField] int accuracy;

    // how much mana cost is the move
    [SerializeField] int mp;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public PlayerSpells nextEvo {
        get { return nextEvolveSpell; }
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

    public int ManaPoints {
        get { return mp; }
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
