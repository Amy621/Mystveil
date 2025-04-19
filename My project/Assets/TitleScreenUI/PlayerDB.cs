using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDB : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerStats Base;

    public static PlayerDB Instance { get; private set; }
    public Player Player => player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple PlayerDB instances found! Destroying the extra one.");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // get from DB, if level is 0 -> start of game
        // else take the number and set the player's level
        int level = 5;
        player = new Player(Base, level);

        // if start of the game, just take all the max of the initial stats
        // else take the other values (HP, Exp, Mana, Spells, etc.)
    }

    void Update()
    {
        
    }
}
