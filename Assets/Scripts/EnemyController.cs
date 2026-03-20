using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float patrolRadius = 10f;
    public float detectionRange = 8f;

    [Header("Combat")]
    public float physicalDamage = 10f;
    public float magicDamage = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float pushForce = 5f; // Сила толчка

    [Header("Magic")]
    public GameObject birdPrefab;  // Птичка для магии
    public float birdSpeed = 15f;

    [Header("References")]
    public Transform player;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 patrolTarget;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.8f;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        SetNewPatrolTarget();
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Атакуем
            agent.isStopped = true;
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Идём к игроку
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Патрулируем
            Patrol();
        }
    }

    void Patrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            SetNewPatrolTarget();
        }
        agent.SetDestination(patrolTarget);
    }

    void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0;
        patrolTarget = transform.position + randomDirection;
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            lastAttackTime = Time.time;

            // Поворачиваемся к игроку
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            // ВСЕГДА случайный выбор, без привязки к предыдущему
            float randomValue = Random.value;
            Debug.Log($"Выбор атаки: {randomValue}");

            if (randomValue > 0.5f)
            {
                Debug.Log("Выбрана ФИЗИЧЕСКАЯ атака");
                StartCoroutine(PerformPhysicalAttack());
            }
            else
            {
                Debug.Log("Выбрана МАГИЧЕСКАЯ атака");
                StartCoroutine(PerformMagicAttack());
            }
        }
    }

    System.Collections.IEnumerator PerformPhysicalAttack()
    {
        isAttacking = true;

        // Небольшая задержка перед толчком (как замах)
        yield return new WaitForSeconds(0.3f);

        // Проверяем, всё ещё ли игрок рядом
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            // Наносим урон
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(physicalDamage);
                Debug.Log("Враг толкнул игрока");
            }

            // Физический толчок (отбрасывание)
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (player.position - transform.position).normalized;
                pushDirection.y = 0.5f; // Немного вверх для эффекта
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    System.Collections.IEnumerator PerformMagicAttack()
    {
        isAttacking = true;

        // Небольшая задержка, как будто враг прицеливается
        yield return new WaitForSeconds(0.3f);

        if (birdPrefab != null)
        {
            // Точка вылета птички (на уровне груди врага)
            Vector3 spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;

            // Создаём птичку
            GameObject bird = Instantiate(birdPrefab, spawnPos, Quaternion.identity);

            // Настраиваем тег
            bird.tag = "EnemyProjectile";

            // Настройка физики как у мячика
            Rigidbody rb = bird.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = bird.AddComponent<Rigidbody>();
            }

            // Делаем птичку лёгкой и упругой
            rb.mass = 0.3f;          // Лёгкая
            rb.linearDamping = 0.2f;          // Малое сопротивление воздуха
            rb.angularDamping = 0.1f;    // Крутится легко
            rb.useGravity = true;     // Гравитация включена - будет лететь по дуге
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Плавное движение
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Чтоб не пролетала сквозь стены

            // Добавляем коллайдер, если нет
            if (bird.GetComponent<Collider>() == null)
            {
                SphereCollider col = bird.AddComponent<SphereCollider>();
                col.material = new PhysicsMaterial(); // Материал для отскоков
                col.material.bounciness = 0.6f;      // Отскок как у мячика
                col.material.dynamicFriction = 0.2f;
                col.material.staticFriction = 0.2f;
            }
            else
            {
                // Если коллайдер уже есть, настраиваем его физический материал
                Collider col = bird.GetComponent<Collider>();
                PhysicsMaterial mat = new PhysicsMaterial();
                mat.bounciness = 0.6f;
                mat.dynamicFriction = 0.2f;
                mat.staticFriction = 0.2f;
                col.material = mat;
            }

            // Добавляем скрипт снаряда (если нет)
            MagicProjectile projScript = bird.GetComponent<MagicProjectile>();
            if (projScript == null)
            {
                projScript = bird.AddComponent<MagicProjectile>();
            }
            projScript.damage = magicDamage;

            // Рассчитываем траекторию как у мячика
            // Цель - игрок, но чуть выше, чтоб летела по дуге
            Vector3 targetPos = player.position + Vector3.up * 0.8f; // Целимся в корпус

            // Рассчитываем скорость для дуги
            float gravity = Physics.gravity.y;
            Vector3 toTarget = targetPos - spawnPos;
            float timeToTarget = Vector3.Distance(spawnPos, targetPos) / birdSpeed; // Приблизительное время

            // Простая баллистика: v = (S - 0.5*g*t^2)/t
            Vector3 horizontal = new Vector3(toTarget.x, 0, toTarget.z);
            float horizontalDistance = horizontal.magnitude;

            // Добавляем немного случайности, чтоб не всегда идеально попадала
            horizontalDistance += Random.Range(-0.5f, 0.5f);

            // Время полёта зависит от горизонтальной дистанции
            float flightTime = horizontalDistance / birdSpeed;

            // Вертикальная скорость с учётом гравитации
            float verticalVelocity = (toTarget.y - 0.5f * gravity * flightTime * flightTime) / flightTime;

            // Итоговая скорость
            Vector3 velocity = horizontal.normalized * birdSpeed + Vector3.up * verticalVelocity;

            // Добавляем небольшой случайный разброс
            velocity += Random.insideUnitSphere * 1f;

            // Запускаем птичку
            rb.linearVelocity = velocity;

            // Добавляем вращение (чтобы крутилась в полёте)
            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

            Debug.Log("Птичка-мячик вылетела!");
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    void ResetAttack()
    {
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