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
        public float stunThreshold = 50f;

        [Header("Enrage")]
        [Range(0, 1)]
        public float enrageHealthThreshold = 0.3f;

        [Header("Flee")]
        [Range(0, 1)]
        public float fleeHealthThreshold = 0.2f;
        public float fleeDistance = 20f;

        // ===== НОВЫЕ ПОЛЯ ДЛЯ СТИХИЙ =====

        [Header("Element Effects - Fire")]
        public float fireDamageOverTime = 5f;
        public float fireDuration = 3f;
        public Color fireColor = new Color(1f, 0.3f, 0f);
        public GameObject fireHitEffectPrefab;
        public GameObject fireProjectilePrefab;

        [Header("Element Effects - Ice")]
        public float iceSlowAmount = 0.5f;
        public float iceSlowDuration = 2f;
        public Color iceColor = new Color(0.3f, 0.8f, 1f);
        public GameObject iceHitEffectPrefab;
        public GameObject iceProjectilePrefab;

        [Header("Element Effects - Earth")]
        public float earthStunChance = 0.3f;
        public float earthStunDuration = 1.5f;
        public Color earthColor = new Color(0.6f, 0.4f, 0.1f);
        public GameObject earthHitEffectPrefab;
        public GameObject earthProjectilePrefab;

        [Header("Element Effects - Ether")]
        public float etherManaBurn = 20f;
        public Color etherColor = new Color(0.8f, 0.2f, 0.8f);
        public GameObject etherHitEffectPrefab;
        public GameObject etherProjectilePrefab;

        [Header("Default Projectile")]
        public GameObject defaultProjectilePrefab;
    }
}