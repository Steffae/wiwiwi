using UnityEngine;

public class GameGoodSceneEntrypoint : MonoBehaviour
{
    private IAudioService audioService;

    [SerializeField] private AudioClip gameGoodMusic;

    void Start()
    {
        audioService = GameEntrypoint.Instance.AudioService;

        if (gameGoodMusic != null)
            audioService.PlayMusic(gameGoodMusic);
    }
}