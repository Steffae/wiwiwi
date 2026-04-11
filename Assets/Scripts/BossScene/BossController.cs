using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Game.Core;

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
        private bool hasBeenAttackedByPlayer = false;

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
        public bool HasBeenAttackedByPlayer => hasBeenAttackedByPlayer;
        public bool IsEnraged { get; private set; }
        public Transform MeleeAttackPoint => meleeAttackPoint;
        public Transform RangedAttackPoint => rangedAttackPoint;
        public GameObject DefaultProjectilePrefab => defaultProjectilePrefab;

        // События
        public System.Action<float, float> OnHealthChanged;
        public System.Action OnBossDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            bossCollider = GetComponent<Collider>();

            currentHealth = stats.maxHealth;

            InitializeStates();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
            {
                Debug.LogError("Boss: Player not found!");
            }

            // Подписываемся на мирный режим
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SubscribeToPeacefulMode(OnPeacefulModeChanged);
            }

            CreateHealthBar();
            TransitionToState(BossState.Idle);
        }

        private void Update()
        {
            if (isDead || player == null) return;

            if (attackTimer > 0) attackTimer -= Time.deltaTime;
            if (stunTimer > 0) stunTimer -= Time.deltaTime;

            currentState?.Update(this);

            UpdateMovementAnimation();
        }

        private void OnPeacefulModeChanged(bool peaceful)
        {
            isPeacefulMode = peaceful;
            hasBeenAttackedByPlayer = false;
            Debug.Log($"Boss: Peaceful mode = {peaceful}");
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
                Instantiate(healthBarPrefab, healthBarPosition);
            }
        }

        public bool IsAgentReady()
        {
            return agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh && agent.enabled;
        }

        public void SafeSetDestination(Vector3 destination)
        {
            if (!IsAgentReady()) return;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                agent.ResetPath();
            }

            agent.SetDestination(destination);
        }

        public void SafeSetAgentStopped(bool stopped)
        {
            if (!IsAgentReady()) return;

            agent.isStopped = stopped;

            if (!stopped)
            {
                agent.ResetPath();
            }
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            // В мирном режиме отмечаем, что босса атаковали
            if (isPeacefulMode && !hasBeenAttackedByPlayer)
            {
                hasBeenAttackedByPlayer = true;
                Debug.Log("Boss was attacked! Entering aggressive mode.");
            }

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);

            animator.SetTrigger("TakeHit");

            if (damage >= stats.stunThreshold && currentStateType != BossState.Stunned)
            {
                TransitionToState(BossState.Stunned);
            }

            if (!IsEnraged && currentHealth <= stats.maxHealth * stats.enrageHealthThreshold)
            {
                IsEnraged = true;
                TransitionToState(BossState.Enrage);
            }

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

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnsubscribeFromPeacefulMode(OnPeacefulModeChanged);
            }
        }
    }
}