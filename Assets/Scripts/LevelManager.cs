using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SOGameEventSystem;

public class LevelManager : Singleton<LevelManager>
{
    public BaseGameEvent onGameStartEvent;
    public BaseGameEvent onLoadCurrentLevel;
    public BaseGameEvent onLoadMainMenu;

    public void StartGameLevelLoad()
    {
        onGameStartEvent.Raise();
        LoadLevelWithDelay(1, 2f);
    }

    public void LoadLevelWithDelay(int levelIndex, float delay)
    {
        StartCoroutine(LoadLevelDelayed(levelIndex, delay));
    }

    IEnumerator LoadLevelDelayed(int levelIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadLevel(levelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ReloadLevel()
    {
        onLoadCurrentLevel.Raise();
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        onLoadMainMenu.Raise();
        LoadLevel(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}