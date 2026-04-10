using UnityEngine;

namespace Game.Boss
{
    public class IdleState : IBossState
    {
        private float idleTimer = 0f;
        private const float maxIdleTime = 3f;

        public void Enter(BossController boss)
        {
            idleTimer = 0f;

            if (boss.Agent != null)
            {
                boss.Agent.isStopped = true;
            }

            boss.Animator.SetFloat("Speed", 0f);
        }

        public void Update(BossController boss)
        {
            // Если босс мёртв - не делаем ничего
            if (boss.CurrentHealth <= 0) return;

            // Если есть игрок - преследуем
            if (boss.Player != null)
            {
                float distance = boss.DistanceToPlayer();

                // Если игрок в зоне видимости - сразу в Chase
                if (distance <= boss.Stats.attackRange * 3f && boss.CanSeePlayer())
                {
                    boss.TransitionToState(BossState.Chase);
                    return;
                }
            }

            // Просто стоим какое-то время, потом патрулируем (или снова Idle)
            idleTimer += Time.deltaTime;
            if (idleTimer >= maxIdleTime)
            {
                // Можно добавить патруль, но пока просто сбрасываем таймер
                idleTimer = 0f;
            }
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