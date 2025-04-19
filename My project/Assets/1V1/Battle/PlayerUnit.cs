using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : MonoBehaviour
{
    public Player Player {get; private set;}

    public void Setup()
    {
        PlayerDB playerDB = FindObjectOfType<PlayerDB>();

        if (playerDB != null)
        {
            Player = playerDB.Player;
            GetComponent<Image>().sprite = Player.Base.Image;
        }
        else
        {
            Debug.LogError("PlayerDB not found in the scene. PlayerUnit cannot be set up.");
            enabled = false;
        }
    }
}
