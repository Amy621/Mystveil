using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurnMonster = (EnemyBase enemy) =>
                {
                    enemy.UpdateHP(enemy.MaxHp / 8);
                    enemy.StatusChanges.Enqueue($"{enemy.Base.Name} was damaged by poison!");
                },
                OnAfterTurnPlayer = (Player player) =>
                {
                    player.UpdateHP(player.MaxHp / 8);
                    player.StatusChanges.Enqueue($"{player.Base.Name} was damaged by poison!");
                }
            }
        },
        {
            ConditionID.badpsn,
            new Condition()
            {
                Name = "Badly Poisoned",
                StartMessage = "has been badly poisoned",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    enemy.StatusTime = 0;
                },
                OnStartPlayer = (Player player) =>
                {
                    player.StatusTime = 0;
                },
                OnAfterTurnMonster = (EnemyBase enemy) =>
                {
                    enemy.StatusTime++;

                    if (enemy.StatusTime >= 7)
                    {
                        enemy.StatusTime = 7;
                    }

                    enemy.UpdateHP(enemy.MaxHp / 8 * enemy.StatusTime);
                    enemy.StatusChanges.Enqueue($"{enemy.Base.Name} was damaged by poison!");
                },
                OnAfterTurnPlayer = (Player player) =>
                {
                    player.StatusTime++;

                    if (player.StatusTime >= 7)
                    {
                        player.StatusTime = 7;
                    }

                    player.UpdateHP(player.MaxHp / 8 * player.StatusTime);
                    player.StatusChanges.Enqueue($"{player.Base.Name} was damaged by poison!");
                }
            }
        },
        {
            ConditionID.burn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    // After 7 turns, the burn is shaken off
                    enemy.StatusTime = 7;
                },
                OnStartPlayer = (Player player) =>
                {
                    // After 7 turns, the burn is shaken off
                    player.StatusTime = 7;
                },
                OnAfterTurnMonster = (EnemyBase enemy) =>
                {
                    if(enemy.StatusTime <= 0)
                    {
                        enemy.CureStatus();
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} shook off the burn!");
                    } else {
                        enemy.StatusTime--;
                        enemy.UpdateHP(enemy.MaxHp / 8);
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} was damaged by burn!");
                    }
                },
                OnAfterTurnPlayer = (Player player) =>
                {
                    if(player.StatusTime <= 0)
                    {
                        player.CureStatus();
                        player.StatusChanges.Enqueue($"{player.Base.Name} shook off the burn!");
                    } else {
                        player.StatusTime--;
                        player.UpdateHP(player.MaxHp / 8);
                        player.StatusChanges.Enqueue($"{player.Base.Name} was damaged by burn!");
                    }
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    // After 1-3 turns, the monster wakes up
                    enemy.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {enemy.StatusTime} moves");
                },
                OnStartPlayer = (Player player) =>
                {
                    // After 1-3 turns, the player wakes up
                    player.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {player.StatusTime} moves");
                },
                OnBeforeMoveMonster = (EnemyBase enemy) =>
                {
                    if(enemy.StatusTime <= 0)
                    {
                        enemy.CureStatus();
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} woke up!");
                        return true;
                    } else {
                        enemy.StatusTime--;
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} is sleeping!");
                        return false;
                    }
                },
                OnBeforeSpellPlayer = (Player player) =>
                {
                    if(player.StatusTime <= 0)
                    {
                        player.CureStatus();
                        player.StatusChanges.Enqueue($"{player.Base.Name} woke up!");
                        return true;
                    } else {
                        player.StatusTime--;
                        player.StatusChanges.Enqueue($"{player.Base.Name} is sleeping!");
                        return false;
                    }
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralysis",
                StartMessage = "has been paralyzed",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    enemy.StatusTime = 3;
                },
                OnStartPlayer = (Player player) =>
                {
                    player.StatusTime = 3;
                },
                OnBeforeMoveMonster = (EnemyBase enemy) =>
                {
                    if (enemy.StatusTime <= 0)
                    {
                        enemy.CureStatus();
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} shook off the paralysis!");
                        return true;
                    }

                    enemy.StatusTime--;

                    if (Random.Range(1, 5) == 1)
                    {
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} is paralyzed and can't move!");
                        return false;
                    }

                    return true;
                },
                OnBeforeSpellPlayer = (Player player) =>
                {
                    if (player.StatusTime <= 0)
                    {
                        player.CureStatus();
                        player.StatusChanges.Enqueue($"{player.Base.Name} shook off the paralysis!");
                        return true;
                    }

                    if (Random.Range(1, 5) == 1)
                    {
                        player.StatusChanges.Enqueue($"{player.Base.Name} is paralyzed and can't move!");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.swm,
            new Condition()
            {
                Name = "Swarm",
                StartMessage = "has been targeted by the swarm",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    // After 5 turns, the swarm leaves
                    enemy.StatusTime = 5;
                },
                OnStartPlayer = (Player player) =>
                {
                    // After 5 turns, the swarm leaves
                    player.StatusTime = 5;
                },
                OnAfterTurnMonster = (EnemyBase enemy) =>
                {
                    if (enemy.StatusTime <= 0)
                    {
                        enemy.CureStatus();
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} escaped the swarm!");
                    } else {
                        enemy.StatusTime--;
                        enemy.UpdateHP(enemy.MaxHp / 8);
                        enemy.StatusChanges.Enqueue($"{enemy.Base.Name} was damaged by the swarm!");
                    }
                },
                OnAfterTurnPlayer = (Player player) =>
                {
                    if (player.StatusTime <= 0)
                    {
                        player.CureStatus();
                        player.StatusChanges.Enqueue($"{player.Base.Name} escaped the swarm!");
                    } else {
                        player.StatusTime--;
                        player.UpdateHP(player.MaxHp / 8);
                        player.StatusChanges.Enqueue($"{player.Base.Name} was damaged by the swarm!");
                    }
                }
            }
        },
        {
            ConditionID.stun,
            new Condition()
            {
                Name = "Stun",
                StartMessage = "was stunned",
                OnStartMonster = (EnemyBase enemy) =>
                {
                    enemy.VolatileStatusTime = 1; 
                },
                OnBeforeMoveMonster = (EnemyBase enemy) =>
                {
                    if (enemy.VolatileStatusTime <= 0) {
                        enemy.CureVolatileStatus();
                        return true;
                    }

                    enemy.VolatileStatusTime--;
                    enemy.StatusChanges.Enqueue($"{enemy.Base.Name} was stunned!");
                    return false;
                }
            }
        },
    };
}

public enum ConditionID
{
    none, psn, badpsn, burn, slp, par, swm, stun
}
