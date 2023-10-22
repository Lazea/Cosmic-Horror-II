using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOGameEventSystem;

public class GameManager : Singleton<GameManager>
{
    public GameSettings settings;

    public Transform propsContainer;

    public bool IsPaused
    {
        get { return Time.timeScale <= 0f; }
    }

    [Header("Events")]
    public SOGameEventSystem.BaseGameEvent onGamePause;
    public SOGameEventSystem.BaseGameEvent onGameResume;

    // Components
    Controls.AppActions controls;

    // Start is called before the first frame update
    void Awake()
    {
        controls = new Controls().App;
    }

    private void Start()
    {
        controls.Pause.performed += ctx =>
        {
            ToggleGamePause();
            if (IsPaused)
                onGamePause.Raise();
            else
                onGameResume.Raise();
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameSettings Settings()
    {
        return Resources.Load<GameSettings>(
            "Assets/Settings/SO_GameSettings.asset");
    }

    #region [Game Pause]
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void ToggleGamePause()
    {
        Time.timeScale = (IsPaused) ? 1f : 0f;
    }
    #endregion

    #region [Audio Utils]
    public static float GetVolume(float value)
    {
        return Mathf.Log10(value) * 20f;
    }

    public static float GetNormalizedVolume(float value)
    {
        return Mathf.Pow(10.0f, value / 20.0f);
    }
    #endregion
}
