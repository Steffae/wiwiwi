using UnityEngine;

namespace Game.Enemy
{
    public class FleeState : IEnemyState
    {
        private Vector3 fleeTarget;

        public void Enter(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(false);

            if (enemy.Agent != null)
            {
                enemy.Agent.speed = enemy.fleeSpeed;
            }

            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", 1.2f);
            }

            Debug.Log($"{enemy.gameObject.name}: Бегство HP {enemy.CurrentHealth}/{enemy.MaxHealth}");
        }

        public void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            float healthPercent = (enemy.CurrentHealth / enemy.MaxHealth) * 100f;
            if (healthPercent > enemy.fleeHealthPercent && !enemy.IsPeacefulMode)
            {
                enemy.TransitionToState(EnemyState.Idle);
                return;
            }

            if (enemy.IsPeacefulMode && enemy.DistanceToPlayer > enemy.chaseRange)
            {
                enemy.TransitionToState(EnemyState.Idle);
                return;
            }

            Vector3 fleeDirection = (enemy.transform.position - enemy.Player.position).normalized;
            fleeDirection.y = 0;
            fleeTarget = enemy.transform.position + fleeDirection * enemy.fleeDistance;

            if (enemy.Agent != null && enemy.IsAgentReady())
            {
                enemy.SafeSetDestination(fleeTarget);
            }
        }

        public void Exit(EnemyController enemy)
        {
            if (enemy.Agent != null && enemy.IsAgentReady())
            {
                enemy.Agent.speed = enemy.moveSpeed;
            }
        }
    }
}