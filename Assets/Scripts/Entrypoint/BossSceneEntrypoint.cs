using UnityEngine;

public class BossSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;

    [SerializeField] private AudioClip bossMusic;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        if (bossMusic != null)
            audioService.PlayMusic(bossMusic);
    }
}