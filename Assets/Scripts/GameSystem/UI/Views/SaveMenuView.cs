using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.System.UI
{
    public class SaveMenuView : UIView
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button menuButton;

        private SaveMenuController Controller => controller as SaveMenuController;

        public override void Initialize(UIController controller)
        {
            base.Initialize(controller);

            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClick);
            if (loadButton != null)
                loadButton.onClick.AddListener(OnLoadClick);
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClick);
        }

        private void OnSaveClick()
        {
            Controller?.OnSavePressed();
        }

        private void OnLoadClick()
        {
            Controller?.OnLoadPressed();
        }

        private void OnMenuClick()
        {
            Controller?.OnMenuPressed();
        }

        private void OnDestroy()
        {
            if (saveButton != null)
                saveButton.onClick.RemoveListener(OnSaveClick);
            if (loadButton != null)
                loadButton.onClick.RemoveListener(OnLoadClick);
            if (menuButton != null)
                menuButton.onClick.RemoveListener(OnMenuClick);
        }
    }
}