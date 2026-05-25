using Game.System.UI;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private ScoreboardController scoreboard;
    [SerializeField] private PauseMenuController pauseMenu;

    void Start()
    {
        var g = GameEntrypoint.Instance;
        var player = GameObject.FindWithTag("Player").GetComponent<HealthComponent>();

        scoreboard.Initialize(g.GameScoreService);
        scoreboard.SetPlayerHealth(player);

        pauseMenu.Initialize(g.SaveInteractor, g.AudioService, g.GameStateService);
    }
}