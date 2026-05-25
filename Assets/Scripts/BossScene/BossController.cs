using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;
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

        private Animator animator;
        private Collider bossCollider;
        private IHealth health;
        private BossMovement bossMovement;
        private BossCombat bossCombat;

        private Slider healthSlider;
        private Text healthText;

        private Dictionary<BossState, IBossState> states;
        private IBossState currentState;
        private BossState currentStateType;

        private bool isPeacefulMode = false;
        private bool hasBeenAttackedByPlayer = false;
        private float stunTimer = 0f;

        private Transform player;

        public BossStats Stats => stats;
        public Animator Animator => animator;
        public NavMeshAgent Agent => bossMovement?.Agent;
        public Transform Player => player;
        public float CurrentHealth => health?.CurrentHealth ?? 0;
        public bool IsEnraged { get; private set; }
        public bool IsPeacefulMode { get => isPeacefulMode; set => isPeacefulMode = value; }
        public bool HasBeenAttackedByPlayer { get => hasBeenAttackedByPlayer; set => hasBeenAttackedByPlayer = value; }

        public float StunTimer { get => stunTimer; set => stunTimer = value; }
        public BossWeaponType CurrentWeapon => bossCombat?.CurrentWeapon ?? BossWeaponType.Melee;
        public BossElementType CurrentElement => bossCombat?.CurrentElement ?? BossElementType.Fire;
        public Transform MeleeAttackPoint => bossCombat?.GetMeleeAttackPoint();
        public Transform RangedAttackPoint => null;

        private BossDeathPortal bdp;

        public float AttackRange => bossCombat?.AttackRange ?? 3f;
        public float HeavyAttackRange => bossCombat?.HeavyAttackRange ?? 4f;
        public float AttackCooldown => bossCombat?.AttackCooldown ?? 2f;
        public float HeavyAttackCooldown => bossCombat?.HeavyAttackCooldown ?? 5f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnBossDeath;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            bossCollider = GetComponent<Collider>();
            health = GetComponent<IHealth>();
            bossMovement = GetComponent<BossMovement>();
            bossCombat = GetComponent<BossCombat>();

            InitializeStates();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            bdp = FindAnyObjectByType<BossDeathPortal>();

            if (health != null)
            {
                health.OnHealthChanged += (cur, max) => OnHealthChanged?.Invoke(cur, max);
                health.OnDeath += HandleDeath;
                SubscribeBossHealthEvents();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SubscribeToPeacefulMode(OnPeacefulModeChanged);
            }

            CreateHealthBar();
            TransitionToState(BossState.Idle);
        }

        private void SubscribeBossHealthEvents()
        {
            if (health is BossHealth bossHealth)
            {
                bossHealth.OnEnrage += HandleEnrage;
                bossHealth.OnStunned += HandleStunned;
            }
        }

        private void Update()
        {
            if (health != null && health.IsDead) return;

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
            IsEnraged = true;
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
            bdp?.ActivatePortal();
            Destroy(gameObject);
        }

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
        public void PerformRangedAttack() => bossCombat?.PerformRangedAttack();
        public void PerformHeavyMeleeAttack() => bossCombat?.PerformHeavyMeleeAttack();
        public void PerformHeavyRangedAttack() => bossCombat?.PerformHeavyRangedAttack();
        public void LaunchProjectile() => bossCombat?.LaunchProjectile(bossCombat.CalculateDamage(), false);
        public void LaunchProjectile(float damage) => bossCombat?.LaunchProjectile(damage, false);
        public bool ShouldFlee() => health is BossHealth bh && bh.ShouldFlee;
        public bool IsAgentReady() => bossMovement?.IsAgentReady() ?? false;

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
            health?.TakeDamage(damage);
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
            if (health != null && health.IsDead) return;

            currentState?.Exit(this);
            currentStateType = newState;
            currentState = states[newState];
            currentState?.Enter(this);
        }

        private void CreateHealthBar()
        {
            if (healthBarPrefab != null && healthBarPosition != null)
            {
                GameObject obj = Instantiate(healthBarPrefab, healthBarPosition.position, Quaternion.identity, healthBarPosition);
                obj.transform.localRotation = Quaternion.identity;

                healthSlider = obj.GetComponentInChildren<Slider>();
                if (healthSlider != null)
                {
                    healthSlider.maxValue = health?.MaxHealth ?? 100;
                    healthSlider.value = health?.CurrentHealth ?? 100;
                }

                healthText = obj.GetComponentInChildren<Text>();

                if (health != null)
                {
                    health.OnHealthChanged += UpdateHealthUI;
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

        public float AttackTimer
        {
            get => bossCombat?.AttackTimer ?? 0;
            set => bossCombat?.SetAttackTimer(value);
        }

        public void SpawnHitEffect(Vector3 position)
        {
            Debug.Log($"SpawnHitEffect at {position}");
        }
        public void SetAttackTimer(float value) => bossCombat?.SetAttackTimer(value);
    }
}