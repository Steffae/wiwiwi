using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    public float attackRange = 15f;
    public float minDistance = 8f;      // Минимальная дистанция (если ближе — отступаем)
    public float maxDistance = 12f;     // Максимальная дистанция (если дальше — подходим)
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

        // ЛОГИКА ПЕРЕДВИЖЕНИЯ
        if (distanceToPlayer <= attackRange)
        {
            // Враг видит игрока

            if (distanceToPlayer < minDistance)
            {
                // СЛИШКОМ БЛИЗКО — ОТСТУПАЕМ
                Vector3 awayFromPlayer = (transform.position - player.position).normalized;
                awayFromPlayer.y = 0;
                Vector3 retreatPoint = transform.position + awayFromPlayer * 5f;

                agent.SetDestination(retreatPoint);
                agent.isStopped = false;
                isMoving = true;
                Debug.Log($"RangedEnemy: ОТСТУПАЮ! dist={distanceToPlayer}");
            }
            else if (distanceToPlayer > maxDistance)
            {
                // СЛИШКОМ ДАЛЕКО — ПОДХОДИМ
                agent.SetDestination(player.position);
                agent.isStopped = false;
                isMoving = true;
                Debug.Log($"RangedEnemy: ПОДХОЖУ! dist={distanceToPlayer}");
            }
            else
            {
                // НА ПРАВИЛЬНОЙ ДИСТАНЦИИ — СТОЮ И СТРЕЛЯЮ
                if (isMoving)
                {
                    agent.isStopped = true;
                    isMoving = false;
                }

                // Поворачиваемся к игроку
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDirection);

                // Атакуем
                AttackPlayer();
            }
        }
        else
        {
            // ИГРОК НЕ ВИДЕН — ПАТРУЛИРУЕМ
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
            // Точка вылета — ДАЛЬШЕ от врага, чтобы не столкнуться с ним
            Vector3 spawnPos = transform.position + transform.forward * 2.5f + Vector3.up * 1.5f;

            // Проверка: не спавнится ли внутри врага?
            Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
            foreach (var hit in hitColliders)
            {
                if (hit.gameObject == gameObject)
                {
                    // Если спавнится внутри себя — смещаем ещё дальше
                    spawnPos = transform.position + transform.forward * 3f + Vector3.up * 1.5f;
                    break;
                }
            }

            GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
            bird.tag = "EnemyProjectile";

            Rigidbody rb = bird.GetComponent<Rigidbody>();
            if (rb == null) rb = bird.AddComponent<Rigidbody>();

            // Настройки физики
            rb.mass = 0.2f;
            rb.drag = 0.05f;      // Минимальное сопротивление
            rb.angularDrag = 0.05f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Добавляем коллайдер
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

            // ===== СИЛЬНЫЙ БРОСОК =====
            Vector3 directionToPlayer = (player.position - spawnPos).normalized;

            // Большая скорость
            float speed = 15f;
            rb.velocity = directionToPlayer * speed;
            //rb.AddForce(directionToPlayer * 15f, ForceMode.Impulse);

            // Добавляем вращение
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            // Игнорируем столкновение с врагом, который выпустил птичку
            Collider enemyCollider = GetComponent<Collider>();
            if (enemyCollider != null && bird.GetComponent<Collider>() != null)
            {
                Physics.IgnoreCollision(enemyCollider, bird.GetComponent<Collider>(), true);
            }

            Debug.Log($"Птичка вылетела из {spawnPos} в направлении {directionToPlayer}");
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