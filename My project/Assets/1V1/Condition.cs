using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Condition
{
    public ConditionID Id { get; set;}
    public string Name { get; set; }
    public string Descritpion { get; set; }
    public string StartMessage { get; set; }
    public Action<EnemyBase> OnStartMonster { get; set; }
    public Action<Player> OnStartPlayer { get; set; }
    public Func<EnemyBase, bool> OnBeforeMoveMonster { get; set; }
    public Func<Player, bool> OnBeforeSpellPlayer { get; set; }
    public Action<EnemyBase> OnAfterTurnMonster { get; set; }
    public Action<Player> OnAfterTurnPlayer { get; set; }
}
