using UnityEngine;

namespace Game.Boss
{
    public class FleeState : IBossState
    {
        public void Enter(BossController boss)
        {
            if (boss.Agent != null)
            {
                boss.Agent.isStopped = false;
                boss.Agent.speed = boss.Stats.runSpeed * 1.2f; // Бежим быстрее
            }
        }

        public void Update(BossController boss)
        {
            if (boss.Player == null) return;

            // Бежим ОТ игрока
            Vector3 directionFromPlayer = boss.transform.position - boss.Player.position;
            Vector3 fleePosition = boss.transform.position + directionFromPlayer.normalized * boss.Stats.fleeDistance;

            if (boss.Agent != null && boss.Agent.isActiveAndEnabled)
            {
                boss.Agent.SetDestination(fleePosition);
            }

            // Проверяем, восстановилось ли здоровье или игрок далеко
            float distance = boss.DistanceToPlayer();
            float healthPercent = boss.CurrentHealth / boss.Stats.maxHealth;

            if (distance > boss.Stats.fleeDistance || healthPercent > boss.Stats.fleeHealthThreshold * 1.5f)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Exit(BossController boss)
        {
            // Восстанавливаем нормальную скорость
            if (boss.Agent != null)
            {
                boss.Agent.speed = boss.IsEnraged ?
                    boss.Stats.runSpeed * boss.Stats.enrageSpeedMultiplier :
                    boss.Stats.runSpeed;
            }
        }
    }
}