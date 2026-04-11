using UnityEngine;
using UnityEngine.AI;

namespace Game.Boss
{
    public class BossMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        //[SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float enrageSpeedMultiplier = 1.5f;
        [SerializeField] private float rotationSpeed = 10f;

        private NavMeshAgent agent;
        private Transform player;
        private bool isEnraged = false;
        private float currentSpeed;

        public NavMeshAgent Agent => agent;
        public bool IsMoving => agent != null && agent.velocity.magnitude > 0.1f;
        public float CurrentSpeed => currentSpeed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.autoTraverseOffMeshLink = true;
                agent.autoRepath = true;
                agent.isStopped = true;
                agent.speed = runSpeed;
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        public void SetEnraged(bool enraged)
        {
            isEnraged = enraged;
            UpdateSpeed();
        }

        private void UpdateSpeed()
        {
            if (agent == null) return;
            currentSpeed = isEnraged ? runSpeed * enrageSpeedMultiplier : runSpeed;
            agent.speed = currentSpeed;
        }

        public void SetStopped(bool stopped)
        {
            if (!IsAgentReady()) return;
            agent.isStopped = stopped;
            if (!stopped) agent.ResetPath();
        }

        public void ChasePlayer()
        {
            if (player == null || !IsAgentReady()) return;
            agent.SetDestination(player.position);
        }

        public void FleeFromPlayer(float distance)
        {
            if (player == null || !IsAgentReady()) return;

            Vector3 dirFromPlayer = transform.position - player.position;
            Vector3 fleePos = transform.position + dirFromPlayer.normalized * distance;
            agent.SetDestination(fleePos);
        }

        public void FacePlayer()
        {
            if (player == null) return;

            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;

            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        public float DistanceToPlayer()
        {
            if (player == null) return float.MaxValue;
            return Vector3.Distance(transform.position, player.position);
        }

        public bool IsAgentReady()
        {
            return agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh && agent.enabled;
        }

        public void UpdateMovementAnimation(Animator animator)
        {
            if (animator == null || agent == null) return;
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
}