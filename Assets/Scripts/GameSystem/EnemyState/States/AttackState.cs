using UnityEngine;
using System.Collections;

namespace Game.Enemy
{
    public class AttackState : IEnemyState
    {
        protected float lastAttackTime = 0f;

        public virtual void Enter(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(true);

            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", 0f);
            }

            Debug.Log($"{enemy.gameObject.name}: Атака");
        }

        public virtual void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            if (enemy.ShouldFlee())
            {
                enemy.TransitionToState(EnemyState.Flee);
                return;
            }

            enemy.FacePlayer();

            if (Time.time > lastAttackTime + enemy.attackCooldown && !enemy.IsStateChanging)
            {
                lastAttackTime = Time.time;
                enemy.SetStateChanging(true);
                enemy.StartCoroutine(PerformAttack(enemy));
            }
        }

        protected virtual IEnumerator PerformAttack(EnemyController enemy)
        {
            yield return new WaitForSeconds(0.5f);
            enemy.SetStateChanging(false);

            if (!enemy.IsDying && enemy.CurrentStateType == EnemyState.Attack)
            {
                if (enemy.DistanceToPlayer > enemy.attackRange)
                {
                    enemy.TransitionToState(EnemyState.Chase);
                }
            }
        }

        public virtual void Exit(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(false);
        }
    }
}