using System;

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }

    event Action<float, float> OnHealthChanged;
    event Action<float> OnDamageTaken;
    event Action OnDeath;

    void TakeDamage(float damage);
    void Heal(float amount);
    void Reset();
    void SetHealth(float value);
}