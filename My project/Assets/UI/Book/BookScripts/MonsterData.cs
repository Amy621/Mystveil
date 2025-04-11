using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public string monsterName;
    public Sprite monsterIcon;
    public Sprite monsterDetailImage;
    public string description;
    public bool isDiscovered = false;
    
    // Stats
    public int health;
    public int damage;
    public string habitat;
    public string behavior;
    public string weaknesses;
} 