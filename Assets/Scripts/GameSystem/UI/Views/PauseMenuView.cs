using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.System.UI
{
    public class PauseMenuView : UIView
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button exitButton;

        private PauseMenuController Controller => controller as PauseMenuController;

        public override void Initialize(UIController controller)
        {
            base.Initialize(controller);

            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClick);
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoadClick);
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClick);
        }

        private void OnSaveClick() => Controller?.OnSavePressed();
        private void OnLoadClick() => Controller?.OnLoadPressed();
        private void OnExitClick() => Controller?.OnExitPressed();

        public override void Show()
        {
            base.Show();
            Time.timeScale = 0f;
        }

        public override void Hide()
        {
            base.Hide();
            Time.timeScale = 1f;
        }

        private void OnDestroy()
        {
            if (saveButton != null) saveButton.onClick.RemoveListener(OnSaveClick);
            if (loadButton != null) loadButton.onClick.RemoveListener(OnLoadClick);
            if (exitButton != null) exitButton.onClick.RemoveListener(OnExitClick);
        }
    }
}