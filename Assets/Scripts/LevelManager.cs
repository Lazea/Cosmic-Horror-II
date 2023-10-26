using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SOGameEventSystem;

public class LevelManager : Singleton<LevelManager>
{
    public BaseGameEvent onLoadCurrentLevel;
    public BaseGameEvent onLoadMainMenu;

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        LoadLevel(0);
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
