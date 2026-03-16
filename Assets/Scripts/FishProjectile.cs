using UnityEngine;

public class FishProjectile : MonoBehaviour
{
    public float damage = 20f;
    public float speed = 15f;
    public float lifetime = 20f;
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

        // Игнорируем игрока и его снаряды
        if (collision.collider.CompareTag("Player"))
            return;
        if (collision.collider.CompareTag("PlayerProjectile"))
            return;

        Health health = collision.collider.GetComponent<Health>();
        if (health != null && collision.collider.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Debug.Log("Рыбка попала во врага!");
        }
        else
        {
            Debug.Log("Рыбка отскочила от " + collision.collider.name);
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