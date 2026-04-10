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

            // Эффект ярости (можно добавить партиклы)
            Debug.Log("BOSS ENRAGED!");

            boss.StartCoroutine(EnrageRoutine(boss));
        }

        private IEnumerator EnrageRoutine(BossController boss)
        {
            // Анимация/эффект ярости
            yield return new WaitForSeconds(1.5f);

            if (boss.CurrentHealth > 0)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Update(BossController boss)
        {
            // Можно добавить эффект дрожания камеры или экрана
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