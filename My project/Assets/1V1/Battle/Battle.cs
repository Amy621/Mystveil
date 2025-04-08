using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, BattlePhase }

public class Battle : MonoBehaviour
{
    [SerializeField] PlayerUnit playerUnit;
    [SerializeField] PlayerHud playerHud;
    [SerializeField] EnemyUnit enemyUnit;
    [SerializeField] EnemyHud enemyHud;
    [SerializeField] BattleDialogueBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;

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

        PlayerAction();
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

        var spell = playerUnit.Player.Spells[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name} used {spell.Base.Name}!");

        yield return new WaitForSeconds(1f);

        bool isDefeated = enemyUnit.Enemy.TakeDamage(spell, playerUnit.Player);
        yield return enemyHud.UpdateHP();
        yield return playerHud.UpdateMana();

        if (isDefeated)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} was defeated!");
        } else { StartCoroutine(EnemyMove()); }
    }

    private void Update()
    {
        // if(state == BattleState.PlayerAction) { HandleActionSelection(); }
        // else if (state == BattleState.PlayerMove) { HandleMoveSelection(); }
    }

    void HandleActionSelectionClick(int actionIndex)
    {
        // if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        // {
        //     if(currentAction < 2)
        //         ++currentAction;
        // }
        // else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        // {
        //     if(currentAction > 0)
        //         --currentAction;
        // }

        // dialogBox.UpdateActionSelection(currentAction);

        // if(Input.GetKeyDown(KeyCode.Return))
        // {
        //     if(currentAction == 0)
        //     {
        //         // Attack
        //         PlayerMove();
        //     } else if (currentAction == 1) {
        //         // Item
        //     } else {
        //         // Run
        //     }
        // }

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
                Debug.Log("Run selected (not implemented)");
            }
        }
    }

    void HandleMoveHover(int moveIndex)
    {
        // if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        // {
        //     if(currentMove < playerUnit.Player.Spells.Count - 1)
        //         ++currentMove;
        // }
        // else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        // {
        //     if(currentMove > 0)
        //         --currentMove;
        // }

        // dialogBox.UpdateSpellSelection(currentMove);

        // if(Input.GetKeyDown(KeyCode.Return))
        // {
        //     var spell = playerUnit.Player.Spells[currentMove];
        //     // not enough mana to cast
        //     if (spell.MP > playerUnit.Player.Base.MANA)
        //     {
        //         Debug.Log("Not enough mana!");
        //         dialogBox.EnableMoveSelector(false);
        //         dialogBox.EnableDialogText(true);
        //         StartCoroutine(ErrorMovePopUp());
        //     } else {
        //         dialogBox.EnableMoveSelector(false);
        //         dialogBox.EnableDialogText(true);
        //         StartCoroutine(PerformPlayerMove());
        //     }
        // }

        if (state == BattleState.PlayerMove)
        {
            currentMove = moveIndex;
            dialogBox.UpdateSpellSelection(currentMove);
        }
    }

    void HandleMoveSelectionClick(int moveIndex)
    {
        if (state == BattleState.PlayerMove)
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

        PlayerMove();
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Enemy.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} used {move.Base.Name}!");

        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Player.TakeDamage(move, enemyUnit.Enemy);
        yield return playerHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Player.Base.Name} Fainted!");
        } else { PlayerAction(); }
    }

    void UpdateMoveSelectionUI()
    {
        dialogBox.UpdateSpellSelection(currentMove);
    }
}
