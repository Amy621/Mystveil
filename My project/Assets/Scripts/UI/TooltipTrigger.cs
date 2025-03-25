using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
} 