using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public bool isMusic; // true для музыки, false для звуковых эффектов

    void Start()
    {
        //Загрузка значений из SoundManager
        if (SoundManager.Instance != null)
        {
            if (isMusic)
            {
                volumeSlider.value = SoundManager.Instance.musicVolume;
            }
            else
            {
                volumeSlider.value = SoundManager.Instance.soundEffectsVolume;
            }
        }
    }

    public void SetVolume()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager is not initialized!");
            return;
        }

        if (isMusic)
        {
            SoundManager.Instance.SetMusicVolume(volumeSlider.value);
        }
        else
        {
            SoundManager.Instance.SetSoundEffectsVolume(volumeSlider.value);
        }
    }
}
