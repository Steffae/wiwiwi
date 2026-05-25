using UnityEngine;

namespace Game.Enemy
{
    public class ChaseState : IEnemyState
    {
        public void Enter(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(false);

            if (enemy.Agent != null)
            {
                enemy.Agent.speed = enemy.moveSpeed;
            }

            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", 1f);
            }
        }

        public void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            if (enemy.ShouldFlee())
            {
                enemy.TransitionToState(EnemyState.Flee);
                return;
            }

            if (!enemy.IsPeacefulMode && enemy.CanAttack() && enemy.DistanceToPlayer <= enemy.attackRange)
            {
                enemy.TransitionToState(EnemyState.Attack);
                return;
            }

            if (enemy.IsPeacefulMode && enemy.DistanceToPlayer > enemy.chaseRange)
            {
                enemy.TransitionToState(EnemyState.Idle);
                return;
            }

            if (enemy.Agent != null && enemy.IsAgentReady())
            {
                enemy.SafeSetDestination(enemy.Player.position);
            }
        }

        public void Exit(EnemyController enemy)
        {
        }
    }
}