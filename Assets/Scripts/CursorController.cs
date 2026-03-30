using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [Header("Settings")]
    public bool startLocked = true;

    private bool isCursorLocked = true;

    public GameObject GameMenuPanel;

    void Start()
    {
        GameMenuPanel.SetActive(false);

        if (startLocked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    void Update()
    {
        // Отслеживаем нажатие ESC через Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isCursorLocked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
        GameMenuPanel.SetActive(false);
        Debug.Log("Курсор заблокирован");
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
        GameMenuPanel.SetActive(true);
        Debug.Log("Курсор разблокирован");
    }

    public void ToggleCursor()
    {
        if (isCursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    public bool IsCursorLocked()
    {
        return isCursorLocked;
    }
}