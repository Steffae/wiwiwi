using UnityEngine;
using System;

namespace Game.Boss
{
    public class BossHealth : MonoBehaviour, IHealth
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 500f;
        [SerializeField] private float stunThreshold = 50f;
        [SerializeField] private float stunDuration = 2f;
        [SerializeField] private float enrageHealthPercent = 0.3f;
        [SerializeField] private float fleeHealthAmount = 50f;

        private float currentHealth;
        private bool isDead = false;
        private bool isEnraged = false;

        public event Action<float, float> OnHealthChanged;
        public event Action<float> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnEnrage;
        public event Action<float> OnStunned;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        public bool IsEnraged => isEnraged;
        public float HealthPercent => currentHealth / maxHealth;
        public bool ShouldFlee => currentHealth <= fleeHealthAmount;

        private void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);

            OnDamageTaken?.Invoke(damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (damage >= stunThreshold && !isDead)
            {
                OnStunned?.Invoke(stunDuration);
            }

            if (!isEnraged && currentHealth <= maxHealth * enrageHealthPercent)
            {
                isEnraged = true;
                OnEnrage?.Invoke();
            }

            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void Reset()
        {
            currentHealth = maxHealth;
            isDead = false;
            isEnraged = false;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}