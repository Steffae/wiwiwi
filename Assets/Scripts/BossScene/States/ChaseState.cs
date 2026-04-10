using UnityEngine;

namespace Game.Boss
{
    public class ChaseState : IBossState
    {
        public void Enter(BossController boss)
        {
            if (boss.Agent != null)
            {
                boss.Agent.isStopped = false;

                // Увеличиваем скорость в зависимости от режима
                float speed = boss.IsEnraged ?
                    boss.Stats.runSpeed * boss.Stats.enrageSpeedMultiplier :
                    boss.Stats.runSpeed;

                boss.Agent.speed = speed;
            }
        }

        public void Update(BossController boss)
        {
            if (boss.Player == null) return;

            // Обновляем цель для NavMesh
            if (boss.Agent != null && boss.Agent.isActiveAndEnabled)
            {
                boss.Agent.SetDestination(boss.Player.position);
            }

            float distance = boss.DistanceToPlayer();

            // Проверка на переход в атаку
            if (distance <= boss.Stats.attackRange && boss.CanSeePlayer())
            {
                // Выбираем тип атаки
                if (boss.AttackTimer <= 0)
                {
                    // 30% шанс на сильную атаку
                    if (Random.value < 0.3f && !boss.IsPeacefulMode)
                    {
                        boss.TransitionToState(BossState.HeavyAttack);
                    }
                    else
                    {
                        boss.TransitionToState(BossState.Attack);
                    }
                    return;
                }
            }

            // Проверка на бегство (только в мирном режиме)
            if (boss.IsPeacefulMode)
            {
                float healthPercent = boss.CurrentHealth / boss.Stats.maxHealth;
                if (healthPercent <= boss.Stats.fleeHealthThreshold)
                {
                    boss.TransitionToState(BossState.Flee);
                    return;
                }
            }

            // Если игрок далеко и мы не видим его - возвращаемся в Idle
            if (distance > boss.Stats.attackRange * 5f && !boss.CanSeePlayer())
            {
                boss.TransitionToState(BossState.Idle);
            }
        }

        public void Exit(BossController boss)
        {
            // Ничего не делаем при выходе
        }
    }
}