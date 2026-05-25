using UnityEngine;
using Game.Core;

namespace Game.System.UI
{
    public class ScoreboardController : UIController
    {
        private IGameScoreService scoreService;
        private HealthComponent playerHealth;

        public void Initialize(IGameScoreService scoreService)
        {
            this.scoreService = scoreService;

            if (scoreService != null)
            {
                scoreService.OnKillCountChanged += OnKillCountChanged;
                UpdateAllViews();
            }
        }

        public void SetPlayerHealth(HealthComponent healthComponent)
        {
            playerHealth = healthComponent;
        }

        private void OnKillCountChanged(int killCount)
        {
            UpdateAllViews();
        }

        private void UpdateAllViews()
        {
            foreach (var view in GetComponentsInChildren<ScoreboardView>())
            {
                view.UpdateView();
            }
        }

        public IGameScoreService GetScoreService() => scoreService;
        public HealthComponent GetPlayerHealth() => playerHealth;

        private void OnDestroy()
        {
            if (scoreService != null)
            {
                scoreService.OnKillCountChanged -= OnKillCountChanged;
            }
        }
    }
}