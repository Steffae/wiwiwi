public interface IGameStateService
{
    void LoadLocation();
    void LoadLocationBoss();
    void LoadMenu();
    void LoadEnd();
    void LoadGoodEnd();

    void ResetPlayerData();
    void ResetScoreData();
    void FullReset();
}