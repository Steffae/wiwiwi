using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public float musicVolume = 1f;
    public float soundEffectsVolume = 1f;

    [Header("Музыка")]
    public AudioClip backgroundMusic; 
    private AudioSource musicSource; 

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSource(); 
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Настройка AudioSource
    void SetupAudioSource()
    {
        // Создаем или получаем AudioSource компонент
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        // Настройки для фоновой музыки
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.volume = musicVolume;

        // Запускаем музыку
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);

        // Теперь управляем конкретным musicSource
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SOUND_EFFECTS_VOLUME_KEY, soundEffectsVolume);
        // ApplyVolume для звуковых эффектов остается
    }

    // Упрощенный ApplyVolume только для звуковых эффектов
    void ApplyVolume()
    {
        // Только для звуковых эффектов
        AudioSource[] sources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in sources)
        {
            // Пропускаем наш musicSource
            if (source == musicSource) continue;

            if (source.gameObject.CompareTag("SoundEffect"))
            {
                source.volume = soundEffectsVolume;
            }
        }
    }
    void LoadVolume()
    {
        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
        {
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        }
        if (PlayerPrefs.HasKey(SOUND_EFFECTS_VOLUME_KEY))
        {
            soundEffectsVolume = PlayerPrefs.GetFloat(SOUND_EFFECTS_VOLUME_KEY);
        }

        // Применяем громкость к musicSource
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        ApplyVolume();
    }
}