using UnityEngine;

namespace Game.Boss
{
    public class FleeState : IBossState
    {
        public void Enter(BossController boss)
        {
            boss.SafeSetAgentStopped(false);

            if (boss.IsAgentReady())
            {
                boss.Agent.speed = boss.Stats.runSpeed * 1.2f; // ускоренный побег
            }

            Debug.Log("Boss entered Flee state!");
        }

        public void Update(BossController boss)
        {
            if (boss.Player == null) return;

            Vector3 directionFromPlayer = boss.transform.position - boss.Player.position;
            Vector3 fleePosition = boss.transform.position + directionFromPlayer.normalized * boss.Stats.fleeDistance;

            boss.SafeSetDestination(fleePosition);

            float distance = boss.DistanceToPlayer();
            float currentHealth = boss.CurrentHealth;

            if (distance > boss.Stats.fleeDistance || currentHealth > boss.Stats.fleeHealthAmount * 1.5f)
            {
                boss.TransitionToState(BossState.Chase);
            }
        }

        public void Exit(BossController boss)
        {
            if (boss.IsAgentReady())
            {
                boss.Agent.speed = boss.IsEnraged ?
                    boss.Stats.runSpeed * boss.Stats.enrageSpeedMultiplier :
                    boss.Stats.runSpeed;
            }
        }
    }
}