using UnityEngine;
using UnityEngine.AI;
using Game.Enemy;

public enum MeleeAttackType
{
    Push,
    Jump
}

public class MeleeEnemy : EnemyController
{
    [Header("Melee Settings")]
    public float detectionRange = 8f;
    public float attackRangeMelee = 2f;
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

    private float currentPhysicalDamage;
    private Coroutine attackCoroutine;

    public float CurrentDamage => currentPhysicalDamage;

    protected override void Awake()
    {
        base.Awake();

        chaseRange = detectionRange;
        attackRange = attackRangeMelee;

        if (agent != null)
        {
            agent.stoppingDistance = attackRange * 0.8f;
        }

        RegisterCustomStates();
    }

    protected override void Start()
    {
        base.Start();

        if (attackType == MeleeAttackType.Push)
        {
            currentPhysicalDamage = pushDamage;
            this.attackCooldown = 2f;
            Debug.Log($"{gameObject.name}: атака - Толчок (урон {currentPhysicalDamage})");
        }
        else
        {
            currentPhysicalDamage = jumpDamage;
            this.attackCooldown = jumpCooldown;
            Debug.Log($"{gameObject.name}: атака - Прыжок (урон {currentPhysicalDamage})");
        }
    }

    private void RegisterCustomStates()
    {
        RegisterState(EnemyState.Attack, new MeleeAttackState());
    }

    public override bool CanAttack()
    {
        if (IsPeacefulMode) return false;
        return DistanceToPlayer <= attackRange;
    }

    public MeleeAttackType GetAttackType() => attackType;

    public float GetCurrentDamage() => currentPhysicalDamage;

    public void SetAttackType(MeleeAttackType type)
    {
        attackType = type;

        if (attackType == MeleeAttackType.Push)
        {
            currentPhysicalDamage = pushDamage;
            this.attackCooldown = 2f;
        }
        else
        {
            currentPhysicalDamage = jumpDamage;
            this.attackCooldown = jumpCooldown;
        }

        Debug.Log($"{gameObject.name}: тип атаки изменён на {attackType}, урон {currentPhysicalDamage}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRangeMelee);
    }
}