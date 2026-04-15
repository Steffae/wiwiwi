using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class BossCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private float heavyAttackRange = 4f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float heavyAttackCooldown = 5f;
        [SerializeField] private float baseDamage = 25f;
        [SerializeField] private float heavyDamageMultiplier = 2f;
        [SerializeField] private float enrageDamageMultiplier = 1.3f;

        [Header("Attack Points")]
        [SerializeField] private Transform meleeAttackPoint;
        [SerializeField] private Transform rangedAttackPoint;

        [Header("Weapon & Element")]
        [SerializeField] private BossWeaponType currentWeapon;
        [SerializeField] private BossElementType currentElement;

        [Header("Stats Reference")]
        [SerializeField] private BossStats stats;

        [Header("Element Components")]
        [SerializeField] private FireElement fireElement;
        [SerializeField] private IceElement iceElement;
        [SerializeField] private EarthElement earthElement;
        [SerializeField] private EtherElement etherElement;

        private bool isEnraged = false;
        private float attackTimer = 0f;
        private Transform player;
        private BossController boss;
        private IAudioService audioS;
        private BossElementBase currentElementComponent;

        // Свойства
        public BossWeaponType CurrentWeapon => currentWeapon;
        public BossElementType CurrentElement => currentElement;
        public float AttackTimer => attackTimer;
        public float AttackRange => attackRange;
        public float HeavyAttackRange => heavyAttackRange;
        public float AttackCooldown => attackCooldown;
        public float HeavyAttackCooldown => heavyAttackCooldown;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            boss = GetComponent<BossController>();
            InitializeWeaponAndElement();
        }

        private void Update()
        {
            if (attackTimer > 0) attackTimer -= Time.deltaTime;
        }

        public void InitializeWeaponAndElement()
        {
            currentWeapon = (BossWeaponType)Random.Range(0, 2);
            currentElement = (BossElementType)Random.Range(0, 4);

            // Выбираем компонент стихии
            currentElementComponent = GetElementComponent(currentElement);
            if (currentElementComponent != null)
            {
                currentElementComponent.Initialize(audioS, boss, this, stats);
            }

            Debug.Log($"Boss initialized: {currentWeapon} + {currentElement}");
        }

        private BossElementBase GetElementComponent(BossElementType element)
        {
            return element switch
            {
                BossElementType.Fire => fireElement,
                BossElementType.Ice => iceElement,
                BossElementType.Earth => earthElement,
                BossElementType.Ether => etherElement,
                _ => fireElement
            };
        }

        public void SetEnraged(bool enraged) => isEnraged = enraged;
        public void SetAttackTimer(float value) => attackTimer = value;
        public bool CanAttack() => attackTimer <= 0;

        public bool IsPlayerInAttackRange()
        {
            if (player == null) return false;
            return Vector3.Distance(transform.position, player.position) <= attackRange;
        }

        public bool IsPlayerInHeavyRange()
        {
            if (player == null) return false;
            return Vector3.Distance(transform.position, player.position) <= heavyAttackRange;
        }

        public bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector3 dir = player.position - transform.position;
            if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, out RaycastHit hit, dir.magnitude))
            {
                return hit.transform.CompareTag("Player");
            }
            return false;
        }

        public float CalculateDamage()
        {
            float dmg = baseDamage;
            if (isEnraged) dmg *= enrageDamageMultiplier;
            return dmg;
        }

        // ОБЫЧНАЯ АТАКА 

        public void PerformMeleeAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage();
            DealDamageToPlayer(damage);

            // Применяем эффект стихии (обычная ближняя)
            currentElementComponent?.ApplyMeleeEffect(player.gameObject);

            attackTimer = attackCooldown;
        }

        public void PerformRangedAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage();
            LaunchProjectile(damage, false);

            attackTimer = attackCooldown;
        }

        // ТЯЖЁЛАЯ АТАКА 

        public void PerformHeavyMeleeAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage() * heavyDamageMultiplier;
            DealDamageToPlayer(damage);

            // Применяем эффект стихии (тяжёлая ближняя)
            currentElementComponent?.ApplyHeavyMeleeEffect(player.gameObject);

            attackTimer = heavyAttackCooldown;
        }

        public void PerformHeavyRangedAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage() * heavyDamageMultiplier;
            LaunchProjectile(damage, true);

            attackTimer = heavyAttackCooldown;
        }

        // УНИВЕРСАЛЬНЫЕ МЕТОД

        private void DealDamageToPlayer(float damage)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Boss dealt {damage} damage!");
            }

            if (boss != null)
            {
                boss.HasBeenAttackedByPlayer = true;
            }
        }

        public void LaunchProjectile(float damage, bool isHeavy)
        {
            GameObject prefab = currentElementComponent?.GetProjectilePrefab();
            if (prefab == null || player == null) return;

            Vector3 spawnPos = rangedAttackPoint != null ?
                rangedAttackPoint.position :
                transform.position + transform.forward * 2f + Vector3.up * 1.5f;

            GameObject proj = Instantiate(prefab, spawnPos, Quaternion.identity);
            Vector3 dir = (player.position + Vector3.up - spawnPos).normalized;

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = dir * 15f;

            BossProjectile projScript = proj.GetComponent<BossProjectile>();
            if (projScript == null) projScript = proj.AddComponent<BossProjectile>();

            projScript.Initialize(damage, currentElement, this, boss, isHeavy);
        }

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        public Transform GetMeleeAttackPoint() => meleeAttackPoint;
        public Transform GetRangedAttackPoint() => rangedAttackPoint;
        public BossElementBase GetCurrentElementComponent() => currentElementComponent;
    }
}