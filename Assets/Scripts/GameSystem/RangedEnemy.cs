using UnityEngine;
using UnityEngine.AI;
using Game.Enemy;

public enum RangedAttackType
{
    Bird,
    Octopus
}

public class RangedEnemy : EnemyController
{
    [Header("Ranged Settings")]
    public float attackRangeRanged = 15f;
    public float minDistance = 8f;
    public float maxDistance = 12f;

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

    private float currentDamage;
    private GameObject currentProjectile;
    private float currentSpeed;

    protected override void Awake()
    {
        base.Awake();

        chaseRange = attackRangeRanged;
        attackRange = attackRangeRanged;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 1f;
        }

        RegisterCustomStates();
    }

    protected override void Start()
    {
        base.Start();

        if (attackType == RangedAttackType.Bird)
        {
            currentDamage = birdDamage;
            currentProjectile = birdPrefab;
            currentSpeed = birdSpeed;
            this.attackCooldown = 2f;
            Debug.Log($"{gameObject.name}: атака - Птичка (урон {currentDamage})");
        }
        else
        {
            currentDamage = octopusDamage;
            currentProjectile = octopusPrefab;
            currentSpeed = octopusSpeed;
            this.attackCooldown = 2f;
            Debug.Log($"{gameObject.name}: атака - Осьминог (урон {currentDamage})");
        }
    }

    private void RegisterCustomStates()
    {
        RegisterState(EnemyState.Attack, new RangedAttackState());
        RegisterState(EnemyState.Chase, new RangedChaseState());
    }

    public override bool CanAttack()
    {
        if (IsPeacefulMode) return false;
        return DistanceToPlayer <= attackRange && DistanceToPlayer >= minDistance;
    }

    public RangedAttackType GetAttackType() => attackType;

    public float GetCurrentDamage() => currentDamage;

    public GameObject GetCurrentProjectile() => currentProjectile;

    public float GetCurrentSpeed() => currentSpeed;

    public void SetAttackType(RangedAttackType type)
    {
        attackType = type;
        if (attackType == RangedAttackType.Bird)
        {
            currentDamage = birdDamage;
            currentProjectile = birdPrefab;
            currentSpeed = birdSpeed;
            this.attackCooldown = 2f;
        }
        else
        {
            currentDamage = octopusDamage;
            currentProjectile = octopusPrefab;
            currentSpeed = octopusSpeed;
            this.attackCooldown = 2f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRangeRanged);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}