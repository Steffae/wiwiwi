using UnityEngine;
using BossController = Game.Boss.BossController;

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

        // Проверяем обычных врагов
        EnemyBase enemy = collision.collider.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
            Debug.Log($"Нанесён урон {damage} рыбой по врагу {collision.collider.name}!");
            return;
        }

        // Проверяем босса
        BossController boss = collision.collider.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            hasHit = true;

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
            Debug.Log($"Нанесён урон {damage} рыбой по БОССУ!");
            return;
        }

        // Попадание в стену/землю - всё равно уничтожаем
        hasHit = true;
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}