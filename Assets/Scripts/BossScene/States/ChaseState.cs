using UnityEngine;

namespace Game.Boss
{
    public class ChaseState : IBossState
    {
        public void Enter(BossController boss)
        {
            boss.SafeSetAgentStopped(false);

            if (boss.IsAgentReady())
            {
                float speed = boss.IsEnraged ?
                    boss.Stats.runSpeed * boss.Stats.enrageSpeedMultiplier :
                    boss.Stats.runSpeed;

                boss.Agent.speed = speed;
            }
        }

        public void Update(BossController boss)
        {
            if (boss.Player == null) return;

            // Проверяем, можно ли атаковать
            bool canAttack = !boss.IsPeacefulMode || boss.HasBeenAttackedByPlayer;

            if (boss.IsAgentReady())
            {
                boss.SafeSetDestination(boss.Player.position);
            }

            float distance = boss.DistanceToPlayer();

            if (canAttack && distance <= boss.Stats.attackRange && boss.CanSeePlayer())
            {
                if (boss.AttackTimer <= 0)
                {
                    if (Random.value < 0.3f)
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

            // В мирном режиме если HP мало - убегаем
            if (boss.IsPeacefulMode)
            {
                float healthPercent = boss.CurrentHealth / boss.Stats.maxHealth;
                if (healthPercent <= boss.Stats.fleeHealthAmount)
                {
                    boss.TransitionToState(BossState.Flee);
                    return;
                }
            }

            if (distance > boss.Stats.attackRange * 5f && !boss.CanSeePlayer())
            {
                boss.TransitionToState(BossState.Idle);
            }
        }

        public void Exit(BossController boss)
        {
            // Ничего не делаем
        }
    }
}