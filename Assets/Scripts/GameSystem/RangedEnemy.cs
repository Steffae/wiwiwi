using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    public float attackRange = 15f;
    public float minDistance = 8f;      // ���� ����� � ���������
    public float maxDistance = 12f;     // ���� ������ � ��������
    public float attackCooldown = 2f;
    public float magicDamage = 15f;
    public GameObject birdPrefab;
    public float birdSpeed = 25f;

    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;
    private bool isMoving = false;

    protected override void Awake()
    {
        base.Awake();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 1f;
        }

        InvokeRepeating(nameof(SetNewPatrolTarget), 0f, 5f);
    }

    void Update()
    {
        if (player == null || isAttacking || isDying || isHit) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ������������ �������� �����
        if (distanceToPlayer <= attackRange)
        {

            if (distanceToPlayer < minDistance)
            {
                // ������� ������
                Vector3 awayFromPlayer = (transform.position - player.position).normalized;
                awayFromPlayer.y = 0;
                Vector3 retreatPoint = transform.position + awayFromPlayer * 5f;

                agent.SetDestination(retreatPoint);
                agent.isStopped = false;
                isMoving = true;
                Debug.Log($"RangedEnemy: ��������! dist={distanceToPlayer}");
            }
            else if (distanceToPlayer > maxDistance)
            {
                // ������� ������
                agent.SetDestination(player.position);
                agent.isStopped = false;
                isMoving = true;
                Debug.Log($"RangedEnemy: �������! dist={distanceToPlayer}");
            }
            else
            {
                // ��������� ������
                if (isMoving)
                {
                    agent.isStopped = true;
                    isMoving = false;
                }

                // ������� � ������
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDirection);

                AttackPlayer();
            }
        }
        else
        {
            // ������ �����
            agent.isStopped = false;
            agent.SetDestination(patrolTarget);
            isMoving = true;
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
            Vector3 spawnPos = transform.position + transform.forward * 2.5f + Vector3.up * 1.5f;

            Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject == gameObject)
                {
                    spawnPos = transform.position + transform.forward * 3f + Vector3.up * 1.5f;
                    break;
                }
            }

            GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
            bird.tag = "EnemyProjectile";

            Rigidbody rb = bird.GetComponent<Rigidbody>();
            if (rb == null) rb = bird.AddComponent<Rigidbody>();

            // ��������� ������
            rb.mass = 0.2f;
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            if (bird.GetComponent<Collider>() == null)
            {
                SphereCollider col = bird.AddComponent<SphereCollider>();
                col.radius = 0.3f;
                col.material = new PhysicsMaterial();
                col.material.bounciness = 0.5f;
            }

            MagicProjectile projScript = bird.GetComponent<MagicProjectile>();
            if (projScript == null) projScript = bird.AddComponent<MagicProjectile>();
            projScript.damage = magicDamage;

            Vector3 directionToPlayer = (player.position - spawnPos).normalized;
            float speed = 15f;
            rb.linearVelocity = directionToPlayer * speed;
            //rb.AddForce(directionToPlayer * 15f, ForceMode.Impulse);

            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            // ���������� ������������ � ������
            Collider enemyCollider = GetComponent<Collider>();
            if (enemyCollider != null && bird.GetComponent<Collider>() != null)
            {
                Physics.IgnoreCollision(enemyCollider, bird.GetComponent<Collider>(), true);
            }

            Debug.Log($"������ �������� �� {spawnPos} � ����������� {directionToPlayer}");
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
}