using UnityEngine;

/// <summary>
/// Adapter component that connects the player's attributes to the save system
/// </summary>
public class PlayerAttributesAdapter : MonoBehaviour
{
    [SerializeField] private int charisma = 5; // Default charisma value
    
    public int Charisma
    {
        get { return charisma; }
        set 
        { 
            charisma = value;
            OnCharismaChanged?.Invoke(charisma);
        }
    }
    
    // Events for other components to listen to
    public delegate void AttributeEvent(int value);
    public event AttributeEvent OnCharismaChanged;
    
    public void IncreaseCharisma(int amount)
    {
        Charisma += amount;
    }
    
    public void DecreaseCharisma(int amount)
    {
        Charisma = Mathf.Max(0, Charisma - amount);
    }
} 