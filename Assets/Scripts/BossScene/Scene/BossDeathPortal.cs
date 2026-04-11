using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Boss
{
    public class BossDeathPortal : MonoBehaviour
    {
        [Header("Portal Settings")]
        [SerializeField] private string targetSceneName = "GoodEnd";
        [SerializeField] private string playerTag = "Player";

        [SerializeField] private GameObject activationEffect;

        private Collider portalCollider;
        private bool isActive = false;

        private void Awake()
        {
            portalCollider = GetComponent<Collider>();

            // На старте портал выключен
            SetPortalActive(false);
        }

        private void Start()
        {
            // Находим босса и подписываемся на его смерть
            BossController boss = FindAnyObjectByType<BossController>();
            if (boss != null)
            {
                boss.OnBossDeath += ActivatePortal;
                Debug.Log("Portal subscribed to boss death");
            }
            else
            {
                Debug.LogWarning("BossDeathPortal: BossController not found in scene!");
            }
        }

        // Активирует портал (вызывается при смерти босса)
        public void ActivatePortal()
        {
            SetPortalActive(true);

            // Проигрываем эффект активации
            if (activationEffect != null)
            {
                activationEffect.SetActive(true);
            }

            Debug.Log("Portal activated! Player can now exit.");
        }

        // Включает/выключает визуал и коллайдер портала
        private void SetPortalActive(bool active)
        {
            isActive = active;

            if (portalCollider != null)
            {
                portalCollider.enabled = active;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            if (other.CompareTag(playerTag))
            {
                Debug.Log($"Player entered portal, loading scene: {targetSceneName}");
                SceneManager.LoadScene(targetSceneName);
            }
        }

        private void OnDestroy()
        {
            // Отписываемся от события
            BossController boss = FindAnyObjectByType<BossController>();
            if (boss != null)
            {
                boss.OnBossDeath -= ActivatePortal;
            }
        }
    }
}