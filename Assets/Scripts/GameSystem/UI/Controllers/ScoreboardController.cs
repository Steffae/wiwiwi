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
        }

        public void SetPlayerHealth(HealthComponent healthComponent)
        {
            playerHealth = healthComponent;
        }

        protected override void OnControllerStart()
        {
            if (playerHealth == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerHealth = player.GetComponent<HealthComponent>();
            }

            if (scoreService != null)
            {
                scoreService.OnKillCountChanged += OnKillCountChanged;
                UpdateAllViews();
            }
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