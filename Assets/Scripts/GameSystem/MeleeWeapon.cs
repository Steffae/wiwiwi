using UnityEngine;

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
        if (isAttacking)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Нанесён урон посохом по врагу!");
            }
        }
    }
}