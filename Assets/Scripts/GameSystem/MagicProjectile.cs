using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 25f;
    public float lifetime = 5f;
    public GameObject hitEffect;

    private bool hasHit = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        if (collision.collider.CompareTag("Player") && gameObject.CompareTag("PlayerProjectile"))
            return;
        if (collision.collider.CompareTag("Enemy") && gameObject.CompareTag("EnemyProjectile"))
            return;
        if (collision.collider.CompareTag("EnemyProjectile") || collision.collider.CompareTag("PlayerProjectile"))
            return;

        Health health = collision.collider.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // НЕ уничтожаем птичку
            Debug.Log("Птичка попала, но продолжает существовать");
        }
        else
        {
            Debug.Log("Птичка отскочила от " + collision.collider.name);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (!hasHit && rb != null && rb.linearVelocity.magnitude < 0.1f)
        {
            Destroy(gameObject, 3f);
        }
    }
}