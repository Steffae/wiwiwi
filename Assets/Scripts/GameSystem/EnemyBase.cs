using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 50f;
    protected float physicalDamage = 10f;

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
    protected Transform targetPlayer;
    protected Animator animator;

    // UI компоненты
    protected GameObject healthBarInstance;
    protected Slider healthSlider;
    protected RectTransform healthBarRect;
    protected Camera mainCamera;

    // save zone
    public float CurrentHealth => healthSystem.CurrentHealth;
    public float MaxHealthValue => maxHealth;
    public bool IsDying => isDying;

    public void SetHealth(float health)
    {
        if (healthSystem != null)
        {
            healthSystem.SetHealth(health);
        }
    }

    protected virtual void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);

        // Подписываемся на события
        healthSystem.OnHealthChanged += UpdateHealthUI;
        healthSystem.OnDeath += Die;

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        mainCamera = Camera.main;
    }

    protected virtual void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Создание полоски здоровья
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

            Debug.Log($"HealthBar создан для {gameObject.name}: {healthBarInstance != null}");
        }
        else
        {
            Debug.LogWarning($"healthBarPrefab не назначен для {gameObject.name}");
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

        // Проверка расстояния до игрока
        bool showHealthBar = false;
        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
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

        // Останавливаем, чтобы враг не двигался во время отбрасывания
        if (agent != null && agent.isActiveAndEnabled && !isDying)
        {
            agent.isStopped = true;
        }

        // Эффект отдачи
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

        // Возвращение на место
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

        Debug.Log($"{gameObject.name} погиб");

        // Уведомляем систему очков
        if (GameEntrypoint.Instance?.GameScoreService != null)
        {
            GameEntrypoint.Instance.GameScoreService.OnEnemyKilled();
        }

        // Отключение навигации
        if (agent != null) agent.enabled = false;

        // Отключение коллайдеров
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Отключение физики
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Удаление полоски здоровья
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance, 0.5f);
        }

        // Запуск анимации смерти
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
        // Отписка от событий
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthUI;
            healthSystem.OnDeath -= Die;
        }

        // Уничтожение полоски здоровья
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
}