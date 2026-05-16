using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.System.UI
{
    public class ScoreboardView : UIView
    {
        [Header("Kill Count UI")]
        [SerializeField] private Slider killCountSlider;
        [SerializeField] private Text killCountText;

        [Header("Health UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;

        private ScoreboardController Controller => controller as ScoreboardController;

        public override void Initialize(UIController controller)
        {
            base.Initialize(controller);
            UpdateView();
        }

        public override void UpdateView()
        {
            if (Controller == null) return;

            var scoreService = Controller.GetScoreService();
            if (scoreService == null) return;

            if (killCountSlider != null)
            {
                killCountSlider.minValue = 0;
                killCountSlider.maxValue = 5;
                killCountSlider.value = scoreService.KillCount;
            }

            if (killCountText != null)
            {
                killCountText.text = $"{scoreService.KillCount} / {scoreService.VictoryKills}";
            }

            var player = Controller.GetPlayerHealth();
            if (player != null)
            {
                if (healthSlider != null)
                {
                    healthSlider.minValue = 0;
                    healthSlider.maxValue = 100;
                    healthSlider.value = player.CurrentHealth;
                }

                if (healthText != null)
                {
                    healthText.text = $"{Mathf.Ceil(player.CurrentHealth)} / 100";
                }
            }
        }
    }
}