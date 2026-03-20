using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeEnemy : EnemyBase
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float patrolRadius = 10f;
    public float detectionRange = 8f;

    [Header("Combat")]
    public float physicalDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float pushForce = 5f;

    [Header("References")]
    public Transform player;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start(); // вызывает EnemyBase.Start()

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
        agent.updateRotation = true;

        //if (player == null)
            //player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating(nameof(SetNewPatrolTarget), 0f, 5f);
    }

    void Update()
    {
        if (player == null || isAttacking || isDying || isHit) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(patrolTarget);
        }
    }

    void SetNewPatrolTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection.y = 0;
            Vector3 randomPoint = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                return;
            }
        }
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            lastAttackTime = Time.time;

            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            StartCoroutine(PerformPhysicalAttack());
        }
    }

    IEnumerator PerformPhysicalAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.3f);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f && !isDying)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(physicalDamage);
                Debug.Log("Ближний враг толкнул игрока");
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