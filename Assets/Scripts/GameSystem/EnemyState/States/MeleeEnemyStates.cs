using UnityEngine;
using System.Collections;
using Game.Core;

namespace Game.Enemy
{
    public class MeleeAttackState : AttackState
    {
        private MeleeEnemy meleeEnemy;

        public override void Enter(EnemyController enemy)
        {
            base.Enter(enemy);
            meleeEnemy = enemy as MeleeEnemy;
        }

        protected override IEnumerator PerformAttack(EnemyController enemy)
        {
            if (meleeEnemy == null)
            {
                enemy.SetStateChanging(false);
                yield break;
            }

            if (meleeEnemy.GetAttackType() == MeleeAttackType.Push)
            {
                yield return enemy.StartCoroutine(PerformPushAttack(meleeEnemy));
            }
            else
            {
                yield return enemy.StartCoroutine(PerformJumpAttack(meleeEnemy));
            }

            enemy.SetStateChanging(false);

            if (!enemy.IsDying && enemy.CurrentStateType == EnemyState.Attack)
            {
                if (enemy.DistanceToPlayer > enemy.attackRange)
                {
                    enemy.TransitionToState(EnemyState.Chase);
                }
            }
        }

        private IEnumerator PerformPushAttack(MeleeEnemy enemy)
        {
            enemy.SetStateChanging(true);

            yield return new WaitForSeconds(0.3f);

            float currentDistance = enemy.DistanceToPlayer;
            if (currentDistance <= enemy.attackRange + 0.5f && !enemy.IsDying)
            {
                HealthComponent playerHealth = enemy.Player.GetComponent<HealthComponent>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(enemy.GetCurrentDamage());
                    Debug.Log("Нанесён удар толчком!");
                }

                Rigidbody playerRb = enemy.Player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDirection = (enemy.Player.position - enemy.transform.position).normalized;
                    pushDirection.y = 0.5f;
                    playerRb.AddForce(pushDirection * enemy.pushForce, ForceMode.Impulse);
                }

                if (enemy.pushLandEffect != null)
                {
                    GameObject effect = UnityEngine.Object.Instantiate(enemy.pushLandEffect, enemy.transform.position, Quaternion.identity);
                    UnityEngine.Object.Destroy(effect, 2f);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator PerformJumpAttack(MeleeEnemy enemy)
        {
            enemy.SetStateChanging(true);

            yield return new WaitForSeconds(0.2f);

            Vector3 startPos = enemy.transform.position;
            Vector3 targetPos = enemy.Player.position;
            targetPos.y = startPos.y;

            float distanceToPlayer = Vector3.Distance(startPos, targetPos);

            if (distanceToPlayer > enemy.attackRange * 2)
            {
                Debug.Log($"{enemy.gameObject.name}: игрок слишком далеко для прыжка");
                enemy.SetStateChanging(false);
                yield break;
            }

            if (enemy.Agent != null) enemy.Agent.enabled = false;

            Vector3 jumpTop = startPos + Vector3.up * enemy.jumpHeight;
            float jumpDuration = 0.4f;
            float elapsed = 0;

            while (elapsed < jumpDuration)
            {
                float t = elapsed / jumpDuration;
                enemy.transform.position = Vector3.Lerp(startPos, jumpTop, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0;
            float fallDuration = 0.3f;
            Vector3 fallStart = jumpTop;

            while (elapsed < fallDuration)
            {
                float t = elapsed / fallDuration;
                Vector3 currentTarget = Vector3.Lerp(targetPos, enemy.Player.position, t);
                currentTarget.y = Mathf.Lerp(fallStart.y, targetPos.y, t);
                enemy.transform.position = currentTarget;
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (enemy.jumpLandEffect != null)
            {
                GameObject effect = UnityEngine.Object.Instantiate(enemy.jumpLandEffect, enemy.transform.position, Quaternion.identity);
                UnityEngine.Object.Destroy(effect, 2f);
            }

            float finalDistance = Vector3.Distance(enemy.transform.position, enemy.Player.position);
            if (finalDistance <= enemy.attackRange + 1f && !enemy.IsDying)
            {
                HealthComponent playerHealth = enemy.Player.GetComponent<HealthComponent>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(enemy.GetCurrentDamage());
                    Debug.Log($"{enemy.gameObject.name} прыгнул на игрока! Урон {enemy.GetCurrentDamage()}");
                }

                Rigidbody playerRb = enemy.Player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDirection = (enemy.Player.position - enemy.transform.position).normalized;
                    pushDirection.y = 0.5f;
                    playerRb.AddForce(pushDirection * 8f, ForceMode.Impulse);
                }
            }

            if (enemy.Agent != null)
            {
                enemy.Agent.enabled = true;
                enemy.Agent.Warp(enemy.transform.position);
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
}