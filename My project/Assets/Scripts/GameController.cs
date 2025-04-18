using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
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

        playerScripts = playerController.GetComponents<MonoBehaviour>();
        List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>(playerScripts);
        scriptsToDisable.Remove(playerController);
        playerScripts = scriptsToDisable.ToArray();
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        healthGlobes.gameObject.SetActive(false);
        minimap.SetActive(false);

        foreach (var script in playerScripts)
        {
            script.enabled = false;
        }

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
        minimap.SetActive(true);

        foreach (var script in playerScripts)
        {
            script.enabled = true;
        }
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
    }
}
