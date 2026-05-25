using UnityEngine;
using BossController = Game.Boss.BossController;

public class MeleeWeapon : MonoBehaviour
{
    public float damage = 15f;
    public float attackDuration = 0.3f;

    private bool isAttacking = false;
    private Collider weaponCollider;

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    public void StartAttack()
    {
        if (weaponCollider != null && !isAttacking)
        {
            isAttacking = true;
            weaponCollider.enabled = true;
            Invoke(nameof(StopAttack), attackDuration);
        }
    }

    void StopAttack()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            isAttacking = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        // Проверяем обычных врагов (через EnemyBase)
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Нанесён урон {damage} посохом по врагу {other.name}!");
            return;
        }

        // Проверяем босса
        BossController boss = other.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Debug.Log($"Нанесён урон {damage} посохом по БОССУ!");
        }
    }
}