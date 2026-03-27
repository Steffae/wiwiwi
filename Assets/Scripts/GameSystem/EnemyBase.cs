using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 50f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("UI")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    public float showDistance = 10f;

    protected HealthSystem healthSystem;
    protected bool isDying = false;
    protected bool isHit = false;
    protected NavMeshAgent agent;
    protected Transform player;

    // UI элементы
    protected GameObject healthBarInstance;
    protected Slider healthSlider;
    protected RectTransform healthBarRect;
    protected Camera mainCamera;

    protected virtual void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);

        // Подписываемся на события
        healthSystem.OnHealthChanged += UpdateHealthUI;
        healthSystem.OnDeath += Die;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        mainCamera = Camera.main;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Создаём полоску здоровья
        if (healthBarPrefab != null)
        {
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
                healthSlider.value = healthSystem.CurrentHealth;
            }
        }
    }

    protected virtual void LateUpdate()
    {
        if (healthBarRect == null || mainCamera == null || isDying || healthSystem.CurrentHealth <= 0)
        {
            if (healthBarInstance != null && healthBarInstance.activeSelf)
                healthBarInstance.SetActive(false);
            return;
        }

        // Проверяем расстояние до игрока
        bool showHealthBar = false;
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            showHealthBar = distanceToPlayer <= showDistance;
        }

        if (showHealthBar)
        {
            Vector3 worldPos = transform.position + healthBarOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                healthBarRect.position = screenPos;
                healthBarInstance.SetActive(true);
            }
            else
            {
                healthBarInstance.SetActive(false);
            }
        }
        else
        {
            if (healthBarInstance != null && healthBarInstance.activeSelf)
                healthBarInstance.SetActive(false);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDying) return;

        healthSystem.TakeDamage(damage);
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось: {healthSystem.CurrentHealth}");

        if (!isDying && healthSystem.CurrentHealth > 0)
        {
            StartCoroutine(HitReaction());
        }
    }

    protected void UpdateHealthUI(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    protected virtual IEnumerator HitReaction()
    {
        isHit = true;

        // Проверяем, жив ли агент и не умирает ли враг
        if (agent != null && agent.isActiveAndEnabled && !isDying)
        {
            agent.isStopped = true;
        }

        // Тычок назад
        Vector3 startPos = transform.position;
        Vector3 backDirection = -transform.forward * 1.5f;
        Vector3 targetPos = startPos + backDirection;

        float hitTime = 0.2f;
        float elapsed = 0;

        while (elapsed < hitTime)
        {
            float t = elapsed / hitTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаемся обратно
        elapsed = 0;
        while (elapsed < hitTime)
        {
            float t = elapsed / hitTime;
            transform.position = Vector3.Lerp(targetPos, startPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (agent != null && agent.isActiveAndEnabled && !isDying)
        {
            agent.isStopped = false;
        }

        isHit = false;
    }

    protected virtual void Die()
    {
        if (isDying) return;
        isDying = true;

        Debug.Log($"{gameObject.name} умирает");

        // Отключаем движение
        if (agent != null) agent.enabled = false;

        // Отключаем коллайдеры
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Отключаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Прячем полоску здоровья
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance, 0.5f);
        }

        // Запускаем анимацию смерти
        StartCoroutine(DeathAnimation());
    }

    protected virtual IEnumerator DeathAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.position;
        float floatTime = 0.5f;

        float elapsedTime = 0;
        while (elapsedTime < floatTime)
        {
            float t = elapsedTime / floatTime;
            transform.position = originalPosition + Vector3.up * t;
            transform.localScale = originalScale * (1f + t * 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        // Отписываемся от событий
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthUI;
            healthSystem.OnDeath -= Die;
        }

        // Уничтожаем полоску здоровья
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
}