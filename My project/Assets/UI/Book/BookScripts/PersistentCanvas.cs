using UnityEngine;

/// <summary>
/// Makes a Canvas persist across scene loads and prevents duplicates.
/// Attach this to the parent Canvas that contains the EnchantedCodex.
/// </summary>
public class PersistentCanvas : MonoBehaviour
{
    private static PersistentCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            // This is the first instance - make it persistent
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"PersistentCanvas: Made canvas '{gameObject.name}' persistent across scenes");
        }
        else
        {
            // A duplicate - destroy this instance
            Debug.Log($"PersistentCanvas: Destroying duplicate canvas '{gameObject.name}'");
            Destroy(gameObject);
        }
    }
} 