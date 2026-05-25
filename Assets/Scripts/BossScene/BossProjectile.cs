using UnityEngine;

namespace Game.Boss
{
    public class BossProjectile : MonoBehaviour
    {
        private float damage;
        private BossElementType element;
        private BossCombat combat;
        private BossController boss;
        private bool isHeavy;
        private bool hasHit = false;

        public void Initialize(float dmg, BossElementType elem, BossCombat cmbt, BossController bss, bool heavy = false)
        {
            damage = dmg;
            element = elem;
            combat = cmbt;
            boss = bss;
            isHeavy = heavy;

            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            if (other.CompareTag("Boss") || other.CompareTag("Enemy")) return;

            if (other.CompareTag("Player"))
            {
                hasHit = true;

                HealthComponent health = other.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Debug.Log($"Projectile dealt {damage} damage!");
                }

                if (boss != null)
                {
                    boss.HasBeenAttackedByPlayer = true;
                }

                // Применяем эффект стихии при попадании
                BossElementBase elementComponent = combat?.GetCurrentElementComponent();
                if (elementComponent != null)
                {
                    if (isHeavy)
                    {
                        elementComponent.ApplyHeavyRangedEffect(other.gameObject);
                    }
                    else
                    {
                        elementComponent.ApplyRangedEffect(other.gameObject);
                    }
                }

                Destroy(gameObject);
            }
        }
    }
}