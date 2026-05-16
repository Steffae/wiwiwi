using UnityEngine;
using Game.Core;

namespace Game.System.UI
{
    public class PauseMenuController : UIController
    {
        private ISaveInteractor saveInteractor;
        private IAudioService audioService;
        private IGameStateService gameStateService;

        [SerializeField] private AudioClip buttonClick;

        public void Initialize(ISaveInteractor saveInteractor, IAudioService audioService, IGameStateService gameStateService = null)
        {
            this.saveInteractor = saveInteractor;
            this.audioService = audioService;
            this.gameStateService = gameStateService ?? GameEntrypoint.Instance?.GameStateService;
        }

        public void OnSavePressed()
        {
            audioService?.PlaySoundEffect(buttonClick);
            saveInteractor?.SaveGame();
        }

        public void OnLoadPressed()
        {
            audioService?.PlaySoundEffect(buttonClick);
            saveInteractor?.LoadGame();
        }

        public void OnExitPressed()
        {
            audioService?.PlaySoundEffect(buttonClick);
            Time.timeScale = 1f;
            gameStateService?.LoadMenu();
        }

        protected override void OnControllerStart()
        {
            Hide();
        }

        public void ToggleMenu()
        {
            if (gameObject.activeSelf)
                Hide();
            else
                Show();
        }
    }
}