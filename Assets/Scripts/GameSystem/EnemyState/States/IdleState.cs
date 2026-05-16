using UnityEngine;

namespace Game.Enemy
{
    public class IdleState : IEnemyState
    {
        private Vector3 patrolTarget;
        private float patrolTimer = 0f;
        private const float patrolInterval = 5f;

        public void Enter(EnemyController enemy)
        {
            patrolTimer = 0f;
            patrolTarget = enemy.GetPatrolTarget();
            enemy.SafeSetAgentStopped(false);

            if (enemy.Agent != null)
            {
                enemy.Agent.speed = enemy.moveSpeed;
            }

            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", 0.1f);
            }
        }

        public void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            float distance = enemy.DistanceToPlayer;

            if (!enemy.IsPeacefulMode && distance <= enemy.chaseRange)
            {
                enemy.TransitionToState(EnemyState.Chase);
                return;
            }

            if (enemy.ShouldFlee())
            {
                enemy.TransitionToState(EnemyState.Flee);
                return;
            }

            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolInterval)
            {
                patrolTimer = 0f;
                patrolTarget = enemy.GetPatrolTarget();
            }

            if (enemy.Agent != null && enemy.IsAgentReady())
            {
                enemy.SafeSetDestination(patrolTarget);
            }
        }

        public void Exit(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(false);
        }
    }
}