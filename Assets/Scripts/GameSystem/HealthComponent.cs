using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private GameObject damageImage;

    //[Header("Damage Effect")]
    //[SerializeField] private float damageFlashDuration = 0.2f;
    //[SerializeField] private Color damageFlashColor = new Color(1f, 0f, 0f, 0.5f);

    private HealthSystem healthSystem;
    private Animator animator;
    private bool isDead = false;
    //private Color originalImageColor;

    // События для оповещения о получении урона
    public System.Action<float> OnDamageTaken;
    public System.Action OnDeath;

    // Свойство доступа к HealthSystem
    public HealthSystem HealthSystem => healthSystem;
    public float CurrentHealth => healthSystem?.CurrentHealth ?? 0;
    public float MaxHealthValue => maxHealth;

    public void SetHealth(float health)
    {
        healthSystem.SetHealth(health);
    }

    void Awake()
    {
        healthSystem = new HealthSystem(maxHealth);

        healthSystem.OnDamageTaken += HandleDamageTaken;
        healthSystem.OnHealthChanged += UpdateUI;
        healthSystem.OnDeath += HandleDeath;

        animator = GetComponent<Animator>();
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
        damageImage.SetActive(false);
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

        Debug.Log($"{gameObject.name} ������� {damage} �����. ��������: {healthSystem.CurrentHealth}");

        OnDamageTaken?.Invoke(damage);

        // Воспроизведение эффекта удара
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
            knockbackDirection.y = 0.3f;
            rb.AddForce(knockbackDirection * damage * 2f, ForceMode.Impulse);
        }
    }


    IEnumerator ShowVignette()
    {
        // Ждём
        yield return new WaitForSeconds(0.7f);

        // Скрываем
        damageImage.SetActive(false);
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} ����");

        OnDeath?.Invoke();

        // Воспроизведение анимации
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Отключение управления
        DisableControls();

        // Скрытие полоски здоровья
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }

        // Разблокировка курсора
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Загрузка сцены "End"
        UnityEngine.SceneManagement.SceneManager.LoadScene("End");
    }

    void DisableControls()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Отключение коллайдеров
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Отключение физики
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

    void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken -= HandleDamageTaken;
            healthSystem.OnHealthChanged -= UpdateUI;
            healthSystem.OnDeath -= HandleDeath;
        }
    }
}