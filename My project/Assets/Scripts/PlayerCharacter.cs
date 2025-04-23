using UnityEngine;

/// <summary>
/// Component that provides access to player stats and character-related functionality
/// </summary>
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private string playerName;

    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
} 