using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Boss
{
    public class BossDeathPortal : MonoBehaviour
    {
        [Header("Portal Settings")]
        [SerializeField] private string playerTag = "Player";


        private IGameStateService gameStateService;

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
            // Получаем сервис из GameEntrypoint
            if (GameEntrypoint.Instance != null)
            {
                gameStateService = GameEntrypoint.Instance.GameStateService;
            }
            else
            {
                Debug.LogError("BossDeathPortal: GameEntrypoint.Instance is null!");
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
                Debug.Log("Player entered portal, loading GoodEnd scene");
                gameStateService.LoadGoodEnd();
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