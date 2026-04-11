using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum MeleeAttackType
{
    Push,    // Толчок
    Jump     // Прыжок на голову
}

public class MeleeEnemy : EnemyStateMachine
{
    [Header("Melee Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float pushDamage = 10f;
    public float pushForce = 5f;

    [Header("Attack Type")]
    public MeleeAttackType attackType = MeleeAttackType.Push;

    [Header("Jump Attack Settings")]
    public float jumpDamage = 30f;
    public float jumpCooldown = 4f;
    public float jumpHeight = 3f;

    [Header("Effects")]
    public GameObject jumpLandEffect;
    public GameObject pushLandEffect;

    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;
    private float currentPhysicalDamage;

    protected override void Awake()
    {
        base.Awake();

        chaseRange = detectionRange;

        if (agent != null)
        {
            agent.stoppingDistance = attackRange * 0.8f;
        }

        InvokeRepeating(nameof(SetNewPatrolTarget), 0f, 5f);
    }

    protected override void Start()
    {
        base.Start();

        // Устанавливаем урон в зависимости от типа атаки
        if (attackType == MeleeAttackType.Push)
        {
            currentPhysicalDamage = pushDamage;
            Debug.Log($"{gameObject.name}: атака - Толчок (урон {currentPhysicalDamage})");
        }
        else
        {
            currentPhysicalDamage = jumpDamage;
            Debug.Log($"{gameObject.name}: атака - Прыжок (урон {currentPhysicalDamage})");
        }
    }

    // Переключение состояний

    protected override bool CanAttack()
    {
        // В мирном режиме не атакуем
        if (isPeacefulMode) return false;

        return distanceToPlayer <= attackRange;
    }

    // Поведения состояний

    protected override void IdleBehavior()
    {
        // Патрулирование
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;

            if (Vector3.Distance(transform.position, patrolTarget) < 0.5f)
            {
                SetNewPatrolTarget();
            }
            agent.SetDestination(patrolTarget);
        }

        // Анимация
        if (animator != null)
            animator.SetFloat("Speed", 0.1f);
    }

    protected override void ChaseBehavior()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        if (animator != null)
            animator.SetFloat("Speed", 1f);
    }

    protected override void AttackBehavior()
    {
        // Останавливаемся
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // Поворачиваемся к игроку
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);

        // Атака с кулдауном в зависимости от типа
        float currentCooldown = (attackType == MeleeAttackType.Jump) ? jumpCooldown : attackCooldown;

        if (Time.time > lastAttackTime + currentCooldown && !isAttacking && !isDying)
        {
            lastAttackTime = Time.time;

            if (attackType == MeleeAttackType.Push)
            {
                StartCoroutine(PerformPushAttack());
            }
            else
            {
                StartCoroutine(PerformJumpAttack());
            }
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

    // Атака 1 - толчок
    IEnumerator PerformPushAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.3f);

        // Проверяем, что игрок всё ещё рядом
        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance <= attackRange + 0.5f && !isDying)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(currentPhysicalDamage);
                Debug.Log("Нанесён удар толчком!");
            }

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (player.position - transform.position).normalized;
                pushDirection.y = 0.5f;
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Эффекты
            if (jumpLandEffect != null)
            {
                GameObject effect = Instantiate(pushLandEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f); // Уничтожаем через 2 секунды
                Debug.Log("Эффект приземления создан!");
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // Атака 2 - прыжок на голову
    IEnumerator PerformJumpAttack()
    {
        isAttacking = true;

        // Подготовка к прыжку
        yield return new WaitForSeconds(0.2f);

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.position;
        targetPos.y = startPos.y;

        float distanceToPlayer = Vector3.Distance(startPos, targetPos);

        // Если игрок слишком далеко — не прыгаем
        if (distanceToPlayer > attackRange * 2)
        {
            Debug.Log($"{gameObject.name}: игрок слишком далеко для прыжка");
            isAttacking = false;
            yield break;
        }

        // Отключаем навигацию во время прыжка
        if (agent != null) agent.enabled = false;

        // Подъем вверх
        Vector3 jumpTop = startPos + Vector3.up * jumpHeight;
        float jumpDuration = 0.4f;
        float elapsed = 0;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            transform.position = Vector3.Lerp(startPos, jumpTop, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Падение на игрока
        elapsed = 0;
        float fallDuration = 0.3f;
        Vector3 fallStart = jumpTop;

        while (elapsed < fallDuration)
        {
            float t = elapsed / fallDuration;
            Vector3 currentTarget = Vector3.Lerp(targetPos, player.position, t);
            currentTarget.y = Mathf.Lerp(fallStart.y, targetPos.y, t);
            transform.position = currentTarget;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Эффекты
        if (jumpLandEffect != null)
        {
            GameObject effect = Instantiate(jumpLandEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Уничтожаем через 2 секунды
            Debug.Log("Эффект приземления создан!");
        }

        // Нанесение урона
        float finalDistance = Vector3.Distance(transform.position, player.position);
        if (finalDistance <= attackRange + 1f && !isDying)
        {
            HealthComponent playerHealth = player.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(currentPhysicalDamage);
                Debug.Log($"{gameObject.name} прыгнул на игрока! Урон {currentPhysicalDamage}");
            }

            // Эффект отбрасывания при приземлении
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (player.position - transform.position).normalized;
                pushDirection.y = 0.5f;
                playerRb.AddForce(pushDirection * 8f, ForceMode.Impulse);
            }

            
        }

        // Возврат к агенту
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}