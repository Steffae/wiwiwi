using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float damage = 15f;
    public float lifetime = 10f;
    public GameObject hitEffect;

    private bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Игнорируем столкновения со всеми врагами при запуске
        Collider projectileCollider = GetComponent<Collider>();
        if (projectileCollider != null)
        {
            Collider[] enemyColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);
            foreach (var col in enemyColliders)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    Physics.IgnoreCollision(projectileCollider, col, true);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        // Игнорируем столкновения с врагами
        if (collision.collider.CompareTag("Enemy"))
            return;

        HealthComponent healthComp = collision.collider.GetComponent<HealthComponent>();
        if (healthComp != null)
        {
            healthComp.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}