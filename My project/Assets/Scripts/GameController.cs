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
    // [SerializeField] Inventory inventory;
    // [SerializeField] GameObject hotbarSlots;
    // [SerializeField] GameController Instance;

    GameState state;
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        ConditionDB.Init();

        // if (Instance == null)
        // {
        //     Instance = this;
        // }
    }

    // public GameState getState {
    //     get { return state; }
    // }

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
        // inventory = FindObjectOfType<Inventory>();
        // Debug.Log("Start -- Is inventory here: " + inventory);
        List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>(playerScripts);
        scriptsToDisable.Remove(playerController);
        playerScripts = scriptsToDisable.ToArray();
        // hotbarSlots = GameObject.FindGameObjectWithTag("Hotbar");

        // EnableInventoryControl(true);

        state = GameState.FreeRoam;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);

        Debug.Log("start battle state: " + state);
        // Inventory.Singleton.ChangeGameState();

        // hotbarSlots.SetActive(false);
        // EnableInventoryControl(false);
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
        // hotbarSlots.SetActive(true);

        // inventory.ChangeGameState();

        // EnableInventoryControl(true);

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
        // EnableInventoryControl(false);
        EnablePlayerControl(false);
        worldCamera.gameObject.SetActive(false);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);
        // hotbarSlots.SetActive(false);
        dialogueManager.StartDialogue(interactionName); // Pass the Ink File to the trigger

        // Debug.Log("Start Dialogue -- Is inventory here: " + inventory);
    }

    void OnDialogueStarted(Ink.Runtime.Story currentStory)
    {
        state = GameState.Dialogue;
        // EnableInventoryControl(false);
        EnablePlayerControl(false);
        worldCamera.gameObject.SetActive(true);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);
        // hotbarSlots.SetActive(false);

        // Debug.Log("On Dialogue Started -- Is inventory here: " + inventory);
    }

    void OnDialogueEnded()
    {
        Debug.Log("GameController: OnDialogueEnded called");
        state = GameState.FreeRoam;
        worldCamera.gameObject.SetActive(true);
        healthGlobes.gameObject.SetActive(true);
        minimap.SetActive(true);
        // EnableInventoryControl(true);
        EnablePlayerControl(true);
        dialogueTrigger.dialoguePanel.SetActive(false); // Hide the dialogue UI
        // hotbarSlots.SetActive(true);

        // Debug.Log("On Dialogue Ended -- Is inventory here: " + inventory);
    }

    private void EnableInventoryControl(bool enable)
    {
        //inventory.obj.SetActive(enable);
        // inventory.isActive = enable;
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
            // inventory.HandleUpdate();
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
