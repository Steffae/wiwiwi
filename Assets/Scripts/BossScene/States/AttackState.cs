using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class AttackState : IBossState
    {
        private bool hasAttacked = false;

        public void Enter(BossController boss)
        {
            hasAttacked = false;

            boss.SafeSetAgentStopped(true);
            boss.FacePlayer();
            boss.Animator.SetTrigger("Attack");

            boss.StartCoroutine(PerformAttack(boss));
        }

        private IEnumerator PerformAttack(BossController boss)
        {
            yield return new WaitForSeconds(0.4f);

            if (!hasAttacked && boss.Player != null)
            {
                hasAttacked = true;

                float distance = boss.DistanceToPlayer();
                float damage = boss.Stats.damage;

                if (boss.IsEnraged)
                {
                    damage *= boss.Stats.enrageDamageMultiplier;
                }

                // Проверяем тип оружия
                if (boss.CurrentWeapon == BossWeaponType.Melee)
                {
                    // Ближний бой
                    if (distance <= boss.Stats.attackRange && boss.CanSeePlayer())
                    {
                        HealthComponent playerHealth = boss.Player.GetComponent<HealthComponent>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            Debug.Log($"Boss dealt {damage} {boss.CurrentElement} melee damage!");
                        }

                        // Применяем эффект стихии
                        boss.ApplyElementEffect(boss.Player.gameObject);

                        // Эффект попадания
                        boss.SpawnHitEffect(boss.MeleeAttackPoint.position);
                    }
                }
                else
                {
                    // Дальний бой - запускаем снаряд
                    boss.LaunchProjectile(damage);
                    Debug.Log($"Boss launched {boss.CurrentElement} projectile!");
                }
            }

            yield return new WaitForSeconds(0.8f);

            boss.AttackTimer = boss.Stats.attackCooldown;

            if (boss.CurrentHealth > 0)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Update(BossController boss)
        {
            boss.FacePlayer();
        }

        public void Exit(BossController boss)
        {
            boss.SafeSetAgentStopped(false);
        }
    }
}