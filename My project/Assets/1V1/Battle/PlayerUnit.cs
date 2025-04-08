using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] PlayerStats _base;
    [SerializeField] int level;

    public Player Player {get; set;}

    public void Setup()
    {
        Player = new Player(_base, level);
        GetComponent<Image>().sprite = Player.Base.Image;
    }
}
