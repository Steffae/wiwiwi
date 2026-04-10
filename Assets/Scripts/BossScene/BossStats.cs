using UnityEngine;

namespace Game.Boss
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Game/Boss/Stats")]
    public class BossStats : ScriptableObject
    {
        [Header("Health")]
        public float maxHealth = 500f;

        [Header("Movement")]
        public float walkSpeed = 3.5f;
        public float runSpeed = 6f;
        public float enrageSpeedMultiplier = 1.5f;
        public float rotationSpeed = 10f;

        [Header("Combat")]
        public float attackRange = 3f;
        public float heavyAttackRange = 4f;
        public float rangedAttackRange = 15f;
        public float attackCooldown = 2f;
        public float heavyAttackCooldown = 5f;
        public float damage = 25f;
        public float heavyDamageMultiplier = 2f;
        public float enrageDamageMultiplier = 1.3f;

        [Header("Stun")]
        public float stunDuration = 2f;
        public float stunThreshold = 50f; // Урон для стана за одно попадание

        [Header("Enrage")]
        [Range(0, 1)]
        public float enrageHealthThreshold = 0.3f; // 30% HP

        [Header("Flee")]
        [Range(0, 1)]
        public float fleeHealthThreshold = 0.2f; // 20% HP
        public float fleeDistance = 20f;
    }
}