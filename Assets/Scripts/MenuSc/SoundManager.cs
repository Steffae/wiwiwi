using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private AudioSource musicSource;
    private AudioSource sfxSource;

    public float musicVolume = 0.5f;
    public float soundEffectsVolume = 0.5f;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip endMusic;

    [SerializeField] private AudioClip buttonClickSound;

    private void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;

        musicSource.volume = musicVolume;
        sfxSource.volume = soundEffectsVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
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
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = volume;
        sfxSource.volume = volume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MenuScene":
                PlayMusic(menuMusic);
                break;

            case "Location":
                PlayMusic(gameMusic);
                break;

            case "End":
                PlayMusic(endMusic);
                break;
        }
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClickSound);
    }
}