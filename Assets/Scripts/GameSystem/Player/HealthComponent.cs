using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Data;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 300f;

    [Header("ScriptableObject Data")]
    [SerializeField] private PlayerRuntimeData playerData;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private GameObject damageImage;

    private HealthSystem healthSystem;
    private Animator animator;
    private bool isDead = false;

    public System.Action<float> OnDamageTaken;
    public System.Action OnDeath;

    public HealthSystem HealthSystem => healthSystem;
    public float CurrentHealth => healthSystem?.CurrentHealth ?? 0;
    public float MaxHealthValue => maxHealth;

    void Awake()
    {
        InitializeHealthSystem();

        healthSystem.OnDamageTaken += HandleDamageTaken;
        healthSystem.OnHealthChanged += UpdateUI;
        healthSystem.OnHealthChanged += SaveHealthToData;
        healthSystem.OnDeath += HandleDeath;

        animator = GetComponent<Animator>();
    }

    private void InitializeHealthSystem()
    {
        // Если есть PlayerRuntimeData и он инициализирован - берём оттуда
        if (playerData != null && playerData.isInitialized)
        {
            healthSystem = new HealthSystem(playerData.maxHealth);
            healthSystem.SetHealth(playerData.currentHealth);
            Debug.Log($"Health loaded from ScriptableObject: {playerData.currentHealth}/{playerData.maxHealth}");
        }
        else
        {
            healthSystem = new HealthSystem(maxHealth);

            // Если PlayerRuntimeData есть, но не инициализирован - инициализируем
            if (playerData != null)
            {
                playerData.maxHealth = maxHealth;
                playerData.currentHealth = maxHealth;
                playerData.isInitialized = true;
            }

            Debug.Log($"Health initialized with default: {maxHealth}");
        }
    }

    private void SaveHealthToData(float currentHealth)
    {
        if (playerData != null)
        {
            playerData.currentHealth = currentHealth;
        }
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = healthSystem.CurrentHealth;
        }

        UpdateUI(healthSystem.CurrentHealth);

        if (damageImage != null)
        {
            damageImage.SetActive(false);
        }
    }

    void UpdateUI(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {maxHealth}";
        }
    }

    void HandleDamageTaken(float damage)
    {
        if (isDead) return;

        Debug.Log($"{gameObject.name} получил {damage} урона. Здоровье: {healthSystem.CurrentHealth}");

        OnDamageTaken?.Invoke(damage);

        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }

        if (damageImage != null)
        {
            damageImage.SetActive(true);
            StartCoroutine(ShowVignette());
        }

        // Эффект отбрасывания
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && damage > 10f)
        {
            Vector3 knockbackDirection = -transform.forward;
            knockbackDirection.y = 0.2f;
            knockbackDirection.Normalize();

            // Ограничиваем силу отбрасывания (максимум 15)
            float knockbackForce = Mathf.Min(damage * 1.5f, 15f);

            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    IEnumerator ShowVignette()
    {
        yield return new WaitForSeconds(0.7f);

        if (damageImage != null)
        {
            damageImage.SetActive(false);
        }
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} умер");

        OnDeath?.Invoke();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        DisableControls();

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Сбрасываем данные при смерти
        if (playerData != null)
        {
            playerData.DeathReset();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("End");
    }

    void DisableControls()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        DisableCamera();
    }

    void DisableCamera()
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            ThirdPersonCamera camCtrl = cam.GetComponent<ThirdPersonCamera>();
            if (camCtrl != null) camCtrl.enabled = false;

            UnityEngine.InputSystem.PlayerInput playerInput = cam.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput != null) playerInput.enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            healthSystem.TakeDamage(damage);
        }
    }

    public void Heal(float amount)
    {
        if (!isDead)
        {
            healthSystem.Heal(amount);
        }
    }

    public void ResetHealth()
    {
        healthSystem.Reset();
        isDead = false;
        UpdateUI(healthSystem.CurrentHealth);
    }

    // Метод для ручного сохранения позиции (вызывать при переходе между сценами)
    public void SavePlayerState(string sceneName)
    {
        if (playerData != null)
        {
            playerData.SavePosition(transform.position, sceneName);
            playerData.currentHealth = healthSystem.CurrentHealth;
        }
    }

    void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken -= HandleDamageTaken;
            healthSystem.OnHealthChanged -= UpdateUI;
            healthSystem.OnHealthChanged -= SaveHealthToData;
            healthSystem.OnDeath -= HandleDeath;
        }
    }

    public void SetHealth(float health)
    {
        if (healthSystem != null)
        {
            healthSystem.SetHealth(health);
            UpdateUI(healthSystem.CurrentHealth);
        }
    }
}