using UnityEngine;

public class FishProjectile : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 20f;
    public GameObject hitEffect;
    public float speed = 15f;

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

        // Проверяем, не враг ли это
        EnemyBase enemy = collision.collider.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}