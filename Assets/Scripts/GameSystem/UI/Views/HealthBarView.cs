using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.System.UI
{
    public class HealthBarView : UIView
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;

        private HealthBarController Controller => controller as HealthBarController;

        public override void Initialize(UIController controller)
        {
            base.Initialize(controller);

            if (healthSlider == null)
                healthSlider = GetComponent<Slider>();
        }

        public override void UpdateView()
        {
            if (Controller == null) return;

            var targetHealth = Controller.GetTargetHealth();
            if (targetHealth == null) return;

            if (healthSlider != null)
            {
                healthSlider.minValue = 0;
                healthSlider.maxValue = targetHealth.MaxHealth;
                healthSlider.value = targetHealth.CurrentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.Ceil(targetHealth.CurrentHealth)} / {targetHealth.MaxHealth}";
            }
        }
    }
}