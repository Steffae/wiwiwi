using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string MENU_SCENE = "MenuScene";
    public const string LOCATION_SCENE = "Location";
    public const string BOSS_SCENE = "Location_boss";
    public const string END_SCENE = "End";
    public const string GOOD_END_SCENE = "GoodEnd";

    public static void LoadMenu() => SceneManager.LoadScene(MENU_SCENE);
    public static void LoadLocation() => SceneManager.LoadScene(LOCATION_SCENE);
    public static void LoadBossLocation() => SceneManager.LoadScene(BOSS_SCENE);
    public static void LoadEnd() => SceneManager.LoadScene(END_SCENE);
    public static void LoadGoodEnd() => SceneManager.LoadScene(GOOD_END_SCENE);
}