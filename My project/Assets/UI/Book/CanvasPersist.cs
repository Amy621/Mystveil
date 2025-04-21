using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PersistCanvas : MonoBehaviour
{
    private static PersistCanvas _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // Prevent this GameObject (and its children) from being destroyed on scene load
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
        }
    }
}
