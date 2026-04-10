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

            if (boss.Agent != null)
            {
                boss.Agent.isStopped = true;
            }

            boss.FacePlayer();

            // Можно использовать другой триггер или параметр
            boss.Animator.SetTrigger("HeavyAttack");

            boss.StartCoroutine(PerformHeavyAttack(boss));
        }

        private IEnumerator PerformHeavyAttack(BossController boss)
        {
            // Тяжёлая атака имеет более долгую подготовку
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

                // Увеличенный радиус для тяжёлой атаки
                HealthComponent playerHealth = boss.Player.GetComponent<HealthComponent>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Boss dealt {damage} damage to player!");
                }
            }

            yield return new WaitForSeconds(0.7f);

            // Долгий кулдаун для тяжёлой атаки
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
            if (boss.Agent != null)
            {
                boss.Agent.isStopped = false;
            }
        }
    }
}