using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemy : EnemyBase
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float patrolRadius = 10f;

    [Header("Combat")]
    public float magicDamage = 15f;
    public float attackRange = 15f;
    public float preferredDistance = 12f;
    public float attackCooldown = 3f;
    public GameObject birdPrefab;
    public float birdSpeed = 20f;

    [Header("References")]
    public Transform player;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start(); // גûחûגאוע EnemyBase.Start()

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = preferredDistance * 0.8f;
        agent.updateRotation = true;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating(nameof(SetNewPatrolTarget), 0f, 5f);
    }

    void Update()
    {
        if (player == null || isAttacking || isDying || isHit) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (distanceToPlayer < preferredDistance - 2f)
            {
                Vector3 awayFromPlayer = transform.position - player.position;
                awayFromPlayer.y = 0;
                Vector3 retreatPoint = transform.position + awayFromPlayer.normalized * 5f;

                agent.isStopped = false;
                agent.SetDestination(retreatPoint);
            }
            else if (distanceToPlayer > preferredDistance + 2f)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                AttackPlayer();
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
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
            StartCoroutine(PerformRangedAttack());
        }
    }

    IEnumerator PerformRangedAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        if (birdPrefab != null && !isDying)
        {
            Vector3 spawnPos = transform.position + transform.forward * 2f + Vector3.up * 1.5f;

            GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
            bird.tag = "EnemyProjectile";

            Rigidbody rb = bird.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = bird.AddComponent<Rigidbody>();
            }

            rb.mass = 0.2f;
            rb.drag = 0.1f;
            rb.angularDrag = 0.1f;
            rb.useGravity = true;

            if (bird.GetComponent<Collider>() == null)
            {
                SphereCollider col = bird.AddComponent<SphereCollider>();
                PhysicsMaterial mat = new PhysicsMaterial();
                mat.bounciness = 0.4f;
                col.material = mat;
            }

            MagicProjectile projScript = bird.GetComponent<MagicProjectile>();
            if (projScript == null)
            {
                projScript = bird.AddComponent<MagicProjectile>();
            }
            projScript.damage = magicDamage;

            Vector3 targetPos = player.position + Vector3.up * 0.8f;
            Vector3 toTarget = targetPos - spawnPos;
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
            float flightTime = horizontalDistance / birdSpeed;
            float verticalVelocity = (toTarget.y + 0.5f * Mathf.Abs(Physics.gravity.y) * flightTime * flightTime) / flightTime;

            Vector3 horizontalDirection = new Vector3(toTarget.x, 0, toTarget.z).normalized;
            Vector3 horizontalVelocity = horizontalDirection * birdSpeed;

            rb.velocity = horizontalVelocity + Vector3.up * verticalVelocity;
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);
    }
}