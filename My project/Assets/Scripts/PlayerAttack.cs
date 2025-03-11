using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackArea;
    private bool attacking;
    private float timeToAttack = 0.25f;
    private float timer = 0f;
    public int damage = 25; // Amount of damage to deal

void Start()
{
    if (!attackArea)
    {
        // Try to find a child named "AttackArea"
        Transform child = transform.Find("AttackArea");
        if (child) 
        {
            attackArea = child.gameObject;
        }
        // If none found, create one with a trigger collider
        if (!attackArea)
        {
            attackArea = new GameObject("AttackArea");
            attackArea.transform.SetParent(transform);
            var collider = attackArea.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 1.5f;
            attackArea.SetActive(false);
        }
    }
}


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(false);
            }
        }
    }

    private void Attack()
    {
        attacking = true;
        attackArea.SetActive(true);
    }

    // Make sure "attackArea" has a trigger collider and the enemy is tagged "Enemy".
    void OnTriggerEnter(Collider other)
    {
        if (attacking && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
