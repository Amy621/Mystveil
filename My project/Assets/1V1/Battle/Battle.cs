using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, BattlePhase, BattleOver, Busy }

public class Battle : MonoBehaviour
{
    [SerializeField] PlayerUnit playerUnit;
    [SerializeField] PlayerHud playerHud;
    [SerializeField] EnemyUnit enemyUnit;
    [SerializeField] EnemyHud enemyHud;
    [SerializeField] BattleDialogueBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    private bool isShowingError = false;
    int escapeAttempts;
    bool willDropItems;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Player);

        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Enemy);

        dialogBox.SetSpellNames(playerUnit.Player.Spells);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Enemy.Base.Name} appeared.");
        yield return new WaitForSeconds(1f);

        ChooseFirstTurn();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
        dialogBox.OnActionSelected += HandleActionSelectionClick;
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.OnActionSelected -= HandleActionSelectionClick;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
        dialogBox.OnMoveSelected += HandleMoveSelectionClick;
        dialogBox.OnMoveHovered += HandleMoveHover;
        UpdateMoveSelectionUI();
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.BattlePhase;
        dialogBox.OnMoveSelected -= HandleMoveSelectionClick;
        dialogBox.OnMoveHovered -= HandleMoveHover;

        bool canRunMove = playerUnit.Player.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowPlayerStatusChanges(playerUnit.Player);
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyMove());
            yield break;
        }

        yield return ShowPlayerStatusChanges(playerUnit.Player);

        var spell = playerUnit.Player.Spells[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name} used {spell.Base.Name}!");

        yield return new WaitForSeconds(1f);

        if (CheckIfPlayerSpellHits(spell, playerUnit.Player, enemyUnit.Enemy)) {
            if (spell.Base.Category == MoveCategory.Status)
            {
                yield return RunPlayerMoveEffects(spell.Base.Effects, playerUnit.Player, enemyUnit.Enemy, spell.Base.Target);

                yield return playerHud.UpdateMana();
                yield return new WaitForSeconds(1f);

            } else if (spell.Base.Effects.NumberOfHits.MinNum != 0) {
                
                var multiHit = spell.Base.Effects.NumberOfHits;
                int numTimes = 0;
                for (int start = multiHit.MinNum; start <= multiHit.MaxNum; start++) 
                {
                    bool isDefeated = enemyUnit.Enemy.TakeDamage(spell, playerUnit.Player);
                    numTimes++;

                    yield return enemyHud.UpdateHP();
                    yield return playerHud.UpdateMana();

                    yield return new WaitForSeconds(1f);
                    
                    if (isDefeated) {
                        break;
                    }
                }
                
                yield return dialogBox.TypeDialog($"{spell.Base.Name} hit {numTimes} times!");

                yield return new WaitForSeconds(1f);

            } else {
                bool isDefeated = enemyUnit.Enemy.TakeDamage(spell, playerUnit.Player);
                yield return enemyHud.UpdateHP();
                yield return playerHud.UpdateMana();
            }

            if (spell.Base.SecondaryEffects != null && spell.Base.SecondaryEffects.Count > 0 && enemyUnit.Enemy.HP > 0)
            {
                foreach (var secondary in spell.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunPlayerMoveEffects(secondary, playerUnit.Player, enemyUnit.Enemy, secondary.Target);
                }
            }

            if (enemyUnit.Enemy.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} was defeated!");
                // enemy faint animation goes here

                yield return new WaitForSeconds(2f);
                willDropItems = true;
                BattleOver(true);

            } else { StartCoroutine(EnemyMove()); }
        }
        else 
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name}'s spell missed!");
            StartCoroutine(EnemyMove());
        }
    }

    void HandleActionSelectionClick(int actionIndex)
    {
        if (state == BattleState.PlayerAction)
        {
            currentAction = actionIndex;
            if (currentAction == 0)
            {
                // Attack
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Item
                Debug.Log("Item selected (not implemented)");
            }
            else if (currentAction == 2)
            {
                // Run
                StartCoroutine(TryToEscape());
            }
        }
    }

    public void HandleUpdate()
    {

    }

    void HandleMoveHover(int moveIndex)
    {
        if (state == BattleState.PlayerMove)
        {
            currentMove = moveIndex;
            dialogBox.UpdateSpellSelection(currentMove);
        }
    }

    void HandleMoveSelectionClick(int moveIndex)
    {
        if (state == BattleState.PlayerMove && !isShowingError)
        {
            currentMove = moveIndex;
            var spell = playerUnit.Player.Spells[currentMove];
            // not enough mana to cast
            if (spell.Base.ManaPoints > playerUnit.Player.MANA)
            {
                Debug.Log("Not enough mana!");
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(ErrorMovePopUp());
            }
            else
            {
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(PerformPlayerMove());
            }
        }
    }

    IEnumerator ErrorMovePopUp() {
        yield return dialogBox.TypeDialog("Not enough mana to cast!");

        yield return new WaitForSeconds(1f);
        dialogBox.SetDialog("");
        dialogBox.EnableMoveSelector(true);
        dialogBox.EnableDialogText(false);
        isShowingError = false;
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        bool canRunMove = enemyUnit.Enemy.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowEnemyStatusChanges(enemyUnit.Enemy);
            yield return new WaitForSeconds(1f);
            PlayerAction();
            yield break;
        }

        yield return ShowEnemyStatusChanges(enemyUnit.Enemy);

        yield return new WaitForSeconds(1f);

        var move = enemyUnit.Enemy.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} used {move.Base.Name}!");

        yield return new WaitForSeconds(1f);

        if (CheckIfMonsterMoveHits (move, enemyUnit.Enemy, playerUnit.Player))
        {
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMonsterMoveEffects(move.Base.Effects, enemyUnit.Enemy, playerUnit.Player, move.Base.Target);

                yield return new WaitForSeconds(1f);

            } else if(move.Base.Effects.NumberOfHits.MinNum != 0) {

                var multiHit = move.Base.Effects.NumberOfHits;
                int numTimes = 0;
                for (int start = multiHit.MinNum; start <= multiHit.MaxNum; start++) 
                {
                    bool isDefeated = playerUnit.Player.TakeDamage(move, enemyUnit.Enemy);
                    numTimes++;

                    yield return playerHud.UpdateHP();

                    yield return new WaitForSeconds(1f);
                    
                    if (isDefeated)
                        break;
                }

                yield return dialogBox.TypeDialog($"{move.Base.Name} hit {numTimes} times!");

                yield return new WaitForSeconds(1f);

            } else {
                bool isFainted = playerUnit.Player.TakeDamage(move, enemyUnit.Enemy);
                yield return playerHud.UpdateHP();
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && playerUnit.Player.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMonsterMoveEffects(secondary, enemyUnit.Enemy, playerUnit.Player, secondary.Target);
                }
            }

            if (playerUnit.Player.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name} fainted!");
                // add player faint animation here

                yield return new WaitForSeconds(2f);
                willDropItems = false;
                BattleOver(false);

            } else { 
                // Statuses like burn or psn will hurt the pokemon after the turn
                enemyUnit.Enemy.OnAfterTurn();
                playerUnit.Player.OnAfterTurn();

                yield return ShowEnemyStatusChanges(enemyUnit.Enemy);
                yield return ShowPlayerStatusChanges(playerUnit.Player);

                yield return playerHud.UpdateHP();
                yield return enemyHud.UpdateHP();

                yield return new WaitForSeconds(2f);

                if(playerUnit.Player.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name} fainted!");
                    // add player faint animation here

                    yield return new WaitForSeconds(2f);
                    willDropItems = false;
                    BattleOver(false);
                } else if (enemyUnit.Enemy.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} was defeated!");
                    // enemy faint animation goes here

                    yield return new WaitForSeconds(2f);
                    willDropItems = true;
                    BattleOver(true);
                }

                PlayerAction(); 
            }
        }
        else 
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name}'s attack missed!");
            PlayerAction();
        }

    }

    IEnumerator RunPlayerMoveEffects(MoveEffects effects, Player source, EnemyBase target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if(moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else 
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // Healing HP
        if (effects.RestoreHP > 0)
        {
            float hp = effects.RestoreHP / 100f;
            source.HP = Mathf.FloorToInt((source.MaxHp * hp) + source.HP);

            if (source.HP > source.MaxHp)
                source.HP = source.MaxHp;

            source.HpChanged = true;
            yield return playerHud.UpdateHP();
            yield return dialogBox.TypeDialog($"{source.Base.Name} restored health!");
            yield return new WaitForSeconds(1f);
        }

        // Healing MP
        if (effects.RestoreMP > 0)
        {
            float mp = effects.RestoreMP / 100f;
            source.MANA = Mathf.FloorToInt((source.MaxMana / mp) + source.MANA);

            if (source.MANA > source.MaxMana)
                source.MANA = source.MaxMana;

            yield return playerHud.UpdateMana();
            yield return dialogBox.TypeDialog($"{source.Base.Name} restored mana!");
            yield return new WaitForSeconds(1f);
        }

        // Healing Status
        if (effects.RemoveAllStatChanges)
        {
            source.CureStatus();
            yield return dialogBox.TypeDialog($"{source.Base.Name} healed the status condition!");
            yield return new WaitForSeconds(1f);
        }

        yield return ShowPlayerStatusChanges(source);
        yield return ShowEnemyStatusChanges(target);
    }

    IEnumerator RunMonsterMoveEffects(MoveEffects effects, EnemyBase source, Player target, MoveTarget moveTarget)
    {
        // Stat Boosting
        if (effects.Boosts != null)
        {
            if(moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        // Healing HP
        if (effects.RestoreHP > 0)
        {
            float hp = effects.RestoreHP / 100f;
            source.HP = Mathf.FloorToInt((source.MaxHp / hp) + source.HP);

            if (source.HP > source.MaxHp)
                source.HP = source.MaxHp;

            source.HpChanged = true;
            yield return enemyHud.UpdateHP();
            yield return dialogBox.TypeDialog($"{source.Base.Name} restored health!");
            yield return new WaitForSeconds(1f);
        }

        // Healing Status
        if (effects.RemoveAllStatusChanges)
        {
            source.CureStatus();
            yield return dialogBox.TypeDialog($"{source.Base.Name} healed the status condition!");
            yield return new WaitForSeconds(1f);
        }

        // Restoring Stat Changes
        if (effects.RemoveAllStatChanges)
        {
            source.ResetStatBoost();
            target.ResetStatBoost();
            yield return dialogBox.TypeDialog($"All stat changes have been reset!");
            yield return new WaitForSeconds(1f);
        }   

        yield return ShowPlayerStatusChanges(target);
        yield return ShowEnemyStatusChanges(source);
    }

    bool CheckIfMonsterMoveHits (MonsterMove move, EnemyBase source, Player target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f};

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    bool CheckIfPlayerSpellHits (PlayerSpell move, Player source, EnemyBase enemy)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f};

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowEnemyStatusChanges(EnemyBase enemy) {
        while(enemy.StatusChanges.Count > 0)
        {
            var message = enemy.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator ShowPlayerStatusChanges(Player player) {
        while(player.StatusChanges.Count > 0)
        {
            var message = player.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    void ChooseFirstTurn()
    {
        if(playerUnit.Player.Speed >= enemyUnit.Enemy.Speed)
            PlayerAction();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;

        OnBattleOver(won);
    }

    void UpdateMoveSelectionUI()
    {
        dialogBox.UpdateSpellSelection(currentMove);
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (enemyUnit.Enemy.Base.IsSpecialBoss)
        {
            yield return dialogBox.TypeDialog($"You can't run away!");

            yield return new WaitForSeconds(1f);

            PlayerAction();
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Player.Speed;
        int enemySpeed = enemyUnit.Enemy.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            willDropItems = false;

            yield return new WaitForSeconds(1f);

            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                willDropItems = false;

                yield return new WaitForSeconds(1f);

                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"You can't run away!");

                yield return new WaitForSeconds(1f);

                PlayerAction();
            }
        }
    }
}
