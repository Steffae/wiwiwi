using Game.Core;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,      // Патруль
    Chase,     // Преследование
    Attack,    // Атака
    Flee       // Бегство
}

public abstract class EnemyStateMachine : EnemyBase
{
    [Header("State Machine")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Chase Settings")]
    public float chaseRange = 10f;

    [Header("Flee Settings")]
    public float fleeHealthPercent = 30f;
    public float fleeDistance = 15f;
    public float fleeSpeed = 5f;
    protected float distanceToPlayer;
    protected bool isStateChanging = false;
    protected bool isPeacefulMode = false;

    protected override void Awake()
    {
        base.Awake();
        targetPlayer = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Start()
    {
        base.Start(); // Вызов базового метода из EnemyBase

        // Получаем режим
        if (GameManager.Instance != null)
        {
            isPeacefulMode = GameManager.Instance.IsPeacefulMode;
        }
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

        // Если включили мирный режим во время атаки/преследования — останавливаемся
        if (isPeacefulMode && (currentState == EnemyState.Chase || currentState == EnemyState.Attack))
        {
            SwitchState(EnemyState.Idle);
        }
    }

    protected virtual void Update()
    {
        if (targetPlayer == null || isDying || isHit || isStateChanging) return;

        if (agent != null && !agent.isOnNavMesh) return;

        distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        UpdateState();

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Chase:
                ChaseBehavior();
                break;
            case EnemyState.Attack:
                AttackBehavior();
                break;
            case EnemyState.Flee:
                FleeBehavior();
                break;
        }
    }

    protected bool ShouldFlee()
    {
        float healthPercent = (healthSystem.CurrentHealth / maxHealth) * 100f;
        return healthPercent <= fleeHealthPercent && !isDying;
    }

    protected virtual void UpdateState()
    {
        // Если включен мирный режим — не переходим в Chase и Attack
        if (isPeacefulMode)
        {
            // Проверяем бегство
            if (ShouldFlee())
            {
                if (currentState != EnemyState.Flee)
                    SwitchState(EnemyState.Flee);
                return;
            }

            // В мирном режиме только Idle и Flee
            if (currentState != EnemyState.Idle)
                SwitchState(EnemyState.Idle);
            return;
        }

        // Обычный режим
        if (ShouldFlee())
        {
            if (currentState != EnemyState.Flee)
                SwitchState(EnemyState.Flee);
            return;
        }

        if (CanAttack())
        {
            if (currentState != EnemyState.Attack)
                SwitchState(EnemyState.Attack);
        }
        else if (distanceToPlayer <= chaseRange)
        {
            if (currentState != EnemyState.Chase)
                SwitchState(EnemyState.Chase);
        }
        else
        {
            if (currentState != EnemyState.Idle)
                SwitchState(EnemyState.Idle);
        }
    }

    protected virtual bool CanAttack()
    {
        return false;
    }

    protected virtual void IdleBehavior() { }

    protected virtual void ChaseBehavior()
    {
        if (agent != null && agent.isActiveAndEnabled && !isDying)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }

        //if (animator != null)
        //animator.SetFloat("Speed", 1f);
    }

    protected virtual void AttackBehavior()
    {
        Debug.Log($"AttackBehavior: currentState={currentState}, distance={distanceToPlayer}");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Vector3 lookDirection = targetPlayer.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);

    }

    protected virtual void FleeBehavior()
    {
        Vector3 fleeDirection = (transform.position - targetPlayer.position).normalized;
        fleeDirection.y = 0;
        Vector3 fleePoint = transform.position + fleeDirection * fleeDistance;

        if (agent != null && agent.isActiveAndEnabled && !isDying)
        {
            agent.isStopped = false;
            agent.speed = fleeSpeed;
            agent.SetDestination(fleePoint);
        }

        //if (animator != null)
        //animator.SetFloat("Speed", 1.2f);
    }

    protected void SwitchState(EnemyState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(newState);

        Debug.Log($"{gameObject.name} ? {newState} (HP: {healthSystem.CurrentHealth}/{maxHealth})");
    }

    protected virtual void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Flee:
                Debug.Log($"{gameObject.name}: Бег HP {healthSystem.CurrentHealth}/{maxHealth}");
                break;
            case EnemyState.Attack:
                Debug.Log($"{gameObject.name}: Атака");
                break;
            case EnemyState.Chase:
                Debug.Log($"{gameObject.name}: Преследование");
                break;
            case EnemyState.Idle:
                Debug.Log($"{gameObject.name}: Патруль");
                break;
        }
    }

    protected virtual void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Flee:
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    agent.speed = moveSpeed;
                break;
            case EnemyState.Attack:
                if (agent != null && !isDying && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    agent.isStopped = false;
                break;
        }
    }
}