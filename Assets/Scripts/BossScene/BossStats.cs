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

        [Header("Combat")] // аттака
        public float attackRange = 3f;
        public float heavyAttackRange = 4f;
        public float attackCooldown = 2f;
        public float heavyAttackCooldown = 5f;
        public float baseDamage = 25f;
        public float heavyDamageMultiplier = 2f;
        public float enrageDamageMultiplier = 1.3f;

        [Header("Stun")] // ожидание
        public float stunDuration = 2f;
        public float stunThreshold = 50f;

        [Header("Enrage")] // ярость
        [Range(0, 1)]
        public float enrageHealthThreshold = 0.3f;

        [Header("Flee")] // убегание
        public float fleeHealthAmount = 50f;
        public float fleeDistance = 20f;

        // СТИХИЯ ОГНЯ
        [Header("Fire Element")]
        public GameObject fireEffectPrefab;      // Горение на игроке
        public GameObject fireProjectilePrefab;  // Огненный шар
        public GameObject fireFlashPrefab;       // Вспышка при ближней атаке
        public float fireNormalBurnDuration = 2f;
        public float fireHeavyBurnDuration = 5f;
        public float fireFlashNormalDuration = 1f;
        public float fireFlashHeavyDuration = 3f;
        public float fireDamageOverTime = 5f;
        public Color fireColor = new Color(1f, 0.3f, 0f);

        // СТИХИЯ ЛЬДА
        [Header("Ice Element")]
        public GameObject iceProjectilePrefab;
        public GameObject iceEffectPrefab;
        public GameObject iceFreezeEffectPrefab;
        public AudioClip iceSound;
        public float iceNormalFreezeDuration = 3f;
        public float iceHeavyFreezeDuration = 5f;
        public Color iceColor = new Color(0.3f, 0.8f, 1f);

        // СТИХИЯ ЭФИРА
        [Header("Ether Element")]
        public GameObject etherProjectilePrefab;
        public GameObject etherEffectPrefab;
        public AudioClip etherSound;
        public float etherNormalInvisDuration = 3f;
        public float etherHeavyInvisDuration = 6f;
        public Color etherColor = new Color(0.8f, 0.2f, 0.8f);

        // СТИХИЯ ЗЕМЛИ
        [Header("Earth Element")]
        public GameObject earthProjectilePrefab;
        public GameObject earthMudEffectPrefab;
        public AudioClip earthSound;
        public float earthNormalSlowDuration = 2f;
        public float earthHeavySlowDuration = 5f;
        public float earthSlowAmount = 0.5f;
        public float earthKnockupForce = 5f;
        public Color earthColor = new Color(0.6f, 0.4f, 0.1f);

        // ЗАПАСНОЙ ПРЕФАБ
        [Header("Default")]
        public GameObject defaultProjectilePrefab;
    }
}