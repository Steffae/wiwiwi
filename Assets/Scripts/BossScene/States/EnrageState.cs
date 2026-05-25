using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class EnrageState : IBossState
    {
        public void Enter(BossController boss)
        {
            boss.Animator.SetBool("IsEnraged", true);

            if (boss.Agent != null)
            {
                boss.Agent.isStopped = true;
                boss.Agent.speed = boss.Stats.runSpeed * boss.Stats.enrageSpeedMultiplier;
            }

            // Эффект ярости
            Debug.Log("BOSS ENRAGED!");

            boss.StartCoroutine(EnrageRoutine(boss));
        }

        private IEnumerator EnrageRoutine(BossController boss)
        {
            yield return new WaitForSeconds(1.5f);

            if (boss.CurrentHealth > 0)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Update(BossController boss)
        {
            // Для эффекта ярости - потом когда-нибудь будет...
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