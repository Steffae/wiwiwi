using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Boss
{
    public class BossController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BossStats stats;
        [SerializeField] private Transform healthBarPosition;
        [SerializeField] private GameObject healthBarPrefab;

        [Header("Combat Points")]
        [SerializeField] private Transform meleeAttackPoint;
        [SerializeField] private Transform rangedAttackPoint;
        [SerializeField] private GameObject defaultProjectilePrefab;

        // Компоненты
        private Animator animator;
        private NavMeshAgent agent;
        private Collider bossCollider;

        // Состояния
        private Dictionary<BossState, IBossState> states;
        private IBossState currentState;
        private BossState currentStateType;

        // Характеристики
        private float currentHealth;
        private bool isDead = false;
        private bool isPeacefulMode = false;

        // Таймеры
        private float attackTimer = 0f;
        private float stunTimer = 0f;

        // Цель (игрок)
        private Transform player;

        // Свойства
        public BossStats Stats => stats;
        public Animator Animator => animator;
        public NavMeshAgent Agent => agent;
        public Transform Player => player;
        public float CurrentHealth => currentHealth;
        public float AttackTimer { get => attackTimer; set => attackTimer = value; }
        public float StunTimer { get => stunTimer; set => stunTimer = value; }
        public bool IsPeacefulMode { get => isPeacefulMode; set => isPeacefulMode = value; }
        public bool IsEnraged { get; private set; }
        public Transform MeleeAttackPoint => meleeAttackPoint;
        public Transform RangedAttackPoint => rangedAttackPoint;
        public GameObject DefaultProjectilePrefab => defaultProjectilePrefab;

        // События
        public System.Action<float, float> OnHealthChanged; // current, max
        public System.Action OnBossDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            bossCollider = GetComponent<Collider>();

            // Убеждаемся, что агент правильно инициализирован
            if (agent != null)
            {
                agent.autoTraverseOffMeshLink = true;
                agent.autoRepath = true;
                agent.isStopped = true; // Начинаем с остановленного
            }

            currentHealth = stats.maxHealth;

            InitializeStates();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
            {
                Debug.LogError("Boss: Player not found! Make sure Player has 'Player' tag.");
            }

            // Создаём Health Bar
            CreateHealthBar();

            // Начинаем с Idle
            TransitionToState(BossState.Idle);
        }

        private void Update()
        {
            if (isDead || player == null) return;

            // Обновляем таймеры
            if (attackTimer > 0) attackTimer -= Time.deltaTime;
            if (stunTimer > 0) stunTimer -= Time.deltaTime;

            // Обновляем текущее состояние
            currentState?.Update(this);

            // Обновляем анимацию движения
            UpdateMovementAnimation();
        }

        private void InitializeStates()
        {
            states = new Dictionary<BossState, IBossState>
            {
                { BossState.Idle, new IdleState() },
                { BossState.Chase, new ChaseState() },
                { BossState.Attack, new AttackState() },
                { BossState.HeavyAttack, new HeavyAttackState() },
                { BossState.Stunned, new StunnedState() },
                { BossState.Flee, new FleeState() },
                { BossState.Enrage, new EnrageState() }
            };
        }

        public void TransitionToState(BossState newState)
        {
            if (isDead && newState != BossState.Dead) return;

            currentState?.Exit(this);
            currentStateType = newState;
            currentState = states[newState];
            currentState?.Enter(this);

            Debug.Log($"Boss transitioned to: {newState}");
        }

        private void UpdateMovementAnimation()
        {
            if (agent != null && animator != null)
            {
                float speed = agent.velocity.magnitude;
                animator.SetFloat("Speed", speed);
            }
        }

        private void CreateHealthBar()
        {
            if (healthBarPrefab != null && healthBarPosition != null)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarPosition);
                // Здесь позже настроим связь с UI Slider
            }
        }

        // ===== ПУБЛИЧНЫЕ МЕТОДЫ =====

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);

            // Анимация получения урона
            animator.SetTrigger("TakeHit");

            // Проверка на стан
            if (damage >= stats.stunThreshold && currentStateType != BossState.Stunned)
            {
                TransitionToState(BossState.Stunned);
            }

            // Проверка на Enrage
            if (!IsEnraged && currentHealth <= stats.maxHealth * stats.enrageHealthThreshold)
            {
                IsEnraged = true;
                TransitionToState(BossState.Enrage);
            }

            // Проверка на смерть
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            animator.SetTrigger("Death");
            agent.isStopped = true;
            bossCollider.enabled = false;

            OnBossDeath?.Invoke();

            StartCoroutine(DeathCoroutine());
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }

        public float DistanceToPlayer()
        {
            if (player == null) return float.MaxValue;
            return Vector3.Distance(transform.position, player.position);
        }

        public bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector3 direction = player.position - transform.position;
            if (Physics.Raycast(transform.position + Vector3.up, direction.normalized,
                out RaycastHit hit, direction.magnitude))
            {
                return hit.transform.CompareTag("Player");
            }
            return false;
        }

        public void FacePlayer()
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    stats.rotationSpeed * Time.deltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (stats != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, stats.attackRange);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, stats.heavyAttackRange);
            }
        }
    }
}