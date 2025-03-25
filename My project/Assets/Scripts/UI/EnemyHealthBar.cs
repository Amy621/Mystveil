using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarFill;
    public Image healthBarBackground;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public float smoothSpeed = 5f;
    public string displayName = "Spider";
    
    [Header("Colors")]
    public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color fillColor = Color.red;
    public Color targetedColor = Color.yellow;
    
    [Header("Targeting Settings")]
    public float targetRange = 10f;
    public LayerMask playerLayer;
    
    [Header("Animation Settings")]
    public float damageFlashDuration = 0.1f;
    public Color damageFlashColor = Color.white;
    
    private Camera mainCamera;
    private Enemy enemy;
    private Canvas canvas;
    private static EnemyHealthBar currentTarget;
    private bool isTargeted = false;
    private float damageFlashTimer = 0f;
    private bool isFlashing = false;
    
    private void Start()
    {
        mainCamera = Camera.main;
        enemy = GetComponentInParent<Enemy>();
        canvas = GetComponentInChildren<Canvas>();
        
        // Set up the canvas
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;
        
        // Set up the visual elements
        if (healthBarBackground != null)
        {
            healthBarBackground.color = backgroundColor;
            // Make sure the background is behind the fill
            healthBarBackground.raycastTarget = false;
        }
        
        if (healthBarFill != null)
        {
            healthBarFill.color = fillColor;
            healthBarFill.type = Image.Type.Filled;
            healthBarFill.fillMethod = Image.FillMethod.Horizontal;
            healthBarFill.raycastTarget = false;
        }
            
        if (nameText != null)
        {
            nameText.text = displayName;
            nameText.raycastTarget = false;
        }
    }
    
    private void LateUpdate()
    {
        if (mainCamera == null || enemy == null) return;
        
        // Update health bar fill amount
        if (healthBarFill != null)
        {
            float targetFillAmount = (float)enemy.currentHP / enemy.maxHP;
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{enemy.currentHP}/{enemy.maxHP}";
        }
        
        // Handle damage flash effect
        if (isFlashing)
        {
            damageFlashTimer -= Time.deltaTime;
            if (damageFlashTimer <= 0)
            {
                isFlashing = false;
                if (healthBarFill != null)
                {
                    healthBarFill.color = isTargeted ? targetedColor : fillColor;
                }
            }
        }
        
        // Make the health bar face the camera
        Vector3 cameraDirection = mainCamera.transform.forward;
        canvas.transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
        
        // Update position with smooth movement
        Vector3 targetPosition = enemy.transform.position + offset;
        canvas.transform.position = Vector3.Lerp(canvas.transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    public void OnPlayerAttack()
    {
        // Check if the player is in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetRange, playerLayer);
        bool playerInRange = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerInRange = true;
                break;
            }
        }

        if (playerInRange)
        {
            // If this enemy is already targeted, untarget it
            if (isTargeted)
            {
                isTargeted = false;
                if (currentTarget == this)
                {
                    currentTarget = null;
                }
                // Reset visual feedback
                if (healthBarBackground != null)
                    healthBarBackground.color = backgroundColor;
                if (healthBarFill != null)
                    healthBarFill.color = fillColor;
            }
            else
            {
                // If there was a previous target, untarget it
                if (currentTarget != null)
                {
                    currentTarget.isTargeted = false;
                    if (currentTarget.healthBarBackground != null)
                        currentTarget.healthBarBackground.color = currentTarget.backgroundColor;
                    if (currentTarget.healthBarFill != null)
                        currentTarget.healthBarFill.color = currentTarget.fillColor;
                }

                // Set this as the new target
                isTargeted = true;
                currentTarget = this;
                // Highlight the targeted enemy
                if (healthBarBackground != null)
                    healthBarBackground.color = backgroundColor;
                if (healthBarFill != null)
                    healthBarFill.color = targetedColor;
            }
        }
    }

    public void OnDamageTaken()
    {
        // Trigger damage flash effect
        isFlashing = true;
        damageFlashTimer = damageFlashDuration;
        if (healthBarFill != null)
        {
            healthBarFill.color = damageFlashColor;
        }
    }
} 