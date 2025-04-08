using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpell
{
    public PlayerSpells Base { get; set; }
    // public int MP { get; set; }

    public PlayerSpell(PlayerSpells pBase)
    {
        Base = pBase;
        // MP = pBase.ManaPoints;
    }
}
