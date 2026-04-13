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

            boss.SafeSetAgentStopped(true);
            boss.FacePlayer();
            boss.Animator.SetTrigger("HeavyAttack");

            boss.StartCoroutine(PerformHeavyAttack(boss));
        }

        private IEnumerator PerformHeavyAttack(BossController boss)
        {
            yield return new WaitForSeconds(0.8f);

            if (!hasAttacked && boss.Player != null)
            {
                hasAttacked = true;

                float distance = boss.DistanceToPlayer();

                if (boss.CurrentWeapon == BossWeaponType.Melee)
                {
                    if (distance <= boss.HeavyAttackRange && boss.CanSeePlayer())
                    {
                        boss.PerformHeavyMeleeAttack();
                    }
                }
                else
                {
                    boss.PerformHeavyRangedAttack();
                }
            }

            yield return new WaitForSeconds(0.7f);

            boss.SetAttackTimer(boss.HeavyAttackCooldown);

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
            boss.SafeSetAgentStopped(false);
        }
    }
}