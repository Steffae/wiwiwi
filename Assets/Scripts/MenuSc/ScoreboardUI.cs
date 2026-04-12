using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private Text killCountText;
    [SerializeField] private Image progressFill;

    private IGameScoreService scoreService;

    public void Initialize(IGameScoreService scoreService)
    {
        this.scoreService = scoreService;
        scoreService.OnKillCountChanged += UpdateDisplay;

        UpdateDisplay(scoreService.KillCount);
    }

    private void UpdateDisplay(int killCount)
    {
        if (killCountText != null)
        {
            killCountText.text = $"Убийства: {killCount} / {scoreService.VictoryKills}";
        }

        if (progressFill != null)
        {
            progressFill.fillAmount = (float)killCount / scoreService.VictoryKills;
        }
    }

    private void OnDestroy()
    {
        if (scoreService != null)
        {
            scoreService.OnKillCountChanged -= UpdateDisplay;
        }
    }
}
