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

    void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;

        // Создаём полоску здоровья
        if (healthBarPrefab != null)
        {
            // Ищем Canvas (создаём, если нет)
            Canvas canvas = FindObjectOfType<Canvas>();
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
            // Полоска всегда смотрит на камеру и следует за игроком
            Vector3 worldPos = transform.position + healthBarOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            healthBarRect.position = screenPos;
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
            anim.SetTrigger("GetHit");
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
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Отключаем управление
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        EnemyController enemy = GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.enabled = false;
        }

    }
}