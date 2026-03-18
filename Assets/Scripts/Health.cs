using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public GameObject healthBarPrefab;  // Префаб слайдера
    public Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);

    private Slider healthSlider;
    private GameObject healthBarInstance;
    private RectTransform healthBarRect;
    private Camera mainCamera;
    public GameObject GameOverPanel;
    public GameObject AttackPanel;

    void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;
        GameOverPanel.SetActive(false);

        // Создаём полоску здоровья
        if (healthBarPrefab != null)
        {
            // Ищем Canvas (создаём, если нет)
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
            healthSlider = healthBarInstance.GetComponent<Slider>();
            healthBarRect = healthBarInstance.GetComponent<RectTransform>();

            if (healthSlider != null)
            {
                healthSlider.minValue = 0;
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }

    void LateUpdate()
    {
        if (healthBarRect != null && mainCamera != null)
        {
            // Проверяем, виден ли игрок камере
            Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
            bool isVisible = viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0;

            // Дополнительная проверка лучом (чтобы стены не просвечивали)
            if (isVisible)
            {
                RaycastHit hit;
                Vector3 direction = transform.position - mainCamera.transform.position;
                if (Physics.Raycast(mainCamera.transform.position, direction, out hit, direction.magnitude))
                {
                    // Если луч упёрся не в игрока - значит что-то между камерой и игроком
                    if (hit.collider.gameObject != gameObject)
                    {
                        isVisible = false;
                    }
                }
            }

            if (isVisible)
            {
                Vector3 worldPos = transform.position + healthBarOffset;
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                healthBarRect.position = screenPos;
                healthBarInstance.SetActive(true);
            }
            else
            {
                healthBarInstance.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Обновляем слайдер
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось HP: {currentHealth}");

        // Анимация получения урона
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            AttackPanel.SetActive(true);
            anim.SetTrigger("GetHit");
            AttackPanel.SetActive(false);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} умер");
        Animator anim = GetComponent<Animator>();
        PlayerController controller = GetComponent<PlayerController>();
        if (anim != null && controller != null)
        {
            anim.SetTrigger("Die");
            controller.enabled = false;
            GameOverPanel.SetActive(true);
        }

        EnemyController enemy = GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.enabled = false;
        }
    }
}