using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasToggle : MonoBehaviour
{
    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        // ensure it starts visible
        _canvas.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // toggle the Canvas component on/off
            _canvas.enabled = !_canvas.enabled;
        }
    }
}
