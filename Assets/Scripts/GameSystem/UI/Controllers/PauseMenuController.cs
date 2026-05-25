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
        [SerializeField] private GameObject menuPanel;

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameStateService?.LoadMenu();
        }

        protected override void OnControllerStart()
        {
            if (menuPanel != null)
                menuPanel.SetActive(false);
        }

        public void ToggleMenu()
        {
            if (menuPanel == null) return;

            bool isOpen = !menuPanel.activeSelf;
            menuPanel.SetActive(isOpen);
            Time.timeScale = isOpen ? 0f : 1f;

            if (isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}