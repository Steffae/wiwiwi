using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class HeavyAttackState : IBossState
    {
        private bool hasAttacked = false;

        public void Enter(BossController boss)
        {
            hasAttacked = false;

            boss.SafeSetAgentStopped(true);
            boss.FacePlayer();
            boss.Animator.SetTrigger("HeavyAttack");

            boss.StartCoroutine(PerformHeavyAttack(boss));
        }

        private IEnumerator PerformHeavyAttack(BossController boss)
        {
            yield return new WaitForSeconds(0.8f);

            if (!hasAttacked && boss.Player != null)
            {
                hasAttacked = true;

                float distance = boss.DistanceToPlayer();
                float damage = boss.Stats.damage * boss.Stats.heavyDamageMultiplier;

                if (boss.IsEnraged)
                {
                    damage *= boss.Stats.enrageDamageMultiplier;
                }

                if (boss.CurrentWeapon == BossWeaponType.Melee)
                {
                    if (distance <= boss.Stats.heavyAttackRange && boss.CanSeePlayer())
                    {
                        HealthComponent playerHealth = boss.Player.GetComponent<HealthComponent>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            Debug.Log($"Boss HEAVY attack: {damage} {boss.CurrentElement} damage!");
                        }

                        boss.ApplyElementEffect(boss.Player.gameObject);
                        boss.SpawnHitEffect(boss.MeleeAttackPoint.position);
                    }
                }
                else
                {
                    // Для дальнего боя - усиленный снаряд
                    boss.LaunchProjectile(damage);
                    Debug.Log($"Boss launched HEAVY {boss.CurrentElement} projectile!");
                }
            }

            yield return new WaitForSeconds(0.7f);

            boss.AttackTimer = boss.Stats.heavyAttackCooldown;

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