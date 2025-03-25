using UnityEngine;
using UnityEngine.UI;

public class PageTurnEffect : MonoBehaviour
{
    public RectTransform pagePrefab;
    public float turnDuration = 1f;
    private RectTransform currentTurningPage;
    private float turnTimer;
    private bool isTurning;
    private bool isForward;
    
    public void StartPageTurn(bool forward)
    {
        if (isTurning) return;
        
        isForward = forward;
        isTurning = true;
        turnTimer = 0;
        
        // Create new page for animation
        currentTurningPage = Instantiate(pagePrefab, transform);
        currentTurningPage.SetAsLastSibling();
        
        // Set initial rotation
        float startRotation = forward ? 0 : 180;
        currentTurningPage.rotation = Quaternion.Euler(0, startRotation, 0);
    }
    
    void Update()
    {
        if (!isTurning) return;
        
        turnTimer += Time.deltaTime;
        float progress = turnTimer / turnDuration;
        
        if (progress >= 1)
        {
            // Finish turn
            Destroy(currentTurningPage.gameObject);
            isTurning = false;
            return;
        }
        
        // Animate page turn
        float targetRotation = isForward ? 180 : 0;
        float currentRotation = Mathf.Lerp(isForward ? 0 : 180, targetRotation, progress);
        currentTurningPage.rotation = Quaternion.Euler(0, currentRotation, 0);
        
        // Add page curl effect
        float curl = Mathf.Sin(progress * Mathf.PI) * 10;
        currentTurningPage.GetComponent<Image>().material.SetFloat("_Curl", curl);
    }
} 