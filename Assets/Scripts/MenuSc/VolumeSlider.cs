using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public bool isMusic;

    private SoundManager soundManager;

    void Start()
    {
        soundManager = GameBootstrapper.Instance.SoundManager;

        if (isMusic)
            volumeSlider.value = soundManager.musicVolume;
        else
            volumeSlider.value = soundManager.soundEffectsVolume;
    }

    public void SetVolume()
    {
        if (isMusic)
            soundManager.SetMusicVolume(volumeSlider.value);
        else
            soundManager.SetSoundEffectsVolume(volumeSlider.value);
    }
}