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
        [SerializeField] private float baseDamage = 15f;
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

        private bool isEnraged = false;
        private float attackTimer = 0f;
        private Transform player;
        private BossController boss;

        // Свойства
        public BossWeaponType CurrentWeapon => currentWeapon;
        public BossElementType CurrentElement => currentElement;
        public float AttackTimer => attackTimer;
        public float AttackRange => attackRange;
        public float HeavyAttackRange => heavyAttackRange;

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
            Debug.Log($"Boss initialized: {currentWeapon} + {currentElement}");
        }

        public void SetEnraged(bool enraged) => isEnraged = enraged;

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

        public void PerformMeleeAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage();
            DealDamageToPlayer(damage);
            attackTimer = attackCooldown;
        }

        public void PerformHeavyMeleeAttack()
        {
            if (player == null) return;

            float damage = CalculateDamage() * heavyDamageMultiplier;
            DealDamageToPlayer(damage);
            attackTimer = heavyAttackCooldown;
        }

        private void DealDamageToPlayer(float damage)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Boss dealt {damage} {currentElement} damage!");
            }

            ApplyElementEffect(player.gameObject);

            // Отмечаем, что босса атаковали (для мирного режима)
            if (boss != null)
            {
                boss.HasBeenAttackedByPlayer = true;
            }
        }

        public void LaunchProjectile()
        {
            GameObject prefab = GetProjectilePrefab();
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

            float damage = CalculateDamage();
            projScript.Initialize(damage, currentElement, this, boss);

            attackTimer = attackCooldown;
        }

        private GameObject GetProjectilePrefab()
        {
            if (stats == null) return null;

            return currentElement switch
            {
                BossElementType.Fire => stats.fireProjectilePrefab,
                BossElementType.Ice => stats.iceProjectilePrefab,
                BossElementType.Earth => stats.earthProjectilePrefab,
                BossElementType.Ether => stats.etherProjectilePrefab,
                _ => stats.defaultProjectilePrefab
            };
        }

        private void ApplyElementEffect(GameObject target)
        {
            switch (currentElement)
            {
                case BossElementType.Fire:
                    StartCoroutine(ApplyFireEffect(target));
                    break;
                case BossElementType.Ice:
                    StartCoroutine(ApplyIceEffect(target));
                    break;
                case BossElementType.Earth:
                    ApplyEarthEffect(target);
                    break;
                case BossElementType.Ether:
                    ApplyEtherEffect(target);
                    break;
            }
        }

        private IEnumerator ApplyFireEffect(GameObject target)
        {
            HealthComponent health = target.GetComponent<HealthComponent>();
            if (health == null || stats == null) yield break;

            float elapsed = 0f;
            while (elapsed < stats.fireDuration)
            {
                health.TakeDamage(stats.fireDamageOverTime * 0.5f);
                elapsed += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator ApplyIceEffect(GameObject target)
        {
            PlayerController playerCtrl = target.GetComponent<PlayerController>();
            if (playerCtrl == null || stats == null) yield break;

            float origWalk = playerCtrl.walkSpeed;
            float origRun = playerCtrl.runSpeed;

            playerCtrl.walkSpeed *= (1 - stats.iceSlowAmount);
            playerCtrl.runSpeed *= (1 - stats.iceSlowAmount);

            yield return new WaitForSeconds(stats.iceSlowDuration);

            playerCtrl.walkSpeed = origWalk;
            playerCtrl.runSpeed = origRun;
        }

        private void ApplyEarthEffect(GameObject target)
        {
            if (stats == null) return;

            if (Random.value < stats.earthStunChance)
            {
                PlayerController playerCtrl = target.GetComponent<PlayerController>();
                if (playerCtrl != null)
                {
                    StartCoroutine(StunPlayer(playerCtrl));
                }
            }
        }

        private IEnumerator StunPlayer(PlayerController playerCtrl)
        {
            playerCtrl.enabled = false;
            yield return new WaitForSeconds(stats.earthStunDuration);
            playerCtrl.enabled = true;
        }

        private void ApplyEtherEffect(GameObject target)
        {
            Debug.Log($"Ether effect: burned {stats?.etherManaBurn ?? 0} mana");
        }

        public Transform GetMeleeAttackPoint() => meleeAttackPoint;

        // Сеттер для таймера атаки
        public void SetAttackTimer(float value)
        {
            attackTimer = value;
        }

        // Публичный метод для применения эффекта стихии
        public void ApplyElementEffectPublic(GameObject target)
        {
            ApplyElementEffect(target);
        }

        // Публичный метод для запуска снаряда с указанным уроном
        public void LaunchProjectile(float damage)
        {
            GameObject prefab = GetProjectilePrefab();
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

            projScript.Initialize(damage, currentElement, this, boss);

            attackTimer = attackCooldown;
        }
    }
}