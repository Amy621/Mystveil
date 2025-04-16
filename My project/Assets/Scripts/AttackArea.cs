using UnityEngine;
using UnityEngine.UI;

public class AttackArea : MonoBehaviour
{
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

    private bool isAbility2Visible = false;
    private bool isRotatingAbility2 = false;
    private Vector3 lastMousePosition;

    void Start()
    {
        Ability1Canvas.enabled = false;
        Ability1Skillshot.enabled = false;

        Ability2Canvas.enabled = false;
        Ability2Skillshot.enabled = false;
    }

    void Update()
{
    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    // U to trigger and toggle the circle
    if (Input.GetKeyDown(KeyCode.U))
    {
        isAimingAbility1 = !isAimingAbility1;
        Ability1Canvas.enabled = isAimingAbility1;
        Ability1Skillshot.enabled = isAimingAbility1;
        Cursor.visible = !isAimingAbility1;
    }

    // Ability 1 casting
    if (Input.GetMouseButtonDown(0) && isAimingAbility1)
    {
        isAimingAbility1 = false;
        Ability1Canvas.enabled = false;
        Ability1Skillshot.enabled = false;
        Cursor.visible = true;

        CastAbilityAt(targetPosition);
    }

    // Ability 2 activate and begin rotation on left click (when not aiming)
    if (Input.GetMouseButtonDown(0) && !isAimingAbility1)
    {
        isAbility2Visible = true;
        isRotatingAbility2 = true;

        Ability2Canvas.enabled = true;
        Ability2Skillshot.enabled = true;

        lastMousePosition = Input.mousePosition;

        // Cast once on activate
        CastAbilityAt(Ability2Canvas.transform.position);
    }

    // Rotate while dragging
    if (isAbility2Visible && isRotatingAbility2)
    {
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        float rotationSpeed = 0.2f;

        Ability2Canvas.transform.Rotate(Vector3.up, mouseDelta.x * rotationSpeed, Space.World);

        lastMousePosition = Input.mousePosition;
    }

    // On release, stop rotation and hide
    if (Input.GetMouseButtonUp(0) && isAbility2Visible)
    {
        isRotatingAbility2 = false;
        isAbility2Visible = false;

        Ability2Canvas.enabled = false;
        Ability2Skillshot.enabled = false;
    }

    // Ability 1 targeting update
    if (isAimingAbility1)
    {
        UpdateAbilityCanvasPosition();
    }
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
        if (Physics.Raycast(ray, out RaycastHit enemyHit, maxAbilityDistance, enemyLayer))
        {
            Enemy enemy = enemyHit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Destroy(enemy.gameObject);

                if (audioSource && attackSound)
                    audioSource.PlayOneShot(attackSound);

                return;
            }
        }

        Collider[] enemies = Physics.OverlapSphere(center, abilityRadius, enemyLayer);
        foreach (Collider col in enemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Destroy(enemy.gameObject);
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
