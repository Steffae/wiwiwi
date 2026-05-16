using UnityEngine;
using Game.Core;

namespace Game.System.UI
{
    public class HealthBarController : UIController
    {
        private HealthComponent targetHealth;

        public void SetTarget(HealthComponent healthComponent)
        {
            targetHealth = healthComponent;
        }

        protected override void OnControllerStart()
        {
            if (targetHealth != null)
            {
                targetHealth.OnDamageTaken += OnDamageTaken;
                targetHealth.HealthSystem.OnHealthChanged += OnHealthChanged;
                targetHealth.OnDeath += OnDeath;

                UpdateAllViews();
            }
        }

        private void OnDamageTaken(float damage)
        {
            UpdateAllViews();
        }

        private void OnHealthChanged(float currentHealth)
        {
            UpdateAllViews();
        }

        private void OnDeath()
        {
            HideAll();
        }

        private void UpdateAllViews()
        {
            foreach (var view in GetComponentsInChildren<HealthBarView>())
            {
                view.UpdateView();
            }
        }

        public HealthComponent GetTargetHealth() => targetHealth;

        private void OnDestroy()
        {
            if (targetHealth != null)
            {
                targetHealth.OnDamageTaken -= OnDamageTaken;
                if (targetHealth.HealthSystem != null)
                    targetHealth.HealthSystem.OnHealthChanged -= OnHealthChanged;
                targetHealth.OnDeath -= OnDeath;
            }
        }
    }
}