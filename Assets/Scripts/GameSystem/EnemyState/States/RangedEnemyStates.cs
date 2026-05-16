using UnityEngine;
using System.Collections;
using Game.Core;

namespace Game.Enemy
{
    public class RangedAttackState : AttackState
    {
        private RangedEnemy rangedEnemy;

        public override void Enter(EnemyController enemy)
        {
            base.Enter(enemy);
            rangedEnemy = enemy as RangedEnemy;
        }

        public override void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            if (enemy.ShouldFlee())
            {
                enemy.TransitionToState(EnemyState.Flee);
                return;
            }

            enemy.FacePlayer();

            if (rangedEnemy == null)
            {
                if (enemy.DistanceToPlayer > enemy.attackRange)
                {
                    enemy.TransitionToState(EnemyState.Chase);
                }
                return;
            }

            float distance = enemy.DistanceToPlayer;

            if (distance < rangedEnemy.minDistance)
            {
                enemy.TransitionToState(EnemyState.Chase);
                return;
            }

            if (Time.time > lastAttackTime + enemy.attackCooldown && !enemy.IsStateChanging)
            {
                lastAttackTime = Time.time;
                enemy.SetStateChanging(true);
                enemy.StartCoroutine(PerformRangedAttack(enemy));
            }
        }

        private IEnumerator PerformRangedAttack(EnemyController enemy)
        {
            if (rangedEnemy == null)
            {
                enemy.SetStateChanging(false);
                yield break;
            }

            yield return new WaitForSeconds(0.2f);

            if (rangedEnemy.GetCurrentProjectile() != null && !enemy.IsDying)
            {
                Vector3 spawnPos = enemy.transform.position + enemy.transform.forward * 2.5f + Vector3.up * 1.5f;

                Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
                foreach (var hit in hitColliders)
                {
                    if (hit.gameObject == enemy.gameObject)
                    {
                        spawnPos = enemy.transform.position + enemy.transform.forward * 3f + Vector3.up * 1.5f;
                        break;
                    }
                }

                GameObject projectile = UnityEngine.Object.Instantiate(rangedEnemy.GetCurrentProjectile(), spawnPos, Quaternion.identity);
                projectile.tag = "EnemyProjectile";

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb == null) rb = projectile.AddComponent<Rigidbody>();

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

                MagicProjectile projScript = projectile.GetComponent<MagicProjectile>();
                if (projScript == null) projScript = projectile.AddComponent<MagicProjectile>();
                projScript.damage = rangedEnemy.GetCurrentDamage();
                projScript.hitEffect = rangedEnemy.hitEffect;

                Vector3 directionToPlayer = (enemy.Player.position - spawnPos).normalized;
                rb.linearVelocity = directionToPlayer * rangedEnemy.GetCurrentSpeed();
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null && projectile.GetComponent<Collider>() != null)
                {
                    Physics.IgnoreCollision(enemyCollider, projectile.GetComponent<Collider>(), true);
                }

                Debug.Log($"Снаряд ({rangedEnemy.GetAttackType()}) создан на {spawnPos}");
            }

            yield return new WaitForSeconds(1f);
            enemy.SetStateChanging(false);

            if (!enemy.IsDying && enemy.CurrentStateType == EnemyState.Attack)
            {
                if (enemy.DistanceToPlayer > rangedEnemy.maxDistance)
                {
                    enemy.TransitionToState(EnemyState.Chase);
                }
                else if (enemy.DistanceToPlayer < rangedEnemy.minDistance)
                {
                    enemy.TransitionToState(EnemyState.Chase);
                }
            }
        }

        public override void Exit(EnemyController enemy)
        {
            base.Exit(enemy);
            if (enemy.Agent != null && !enemy.IsDying && enemy.IsAgentReady())
            {
                enemy.Agent.isStopped = false;
            }
        }
    }

    public class RangedChaseState : IEnemyState
    {
        public void Enter(EnemyController enemy)
        {
            enemy.SafeSetAgentStopped(false);

            if (enemy.Agent != null)
            {
                enemy.Agent.speed = enemy.moveSpeed;
            }

            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", 1f);
            }
        }

        public void Update(EnemyController enemy)
        {
            if (enemy.Player == null || enemy.IsDying) return;

            RangedEnemy rangedEnemy = enemy as RangedEnemy;
            if (rangedEnemy == null)
            {
                if (enemy.ShouldFlee())
                {
                    enemy.TransitionToState(EnemyState.Flee);
                    return;
                }

                if (!enemy.IsPeacefulMode && enemy.CanAttack() && enemy.DistanceToPlayer <= enemy.attackRange)
                {
                    enemy.TransitionToState(EnemyState.Attack);
                    return;
                }

                if (enemy.Agent != null && enemy.IsAgentReady())
                {
                    enemy.SafeSetDestination(enemy.Player.position);
                }
                return;
            }

            float distance = enemy.DistanceToPlayer;

            if (enemy.ShouldFlee())
            {
                enemy.TransitionToState(EnemyState.Flee);
                return;
            }

            if (!enemy.IsPeacefulMode && distance <= rangedEnemy.maxDistance && distance >= rangedEnemy.minDistance)
            {
                enemy.TransitionToState(EnemyState.Attack);
                return;
            }

            if (distance < rangedEnemy.minDistance)
            {
                Vector3 awayFromPlayer = (enemy.transform.position - enemy.Player.position).normalized;
                awayFromPlayer.y = 0;
                Vector3 retreatPoint = enemy.transform.position + awayFromPlayer * 5f;
                enemy.SafeSetDestination(retreatPoint);
            }
            else if (distance > rangedEnemy.maxDistance)
            {
                enemy.SafeSetDestination(enemy.Player.position);
            }
        }

        public void Exit(EnemyController enemy)
        {
        }
    }
}