using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
public class Recipe : ScriptableObject
{
    public Item ingredient1;
    public Item ingredient2;
    public Item ingredient3;

    public int amount1 = 1;
    public int amount2 = 1;
    public int amount3;

}
