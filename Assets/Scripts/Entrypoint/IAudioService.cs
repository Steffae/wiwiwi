using UnityEngine;

public interface IAudioService
{
    void PlayMusic(AudioClip clip);
    void PlaySoundEffect(AudioClip clip);

    void SetMusicVolume(float volume);
    void SetSoundEffectsVolume(float volume);

    float MusicVolume { get; }
    float SoundEffectsVolume { get; }
}