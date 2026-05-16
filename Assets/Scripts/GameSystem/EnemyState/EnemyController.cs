using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Game.Core;

namespace Game.Enemy
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Flee
    }

    public class EnemyController : EnemyBase
    {
        [Header("State Machine")]
        [SerializeField] private EnemyState currentStateType = EnemyState.Idle;
        private IEnemyState currentState;

        [Header("Chase Settings")]
        public float chaseRange = 10f;

        [Header("Flee Settings")]
        public float fleeHealthPercent = 30f;
        public float fleeDistance = 15f;
        public float fleeSpeed = 5f;

        [Header("Patrol Settings")]
        public float patrolRadius = 10f;

        [Header("Attack Settings")]
        public float attackRange = 2f;
        public float attackCooldown = 2f;

        protected bool isPeacefulMode = false;
        protected bool isStateChanging = false;
        protected Transform player;

        public NavMeshAgent Agent => agent;
        public Animator Animator => animator;
        public Transform Player => player;
        public EnemyState CurrentStateType => currentStateType;
        public bool IsPeacefulMode => isPeacefulMode;
        public bool IsStateChanging => isStateChanging;
        public float DistanceToPlayer => player != null ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

        protected Dictionary<EnemyState, IEnemyState> states;

        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            InitializeStates();
        }

        protected void RegisterState(EnemyState stateType, IEnemyState state)
        {
            if (states == null)
            {
                states = new Dictionary<EnemyState, IEnemyState>();
            }
            states[stateType] = state;
        }

        protected override void Start()
        {
            base.Start();

            if (GameManager.Instance != null)
            {
                isPeacefulMode = GameManager.Instance.IsPeacefulMode;
                GameManager.Instance.SubscribeToPeacefulMode(OnPeacefulModeChanged);
            }

            TransitionToState(EnemyState.Idle);
        }

        protected virtual void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SubscribeToPeacefulMode(OnPeacefulModeChanged);
            }
        }

        protected virtual void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnsubscribeFromPeacefulMode(OnPeacefulModeChanged);
            }
        }

        private void OnPeacefulModeChanged(bool isPeaceful)
        {
            isPeacefulMode = isPeaceful;
            Debug.Log($"{gameObject.name}: режим изменён на {(isPeaceful ? "МИРНЫЙ" : "АГРЕССИВНЫЙ")}");

            if (isPeacefulMode && (currentStateType == EnemyState.Chase || currentStateType == EnemyState.Attack))
            {
                TransitionToState(EnemyState.Idle);
            }
        }

        protected virtual void Update()
        {
            if (player == null || isDying || isHit || isStateChanging) return;

            if (agent != null && !agent.isOnNavMesh) return;

            currentState?.Update(this);
        }

        private void InitializeStates()
        {
            states = new Dictionary<EnemyState, IEnemyState>
            {
                { EnemyState.Idle, new IdleState() },
                { EnemyState.Chase, new ChaseState() },
                { EnemyState.Attack, new AttackState() },
                { EnemyState.Flee, new FleeState() }
            };
        }

        public void TransitionToState(EnemyState newState)
        {
            if (isDying) return;

            currentState?.Exit(this);
            currentStateType = newState;
            currentState = states[newState];
            currentState?.Enter(this);

            Debug.Log($"{gameObject.name} -> {newState} (HP: {healthSystem.CurrentHealth}/{maxHealth})");
        }

        public bool ShouldFlee()
        {
            float healthPercent = (healthSystem.CurrentHealth / maxHealth) * 100f;
            return healthPercent <= fleeHealthPercent && !isDying;
        }

        public virtual bool CanAttack()
        {
            return false;
        }

        public bool IsAgentReady()
        {
            return agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh && !isDying;
        }

        public void SafeSetAgentStopped(bool stopped)
        {
            if (!IsAgentReady()) return;
            agent.isStopped = stopped;
        }

        public void SafeSetDestination(Vector3 destination)
        {
            if (!IsAgentReady()) return;
            agent.SetDestination(destination);
        }

        public void FacePlayer()
        {
            if (player == null) return;
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);
        }

        public Vector3 GetPatrolTarget()
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
                randomDirection.y = 0;
                Vector3 randomPoint = transform.position + randomDirection;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }

        public void SetStateChanging(bool value)
        {
            isStateChanging = value;
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
        }

        protected override void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnsubscribeFromPeacefulMode(OnPeacefulModeChanged);
            }
            base.OnDestroy();
        }
    }
}