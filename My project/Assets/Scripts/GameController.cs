using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialogue }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueTrigger dialogueTrigger;
    [SerializeField] Battle battleSystem;
    [SerializeField] HealthSystem healthGlobes;
    [SerializeField] GameObject minimap;
    [SerializeField] Camera worldCamera;

    GameState state;
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        ConditionDB.Init();
    }

    private void Start()
    {
        playerController.onEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStarted += OnDialogueStarted;
            dialogueManager.OnDialogueEnded += OnDialogueEnded;
        }
        else
        {
            Debug.LogError("DialogueTrigger is not assigned in the Inspector of the GameController!");
        }

        playerScripts = playerController.GetComponents<MonoBehaviour>();
        List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>(playerScripts);
        scriptsToDisable.Remove(playerController);
        playerScripts = scriptsToDisable.ToArray();

        state = GameState.FreeRoam;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);

        EnablePlayerControl(false);

        battleSystem.StartBattle();
    }

    void EndBattle(bool won)
    {
        GameObject bossEnemy = GameObject.FindWithTag("BossMonster");
        bossEnemy.SetActive(false);
        
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        healthGlobes.gameObject.SetActive(true);
        healthGlobes.UpdateHealthOutsideOneVOne();
        minimap.SetActive(true);

        EnablePlayerControl(true);
    }

    // Call this method to trigger the dialogue, getting the Ink file from the Dialogue Manager
    public void StartDialogue(string interactionName) // You can use a string to identify the dialogue
    {
        if (state != GameState.FreeRoam) return; // Only start dialogue from free roam
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is not assigned in the Inspector of the GameController!");
            return;
        }

        state = GameState.Dialogue;
        EnablePlayerControl(false);
        worldCamera.gameObject.SetActive(false);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);
        dialogueManager.StartDialogue(interactionName); // Pass the Ink File to the trigger
    }

    void OnDialogueStarted(Ink.Runtime.Story currentStory)
    {
        state = GameState.Dialogue;
        EnablePlayerControl(false);
        worldCamera.gameObject.SetActive(true);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);
    }

    void OnDialogueEnded()
    {
        Debug.Log("GameController: OnDialogueEnded called");
        state = GameState.FreeRoam;
        worldCamera.gameObject.SetActive(true);
        healthGlobes.gameObject.SetActive(true);
        minimap.SetActive(true);
        EnablePlayerControl(true);
        dialogueTrigger.dialoguePanel.SetActive(false); // Hide the dialogue UI
    }

    private void EnablePlayerControl(bool enable)
    {
        foreach (var script in playerScripts)
        {
            script.enabled = enable;
        }
        playerController.enabled = enable;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            // Dialogue UI will handle updates during dialogue
            if (dialogueTrigger != null)
            {
                dialogueTrigger.Update(); // Dialogue Trigger now handles its own updates
            }
        }
    }
}
