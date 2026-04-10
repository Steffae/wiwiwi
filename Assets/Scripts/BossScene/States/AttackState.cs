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

            if (boss.Agent != null)
            {
                boss.Agent.isStopped = true;
            }

            // Поворачиваемся к игроку
            boss.FacePlayer();

            // Запускаем анимацию атаки
            boss.Animator.SetTrigger("Attack");

            // Запускаем корутину для нанесения урона в нужный момент
            boss.StartCoroutine(PerformAttack(boss));
        }

        private IEnumerator PerformAttack(BossController boss)
        {
            // Ждём момент удара (обычно 30-40% анимации)
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

                // Проверяем, попали ли мы
                HealthComponent playerHealth = boss.Player.GetComponent<HealthComponent>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Boss dealt {damage} damage to player!");
                }
            }

            // Ждём завершения анимации
            yield return new WaitForSeconds(0.8f);

            // Устанавливаем кулдаун
            boss.AttackTimer = boss.Stats.attackCooldown;

            // Возвращаемся в Chase
            if (boss.CurrentHealth > 0)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Update(BossController boss)
        {
            // Поворачиваемся к игроку во время атаки
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