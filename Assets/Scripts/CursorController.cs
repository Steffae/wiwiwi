using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Settings")]
    public bool startLocked = true;

    private bool isCursorLocked = true;

    void Start()
    {
        if (startLocked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
        Debug.Log("Курсор заблокирован");
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
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