using UnityEngine;

public class SpiderProjectile : MonoBehaviour
{
    public float damage = 5f;
    private bool hasHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                hasHit = true;
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
} 