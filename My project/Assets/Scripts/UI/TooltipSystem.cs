using UnityEngine;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public GameObject tooltipContainer;
    public TMP_Text tooltipText;
    
    void Awake()
    {
        current = this;
        tooltipContainer.SetActive(false);
    }
    
    public static void Show(string text)
    {
        current.tooltipText.text = text;
        current.tooltipContainer.SetActive(true);
        
        // Position the tooltip near the mouse
        current.tooltipContainer.transform.position = Input.mousePosition;
    }
    
    public static void Hide()
    {
        current.tooltipContainer.SetActive(false);
    }
    
    void Update()
    {
        if (tooltipContainer.activeSelf)
        {
            // Update position to follow mouse
            tooltipContainer.transform.position = Input.mousePosition;
        }
    }
} 