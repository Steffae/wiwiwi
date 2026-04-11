using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
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

        // Компоненты
        private Animator animator;
        private Collider bossCollider;
        private BossHealth bossHealth;
        private BossMovement bossMovement;
        private BossCombat bossCombat;

        // UI
        private Slider healthSlider;
        private Text healthText;

        // Состояния
        private Dictionary<BossState, IBossState> states;
        private IBossState currentState;
        private BossState currentStateType;

        // Состояние босса
        private bool isPeacefulMode = false;
        private bool hasBeenAttackedByPlayer = false;
        private float stunTimer = 0f;

        // Цель
        private Transform player;

        // Свойства
        public BossStats Stats => stats;
        public Animator Animator => animator;
        public NavMeshAgent Agent => bossMovement?.Agent;
        public Transform Player => player;
        public float CurrentHealth => bossHealth?.CurrentHealth ?? 0;
        public bool IsEnraged => bossHealth?.IsEnraged ?? false;
        public bool IsPeacefulMode { get => isPeacefulMode; set => isPeacefulMode = value; }
        public bool HasBeenAttackedByPlayer { get => hasBeenAttackedByPlayer; set => hasBeenAttackedByPlayer = value; }

        public float StunTimer { get => stunTimer; set => stunTimer = value; }
        public BossWeaponType CurrentWeapon => bossCombat?.CurrentWeapon ?? BossWeaponType.Melee;
        public BossElementType CurrentElement => bossCombat?.CurrentElement ?? BossElementType.Fire;
        public Transform MeleeAttackPoint => bossCombat?.GetMeleeAttackPoint();
        public Transform RangedAttackPoint => null;

        // Для совместимости с состояниями
        public float AttackRange => bossCombat?.AttackRange ?? stats.attackRange;
        public float HeavyAttackRange => bossCombat?.HeavyAttackRange ?? stats.heavyAttackRange;
        public float AttackCooldown => stats.attackCooldown;
        public float HeavyAttackCooldown => stats.heavyAttackCooldown;

        // События
        public System.Action<float, float> OnHealthChanged;
        public System.Action OnBossDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            bossCollider = GetComponent<Collider>();
            bossHealth = GetComponent<BossHealth>();
            bossMovement = GetComponent<BossMovement>();
            bossCombat = GetComponent<BossCombat>();

            InitializeStates();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            // Подписываемся на события здоровья
            if (bossHealth != null)
            {
                bossHealth.OnHealthChanged += (cur, max) => OnHealthChanged?.Invoke(cur, max);
                bossHealth.OnDeath += HandleDeath;
                bossHealth.OnEnrage += HandleEnrage;
                bossHealth.OnStunned += HandleStunned;
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
            if (bossHealth != null && bossHealth.IsDead) return;

            if (stunTimer > 0) stunTimer -= Time.deltaTime;

            currentState?.Update(this);
            bossMovement?.UpdateMovementAnimation(animator);
        }

        private void OnPeacefulModeChanged(bool peaceful)
        {
            isPeacefulMode = peaceful;
            hasBeenAttackedByPlayer = false;
        }

        private void HandleEnrage()
        {
            bossMovement?.SetEnraged(true);
            bossCombat?.SetEnraged(true);
            TransitionToState(BossState.Enrage);
        }

        private void HandleStunned(float duration)
        {
            stunTimer = duration;
            TransitionToState(BossState.Stunned);
        }

        private void HandleDeath()
        {
            animator?.SetTrigger("Death");
            if (Agent != null) Agent.isStopped = true;
            if (bossCollider != null) bossCollider.enabled = false;

            if (healthSlider != null) healthSlider.gameObject.SetActive(false);

            OnBossDeath?.Invoke();
            StartCoroutine(DeathCoroutine());
        }

        private IEnumerator DeathCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }

        // ===== МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ =====

        public void SetAgentStopped(bool stopped) => bossMovement?.SetStopped(stopped);
        public void ChasePlayer() => bossMovement?.ChasePlayer();
        public void FleeFromPlayer() => bossMovement?.FleeFromPlayer(stats.fleeDistance);
        public void FacePlayer() => bossMovement?.FacePlayer();
        public float DistanceToPlayer() => bossMovement?.DistanceToPlayer() ?? float.MaxValue;
        public bool CanSeePlayer() => bossCombat?.CanSeePlayer() ?? false;
        public bool CanAttack() => bossCombat?.CanAttack() ?? false;
        public bool IsPlayerInAttackRange() => bossCombat?.IsPlayerInAttackRange() ?? false;
        public bool IsPlayerInHeavyRange() => bossCombat?.IsPlayerInHeavyRange() ?? false;
        public void PerformMeleeAttack() => bossCombat?.PerformMeleeAttack();
        public void PerformHeavyMeleeAttack() => bossCombat?.PerformHeavyMeleeAttack();
        public void LaunchProjectile() => bossCombat?.LaunchProjectile();
        public bool ShouldFlee() => bossHealth?.ShouldFlee ?? false;
        public bool IsAgentReady() => bossMovement?.IsAgentReady() ?? false;

        // ===== МЕТОДЫ ДЛЯ СОВМЕСТИМОСТИ С СОСТОЯНИЯМИ =====

        public void SafeSetAgentStopped(bool stopped)
        {
            SetAgentStopped(stopped);
        }

        public void SafeSetDestination(Vector3 destination)
        {
            if (!IsAgentReady()) return;
            Agent.SetDestination(destination);
        }

        public void TakeDamage(float damage)
        {
            hasBeenAttackedByPlayer = true;
            bossHealth?.TakeDamage(damage);
        }

        // ===== УПРАВЛЕНИЕ СОСТОЯНИЯМИ =====

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
            if (bossHealth != null && bossHealth.IsDead) return;

            currentState?.Exit(this);
            currentStateType = newState;
            currentState = states[newState];
            currentState?.Enter(this);
        }

        // ===== UI =====

        private void CreateHealthBar()
        {
            if (healthBarPrefab != null && healthBarPosition != null)
            {
                GameObject obj = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity, healthBarPosition);
                obj.transform.localRotation = Quaternion.identity;

                healthSlider = obj.GetComponentInChildren<Slider>();
                if (healthSlider != null)
                {
                    healthSlider.maxValue = bossHealth?.MaxHealth ?? 100;
                    healthSlider.value = bossHealth?.CurrentHealth ?? 100;
                }

                healthText = obj.GetComponentInChildren<Text>();

                if (bossHealth != null)
                {
                    bossHealth.OnHealthChanged += UpdateHealthUI;
                }
            }
        }

        private void UpdateHealthUI(float current, float max)
        {
            if (healthSlider != null) healthSlider.value = current;
            if (healthText != null) healthText.text = $"{Mathf.Ceil(current)}/{max}";
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnsubscribeFromPeacefulMode(OnPeacefulModeChanged);
            }
        }

        // Свойства
        public float AttackTimer
        {
            get => bossCombat?.AttackTimer ?? 0;
            set => bossCombat?.SetAttackTimer(value);
        }

        // Методы для совместимости
        public void ApplyElementEffect(GameObject target)
        {
            bossCombat?.ApplyElementEffectPublic(target);
        }

        public void SpawnHitEffect(Vector3 position)
        {
            Debug.Log($"SpawnHitEffect at {position}");
        }

        public void LaunchProjectile(float damage)
        {
            bossCombat?.LaunchProjectile(damage);
        }

        public void SetAttackTimer(float value)
        {
            bossCombat?.SetAttackTimer(value);
        }
    }
}