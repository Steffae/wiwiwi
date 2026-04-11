using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum RangedAttackType
{
    Bird,      // Птичка
    Octopus    // Осьминог
}

public class RangedEnemy : EnemyStateMachine
{
    [Header("Ranged Settings")]
    public float attackRange = 15f;
    public float minDistance = 8f;
    public float maxDistance = 12f;
    public float attackCooldown = 2f;

    [Header("Attack Type")]
    public RangedAttackType attackType = RangedAttackType.Bird;

    [Header("Bird Attack Settings")]
    public float birdDamage = 15f;
    public GameObject birdPrefab;
    public float birdSpeed = 25f;

    [Header("Octopus Attack Settings")]
    public float octopusDamage = 25f;
    public GameObject octopusPrefab;
    public float octopusSpeed = 20f;

    [Header("Effects")]
    public GameObject hitEffect;

    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;
    private bool isMoving = false;
    private float currentDamage;
    private GameObject currentProjectile;
    private float currentSpeed;

    protected override void Awake()
    {
        base.Awake();

        chaseRange = attackRange;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 1f;
        }

        InvokeRepeating(nameof(SetNewPatrolTarget), 0f, 5f);
    }

    protected override void Start()
    {
        base.Start();

        // Устанавливаем параметры в зависимости от типа атаки
        if (attackType == RangedAttackType.Bird)
        {
            currentDamage = birdDamage;
            currentProjectile = birdPrefab;
            currentSpeed = birdSpeed;
            Debug.Log($"{gameObject.name}: атака - Птичка (урон {currentDamage})");
        }
        else
        {
            currentDamage = octopusDamage;
            currentProjectile = octopusPrefab;
            currentSpeed = octopusSpeed;
            Debug.Log($"{gameObject.name}: атака - Осьминог (урон {currentDamage})");
        }
    }

    // Переключение состояний

    protected override bool CanAttack()
    {
        // В мирном режиме не атакуем
        if (isPeacefulMode)
        {
            return false;
        }

        return distanceToPlayer <= attackRange && distanceToPlayer >= minDistance;
    }

    protected override void UpdateState()
    {
        if (isPeacefulMode)
        {
            if (currentState != EnemyState.Idle)
            {
                SwitchState(EnemyState.Idle);
            }
            return;
        }

        // Проверка дистанции
        if (distanceToPlayer <= attackRange)
        {
            // Слишком близко — отступаем
            if (distanceToPlayer < minDistance)
            {
                if (currentState != EnemyState.Chase)
                    SwitchState(EnemyState.Chase);
                return;
            }

            // На дистанции атаки
            if (distanceToPlayer <= maxDistance && distanceToPlayer >= minDistance)
            {
                if (currentState != EnemyState.Attack)
                    SwitchState(EnemyState.Attack);
                return;
            }

            // Слишком далеко — сближаемся
            if (distanceToPlayer > maxDistance)
            {
                if (currentState != EnemyState.Chase)
                    SwitchState(EnemyState.Chase);
                return;
            }
        }

        // Патруль
        if (distanceToPlayer > attackRange)
        {
            if (currentState != EnemyState.Idle)
                SwitchState(EnemyState.Idle);
        }
    }

    // Поведения состояний

    protected override void IdleBehavior()
    {
        // Патрулирование
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(patrolTarget);
            isMoving = true;
        }
    }

    protected override void ChaseBehavior()
    {
        if (distanceToPlayer < minDistance)
        {
            // Отступаем назад
            Vector3 awayFromPlayer = (transform.position - targetPlayer.position).normalized;
            awayFromPlayer.y = 0;
            Vector3 retreatPoint = transform.position + awayFromPlayer * 5f;

            agent.SetDestination(retreatPoint);
            agent.isStopped = false;
            isMoving = true;
        }
        else if (distanceToPlayer > maxDistance)
        {
            // Сближаемся
            agent.SetDestination(targetPlayer.position);
            agent.isStopped = false;
            isMoving = true;
        }
        else
        {
            // Атаковать
            if (currentState != EnemyState.Attack)
                SwitchState(EnemyState.Attack);
        }
    }

    protected override void AttackBehavior()
    {
        // Останавливаемся
        if (isMoving)
        {
            agent.isStopped = true;
            isMoving = false;
        }

        // Поворачиваемся к игроку
        Vector3 lookDirection = targetPlayer.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        // Атака с кулдауном
        if (Time.time > lastAttackTime + attackCooldown && !isAttacking && !isDying)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformRangedAttack());
        }
    }

    void SetNewPatrolTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection.y = 0;
            Vector3 randomPoint = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                return;
            }
        }
    }

    IEnumerator PerformRangedAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        if (currentProjectile != null && !isDying)
        {
            Vector3 spawnPos = transform.position + transform.forward * 2.5f + Vector3.up * 1.5f;

            // Проверка, не спавнится ли внутри врага
            Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject == gameObject)
                {
                    spawnPos = transform.position + transform.forward * 3f + Vector3.up * 1.5f;
                    break;
                }
            }

            GameObject projectile = Instantiate(currentProjectile, spawnPos, Quaternion.identity);
            projectile.tag = "EnemyProjectile";

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb == null) rb = projectile.AddComponent<Rigidbody>();

            // Настройка физики
            rb.mass = 0.1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (projectile.GetComponent<Collider>() == null)
            {
                SphereCollider col = projectile.AddComponent<SphereCollider>();
                col.radius = 0.3f;
                col.material = new PhysicsMaterial();
                col.material.bounciness = 0.5f;
            }

            // Добавляем скрипт снаряда
            MagicProjectile projScript = projectile.GetComponent<MagicProjectile>();
            if (projScript == null) projScript = projectile.AddComponent<MagicProjectile>();
            projScript.damage = currentDamage;
            projScript.hitEffect = hitEffect;

            Vector3 directionToPlayer = (targetPlayer.position - spawnPos).normalized;
            rb.linearVelocity = directionToPlayer * currentSpeed;
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            // Игнорируем столкновения с врагом
            Collider enemyCollider = GetComponent<Collider>();
            if (enemyCollider != null && projectile.GetComponent<Collider>() != null)
            {
                Physics.IgnoreCollision(enemyCollider, projectile.GetComponent<Collider>(), true);
            }

            Debug.Log($"Снаряд ({attackType}) создан на {spawnPos} с направлением {directionToPlayer}");
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    public RangedAttackType GetAttackType()
    {
        return attackType;
    }

    public float GetCurrentDamage()
    {
        return currentDamage;
    }

    public void SetAttackType(RangedAttackType type)
    {
        attackType = type;
        // Обновляем параметры в зависимости от типа
        if (attackType == RangedAttackType.Bird)
        {
            currentDamage = birdDamage;
            currentProjectile = birdPrefab;
            currentSpeed = birdSpeed;
        }
        else
        {
            currentDamage = octopusDamage;
            currentProjectile = octopusPrefab;
            currentSpeed = octopusSpeed;
        }
    }
}