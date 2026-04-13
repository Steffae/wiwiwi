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
            boss.SafeSetAgentStopped(true);
            boss.Animator.SetFloat("Speed", 0f);
        }

        public void Update(BossController boss)
        {
            if (boss.CurrentHealth <= 0) return;

            if (boss.Player != null)
            {
                float distance = boss.DistanceToPlayer();

                // В обычном режиме или если босса уже атаковали в мирном - преследуем
                bool shouldChase = !boss.IsPeacefulMode || boss.HasBeenAttackedByPlayer;

                if (shouldChase && distance <= boss.AttackRange * 3f && boss.CanSeePlayer())
                {
                    boss.TransitionToState(BossState.Chase);
                    return;
                }
            }

            idleTimer += Time.deltaTime;
            if (idleTimer >= maxIdleTime)
            {
                idleTimer = 0f;
            }
        }

        public void Exit(BossController boss)
        {
            boss.SafeSetAgentStopped(false);
        }
    }
}