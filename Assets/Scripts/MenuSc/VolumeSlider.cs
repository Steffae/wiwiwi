using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public bool isMusic;

    private IAudioService audioService;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        volumeSlider.value = isMusic
            ? audioService.MusicVolume
            : audioService.SoundEffectsVolume;
    }

    public void SetVolume()
    {
        if (isMusic)
            audioService.SetMusicVolume(volumeSlider.value);
        else
            audioService.SetSoundEffectsVolume(volumeSlider.value);
    }
}