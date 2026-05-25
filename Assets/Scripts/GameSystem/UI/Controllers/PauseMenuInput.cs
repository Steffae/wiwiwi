using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.System.UI
{
    public class PauseMenuInput : MonoBehaviour
    {
        [SerializeField] private PauseMenuController pauseMenuController;

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (pauseMenuController != null)
                {
                    pauseMenuController.ToggleMenu();
                }
            }
        }
    }
}