using UnityEngine;

public class AudioService : IAudioService
{
    private readonly AudioSource musicSource;
    private readonly AudioSource sfxSource;

    public float MusicVolume => musicSource.volume;
    public float SoundEffectsVolume => sfxSource.volume;

    public AudioService(GameObject host)
    {
        musicSource = host.AddComponent<AudioSource>();
        sfxSource = host.AddComponent<AudioSource>();

        musicSource.loop = true;

        musicSource.volume = 0.5f;
        sfxSource.volume = 0.5f;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}