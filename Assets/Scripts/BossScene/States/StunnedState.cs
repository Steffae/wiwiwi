using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class StunnedState : IBossState
    {
        public void Enter(BossController boss)
        {
            if (boss.Agent != null)
            {
                boss.Agent.isStopped = true;
            }

            boss.Animator.SetBool("IsStunned", true);
            boss.Animator.SetTrigger("TakeHit");

            boss.StunTimer = boss.Stats.stunDuration;

            boss.StartCoroutine(StunRoutine(boss));
        }

        private IEnumerator StunRoutine(BossController boss)
        {
            yield return new WaitForSeconds(boss.Stats.stunDuration);

            boss.Animator.SetBool("IsStunned", false);

            if (boss.CurrentHealth > 0)
            {
                // После стана либо в ярость, либо в преследование
                if (boss.IsEnraged)
                {
                    boss.TransitionToState(BossState.Enrage);
                }
                else
                {
                    boss.TransitionToState(BossState.Chase);
                }
            }
        }

        public void Update(BossController boss)
        {
            // В стане ничего не делаем
        }

        public void Exit(BossController boss)
        {
            if (boss.Agent != null)
            {
                boss.Agent.isStopped = false;
            }
            boss.Animator.SetBool("IsStunned", false);
        }
    }
}