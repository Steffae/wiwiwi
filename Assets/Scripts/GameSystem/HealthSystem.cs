using System;

public class HealthSystem
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    // События
    public event Action<float> OnDamageTaken;    // (урон)
    public event Action<float> OnHealthChanged;  // (текущее здоровье)
    public event Action OnDeath;                  // ()

    public HealthSystem(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        CurrentHealth = Math.Max(CurrentHealth, 0);

        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public float GetHealthPercent()
    {
        return CurrentHealth / MaxHealth;
    }
}