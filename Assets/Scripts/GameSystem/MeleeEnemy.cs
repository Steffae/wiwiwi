using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeEnemy : EnemyStateMachine
{
    [Header("Melee Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float physicalDamage = 10f;
    public float pushForce = 5f;

    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;

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

        // Атака с кулдауном
        if (Time.time > lastAttackTime + attackCooldown && !isAttacking && !isDying)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformPhysicalAttack());
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

    IEnumerator PerformPhysicalAttack()
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
                playerHealth.TakeDamage(physicalDamage);
                Debug.Log("Нанесён удар ближнего боя");
            }

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (player.position - transform.position).normalized;
                pushDirection.y = 0.5f;
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(0.5f);
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