using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [Header("References")]
    public HealthComponent targetHealth;
    public Slider healthSlider;
    public Text healthText;

    void Start()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (targetHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                targetHealth = player.GetComponent<HealthComponent>();
        }

        if (targetHealth != null)
        {
            targetHealth.OnDamageTaken += OnHealthChanged;
            targetHealth.HealthSystem.OnHealthChanged += UpdateHealthUI;
            targetHealth.OnDeath += OnTargetDeath;

            healthSlider.minValue = 0;
            healthSlider.maxValue = targetHealth.MaxHealthValue;
            healthSlider.value = targetHealth.CurrentHealth;
            UpdateHealthUI(targetHealth.CurrentHealth);
        }
    }

    void UpdateHealthUI(float currentHealth)
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {targetHealth.MaxHealthValue}";
    }

    void OnHealthChanged(float damage)
    {
        UpdateHealthUI(targetHealth.CurrentHealth);
    }

    void OnTargetDeath()
    {
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnDamageTaken -= OnHealthChanged;
            if (targetHealth.HealthSystem != null)
                targetHealth.HealthSystem.OnHealthChanged -= UpdateHealthUI;
            targetHealth.OnDeath -= OnTargetDeath;
        }
    }
}