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

            boss.SafeSetAgentStopped(true);
            boss.FacePlayer();
            boss.Animator.SetTrigger("Attack");

            boss.StartCoroutine(PerformAttack(boss));
        }

        private IEnumerator PerformAttack(BossController boss)
        {
            yield return new WaitForSeconds(0.4f);

            if (!hasAttacked && boss.Player != null)
            {
                hasAttacked = true;

                float distance = boss.DistanceToPlayer();

                if (boss.CurrentWeapon == BossWeaponType.Melee)
                {
                    if (distance <= boss.AttackRange && boss.CanSeePlayer())
                    {
                        boss.PerformMeleeAttack();
                    }
                }
                else
                {
                    boss.PerformRangedAttack();
                }
            }

            yield return new WaitForSeconds(0.8f);

            boss.SetAttackTimer(boss.AttackCooldown);

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