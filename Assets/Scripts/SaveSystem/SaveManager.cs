using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SaveInteractor saveInteractor;

    void Awake()
    {
        saveInteractor = new SaveInteractor();
    }

    public void SaveGame()
    {
        saveInteractor.SaveGame();
    }

    public void LoadGame()
    {
        saveInteractor.LoadGame();
    }

    public bool HasSave()
    {
        return saveInteractor.HasSave();
    }
}