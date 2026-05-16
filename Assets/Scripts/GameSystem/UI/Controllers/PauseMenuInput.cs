using UnityEngine;

namespace Game.System.UI
{
    public class PauseMenuInput : MonoBehaviour
    {
        [SerializeField] private PauseMenuController pauseMenuController;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenuController != null)
                {
                    pauseMenuController.ToggleMenu();
                }
            }
        }
    }
}