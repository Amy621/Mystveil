// using UnityEngine;
// using UnityEngine.UI;

// /**
//     For now the attack is like this:
//         whatever the player has in the 1st slot of the 1v1 -> CONE
//         whatever the player has in the 2nd slot of the 1v1 -> CIRCLE
// **/
// public class AttackArea : MonoBehaviour
// {
//     public Player player { get; private set; }

//     public HealthSystem healthSystem { get; private set; }

//     [Header("UI Elements")]
//     public Canvas Ability1Canvas; // circle
//     public Image Ability1Skillshot;

//     public Canvas Ability2Canvas; // cone
//     public Image Ability2Skillshot;

//     [Header("Ability Settings")]
//     public float maxAbilityDistance = 7f;
//     public float ability1Radius = 3f; // Radius for the circle ability
//     public float ability2Angle = 60f; // Angle of the cone in degrees
//     public float ability2Range = 5f; // Range of the cone
//     public LayerMask enemyLayer;

//     [Header("Audio")]
//     public AudioSource audioSource;
//     public AudioClip attackSound;

//     private Ray ray;
//     private RaycastHit hit;
//     private Vector3 ability1TargetPosition;
//     private bool isAimingAbility1 = false;

//     private bool isShowingAbility2 = false;
//     private bool isRotatingAbility2 = false;
//     private Vector3 lastMousePosition;

//     void Start()
//     {
//         Ability1Canvas.enabled = false;
//         Ability1Skillshot.enabled = false;

//         Ability2Canvas.enabled = false;
//         Ability2Skillshot.enabled = false;

//         Ability2Canvas.transform.Rotate(0, 180f, 0);

//         // get player DB stuff
//         PlayerDB playerDB = FindObjectOfType<PlayerDB>();

//         if (playerDB != null)
//         {
//             player = playerDB.Player;
//         }
//         else
//         {
//             Debug.LogError("PlayerDB not found in the scene. PlayerUnit cannot be set up.");
//             enabled = false;
//         }

//         // find health system and attach to use mana system
//         healthSystem = FindObjectOfType<HealthSystem>();

//         // Debug.Log("Getting Cone spell: " + player.Spells[0].Base.Name + " POW: " + player.Spells[0].Base.Power);
//         // Debug.Log("Getting Circle spell: " + player.Spells[1].Base.Name + " POW: " + player.Spells[1].Base.Power);
//     }

//     void Update()
//     {
//         ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//         // Ability 1 (Circle) - Right Click to Show/Hide, Right Click to Activate
//         if (Input.GetMouseButtonDown(1)) // Right mouse button down
//         {
//             if (!isAimingAbility1)
//             {
//                 isAimingAbility1 = true;
//                 Ability1Canvas.enabled = true;
//                 Ability1Skillshot.enabled = true;
//                 Cursor.visible = false;
//                 UpdateAbility1CanvasPosition(); // Update position immediately on show
//             }
//             else
//             {
//                 // Activate ability 1
//                 isAimingAbility1 = false;
//                 Ability1Canvas.enabled = false;
//                 Ability1Skillshot.enabled = false;
//                 Cursor.visible = true;
//                 CastAbility1(ability1TargetPosition);
//             }
//         }

//         // Ability 1 targeting update while visible
//         if (isAimingAbility1)
//         {
//             UpdateAbility1CanvasPosition();
//         }

//         // Ability 2 (Cone) - Left Click Hold to Show and Rotate, Left Click Release to Activate
//         if (Input.GetMouseButtonDown(0) && !isAimingAbility1)
//         {
//             isShowingAbility2 = true;
//             isRotatingAbility2 = true;
//             Ability2Canvas.enabled = true;
//             Ability2Skillshot.enabled = true;
//             lastMousePosition = Input.mousePosition;
//             UpdateAbility2PositionAndRotation(); // Initial positioning
//         }

//         // Rotate Ability 2 while holding left click
//         if (isShowingAbility2 && isRotatingAbility2 && Input.GetMouseButton(0))
//         {
//             RotateAbility2();
//         }

//         // Activate Ability 2 on left click release
//         if (Input.GetMouseButtonUp(0) && isShowingAbility2)
//         {
//             isRotatingAbility2 = false;
//             isShowingAbility2 = false;
//             Ability2Canvas.enabled = false;
//             Ability2Skillshot.enabled = false;
//             CastAbility2();
//         }
//     }

//     void UpdateAbility1CanvasPosition()
//     {
//         int layerMask = ~LayerMask.GetMask("Player");
//         if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
//         {
//             Vector3 direction = (hit.point - transform.position).normalized;
//             float distance = Mathf.Min(Vector3.Distance(transform.position, hit.point), maxAbilityDistance);
//             ability1TargetPosition = transform.position + direction * distance;
//             Ability1Canvas.transform.position = ability1TargetPosition;
//         }
//     }

//     void UpdateAbility2PositionAndRotation()
//     {
//         // Position the cone at the player's position
//         Ability2Canvas.transform.position = transform.position;
//         // Rotate the cone to face the player's forward direction
//         Ability2Canvas.transform.rotation = transform.rotation;
//     }

//     void RotateAbility2()
//     {
//         Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
//         float rotationSpeed = 0.2f;

//         // Calculate rotation based on horizontal mouse movement
//         float rotationAmount = mouseDelta.x * rotationSpeed;

//         // Rotate the Ability2Canvas around the player's up axis (local Y)
//         Ability2Canvas.transform.Rotate(Vector3.up, mouseDelta.x * rotationSpeed, Space.World);

//         lastMousePosition = Input.mousePosition;
//     }

//     void CastAbility1(Vector3 center)
//     {
//         Debug.Log("Casting Circle Ability at: " + center);

//         if (healthSystem.manaPoint < 1)
//         {
//             Debug.Log("No mana left, cannot cast spell!");
//         }
//         else
//         {
//             Collider[] hitColliders = Physics.OverlapSphere(center, ability1Radius, enemyLayer);
//             foreach (Collider hitCollider in hitColliders)
//             {
//                 Enemy enemy = hitCollider.GetComponent<Enemy>();
//                 if (enemy != null)
//                 {
//                     healthSystem.UseMana((float) player.Spells[1].Base.ManaPoints);
//                     Debug.Log("Hit Enemy: " + enemy.name);
//                     enemy.TakeDamage(player.Spells[1]);
//                     // Destroy(enemy.gameObject); // Consider if you want to destroy immediately
//                 }
//             }
//             PlayAttackSound();
//         }
//     }

//     void CastAbility2()
//     {
//         Debug.Log("Casting Cone Ability");
//         // Get the forward direction of the cone UI
//         Vector3 forwardDirection = Ability2Canvas.transform.forward;
//         Vector3 origin = transform.position;

//         // Perform a sphere cast to find potential targets within the cone's range
//         RaycastHit[] hits = Physics.SphereCastAll(origin, 0.5f, forwardDirection, ability2Range, enemyLayer);

//         foreach (RaycastHit hit in hits)
//         {
//             Vector3 directionToTarget = (hit.point - origin).normalized;
//             float angleToTarget = Vector3.Angle(forwardDirection, directionToTarget);

//             if (angleToTarget <= ability2Angle / 2f)
//             {
//                 if (healthSystem.manaPoint < 1)
//                 {
//                     Debug.Log("No mana left, cannot cast spell!");
//                 }
//                 else
//                 {
//                     Enemy enemy = hit.collider.GetComponent<Enemy>();
//                     if (enemy != null)
//                     {
//                         healthSystem.UseMana((float) player.Spells[0].Base.ManaPoints);
//                         Debug.Log("Hit Enemy with Cone: " + enemy.name);
//                         enemy.TakeDamage(player.Spells[0]);
//                         // Destroy(enemy.gameObject); // Consider if you want to destroy immediately
//                     }
//                 }
//             }
//         }
//         PlayAttackSound();
//     }

//     void PlayAttackSound()
//     {
//         if (audioSource && attackSound)
//         {
//             audioSource.PlayOneShot(attackSound);
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         // Draw gizmo for the circle ability
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(ability1TargetPosition, ability1Radius);

//         // Draw gizmo for the cone ability (approximate)
//         Gizmos.color = Color.cyan;
//         Vector3 forward = Ability2Canvas ? Ability2Canvas.transform.forward : transform.forward;
//         Quaternion leftRayRotation = Quaternion.AngleAxis(-ability2Angle / 2, Vector3.up);
//         Quaternion rightRayRotation = Quaternion.AngleAxis(ability2Angle / 2, Vector3.up);
//         Vector3 leftRayDirection = leftRayRotation * forward * ability2Range;
//         Vector3 rightRayDirection = rightRayRotation * forward * ability2Range;
//         Vector3 coneOrigin = Ability2Canvas ? Ability2Canvas.transform.position : transform.position;

//         Gizmos.DrawRay(coneOrigin, forward * ability2Range);
//         Gizmos.DrawRay(coneOrigin, leftRayDirection);
//         Gizmos.DrawRay(coneOrigin, rightRayDirection);
//         Gizmos.DrawWireSphere(coneOrigin + forward * ability2Range, 0.2f); // Indicate the end of the cone
//     }
// }


/**
    For now the attack is like this:
        whatever the player has in the 1st slot of the 1v1 -> CONE
        whatever the player has in the 2nd slot of the 1v1 -> CIRCLE
**/

using UnityEngine;
using UnityEngine.UI;

public class AttackArea : MonoBehaviour
{
    public Player player { get; private set; }
    public HealthSystem healthSystem { get; private set; }

     
    [Header("VFX")]
    public GameObject ability1VFXPrefab;


    [Header("UI Elements")]
    public Canvas Ability1Canvas; // circle
    public Image Ability1Skillshot;

    public Canvas Ability2Canvas; // cone
    public Image Ability2Skillshot;

    [Header("Ability Settings")]
    public float maxAbilityDistance = 7f;
    public float abilityRadius = 3f;
    public int attackDamage = 20;
    public LayerMask enemyLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    private Ray ray;
    private RaycastHit hit;
    private Vector3 targetPosition;
    private bool isAimingAbility1 = false;

    private bool isAbility2Active = false;
    private Vector3 ability2Direction;

    void Start()
    {
        Ability1Canvas.enabled = false;
        Ability1Skillshot.enabled = false;

        Ability2Canvas.enabled = false;
        Ability2Skillshot.enabled = false;

        // get player DB stuff
        PlayerDB playerDB = FindObjectOfType<PlayerDB>();

        if (playerDB != null)
        {
            player = playerDB.Player;
        }
        else
        {
            Debug.LogError("PlayerDB not found in the scene. PlayerUnit cannot be set up.");
            enabled = false;
        }

        // find health system and attach to use mana system
        healthSystem = FindObjectOfType<HealthSystem>();
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Ability 1 casting
        if (Input.GetMouseButtonDown(1))
        {

            if (isAimingAbility1)
            {
                isAimingAbility1 = false;
                Ability1Canvas.enabled = false;
                Ability1Skillshot.enabled = false;
                Cursor.visible = true;

                CastAbilityAt(targetPosition);

                if (ability1VFXPrefab)
                {
                    Vector3 spawnPos = targetPosition;
                    spawnPos.y += 1f; 
                    Instantiate(ability1VFXPrefab, spawnPos, Quaternion.identity);
                }
            }

            else
            {
                isAimingAbility1 = !isAimingAbility1;
                Ability1Canvas.enabled = isAimingAbility1;
                Ability1Skillshot.enabled = isAimingAbility1;
                Cursor.visible = !isAimingAbility1;
            }
        }

        // Ability 2 activation (left click when not aiming Ability 1)
        if (Input.GetMouseButtonDown(0) && !isAimingAbility1 && !isAbility2Active)
        {
            isAbility2Active = true;
            UpdateAbility2Direction();
            
            // Show the ability indicator
            Ability2Canvas.enabled = true;
            Ability2Skillshot.enabled = true;
        }

        // Ability 2 rotation while active
        if (isAbility2Active)
        {
            UpdateAbility2Direction();

            // Cast on mouse release
            if (Input.GetMouseButtonUp(0))
            {
                isAbility2Active = false;
                Ability2Canvas.enabled = false;
                Ability2Skillshot.enabled = false;
                
                CastAbility2();
            }
        }

        // Ability 1 targeting update
        if (isAimingAbility1)
        {
            UpdateAbilityCanvasPosition();
        }
    }

    void UpdateAbility2Direction()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Player")))
        {
            // Calculate direction from player to mouse position
            Vector3 direction = hit.point - transform.position;
            direction.y = 0; // Keep it horizontal
            
            // Position the canvas slightly above ground
            Vector3 canvasPos = transform.position;
            canvasPos.y += 1.1f;
            Ability2Canvas.transform.position = canvasPos;
            
            // Rotate to face the direction
            if (direction != Vector3.zero)
            {
                Ability2Canvas.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void CastAbility2()
    {
        // Get the forward direction of the ability cone
        Vector3 castDirection = Ability2Canvas.transform.forward;
        Vector3 castOrigin = transform.position;
        castOrigin.y += 1f; // Adjust height if needed

        // Visualize the cone direction
        Debug.DrawRay(castOrigin, castDirection * maxAbilityDistance, Color.red, 1f);

        // Damage enemies in cone
        Collider[] enemies = Physics.OverlapSphere(castOrigin, maxAbilityDistance, enemyLayer);
        foreach (Collider col in enemies)
        {
            Vector3 toEnemy = (col.transform.position - castOrigin).normalized;
            float angle = Vector3.Angle(castDirection, toEnemy);
            
            // If enemy is within ~60 degree cone (adjust as needed)
            if (angle < 60f)
            {
                if (healthSystem.manaPoint < 1)
                {
                    Debug.Log("No mana left, cannot cast spell!");
                }
                else
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        healthSystem.UseMana((float) player.Spells[0].Base.ManaPoints);
                        Debug.Log("Hit Enemy with Cone: " + enemy.name);
                        enemy.TakeDamage(player.Spells[0]);
                        // Destroy(enemy.gameObject); // Consider if you want to destroy immediately
                    }
                }
            }
        }

        if (audioSource && attackSound)
            audioSource.PlayOneShot(attackSound);
    }

    void UpdateAbilityCanvasPosition()
    {
        int layerMask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 direction = (hit.point - transform.position).normalized;
            float distance = Mathf.Min(Vector3.Distance(transform.position, hit.point), maxAbilityDistance);
            targetPosition = transform.position + direction * distance;

            Ability1Canvas.transform.position = targetPosition;
        }
    }

    void CastAbilityAt(Vector3 center)
    {
        Collider[] enemies = Physics.OverlapSphere(center, abilityRadius, enemyLayer);
        foreach (Collider col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    healthSystem.UseMana((float) player.Spells[1].Base.ManaPoints);
                    Debug.Log("Hit Enemy: " + enemy.name);
                    enemy.TakeDamage(player.Spells[1]);
                    // Destroy(enemy.gameObject); // Consider if you want to destroy immediately
                }
        }

        if (audioSource && attackSound)
            audioSource.PlayOneShot(attackSound);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, abilityRadius);
    }
}