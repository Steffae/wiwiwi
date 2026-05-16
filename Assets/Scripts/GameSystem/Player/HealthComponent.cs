using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Game.Data;

public class HealthComponent : MonoBehaviour, IHealth
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

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    public float CurrentHealth => healthSystem?.CurrentHealth ?? 0;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public HealthSystem HealthSystem => healthSystem;

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
        if (playerData != null && playerData.isInitialized)
        {
            healthSystem = new HealthSystem(playerData.maxHealth);
            healthSystem.SetHealth(playerData.currentHealth);
        }
        else
        {
            healthSystem = new HealthSystem(maxHealth);

            if (playerData != null)
            {
                playerData.maxHealth = maxHealth;
                playerData.currentHealth = maxHealth;
                playerData.isInitialized = true;
            }
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

    private void UpdateUI(float currentHealth)
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {maxHealth}";
        }
    }

    private void HandleDamageTaken(float damage)
    {
        if (isDead) return;

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

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && damage > 10f)
        {
            Vector3 knockbackDirection = -transform.forward;
            knockbackDirection.y = 0.2f;
            knockbackDirection.Normalize();

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

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

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

        SceneLoader.LoadEnd();
    }

    private void DisableControls()
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

    private void DisableCamera()
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

    public void Reset()
    {
        healthSystem.Reset();
        isDead = false;
        UpdateUI(healthSystem.CurrentHealth);
    }

    public void SetHealth(float health)
    {
        if (healthSystem != null)
        {
            healthSystem.SetHealth(health);
            UpdateUI(healthSystem.CurrentHealth);
        }
    }

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
}