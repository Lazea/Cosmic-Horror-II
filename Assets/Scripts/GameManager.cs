using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOGameEventSystem;

public class GameManager : Singleton<GameManager>
{
    public GameSettings settings;

    public Player player;
    public Transform propsContainer;

    public bool IsPaused
    {
        get { return Time.timeScale <= 0f; }
    }

    [Header("Events")]
    public BaseGameEvent onGamePause;
    public BaseGameEvent onGameResume;

    // Components
    Controls.AppActions controls;

    private void Awake()
    {
        base.Awake();

        var _player = GameObject.Find("Player");
        if(_player != null)
            player = _player.GetComponent<Player>();

        controls = new Controls().App;
    }

    private void Start()
    {
        controls.Pause.performed += ctx =>
        {
            ToggleGamePause(true);
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

    public static GameSettings Settings()
    {
        return Resources.Load<GameSettings>(
            "Assets/Settings/SO_GameSettings.asset");
    }

    #region [Game Pause]
    public void PauseGame(bool raiseEvent)
    {
        Time.timeScale = 0f;
        if (raiseEvent)
            onGamePause.Raise();
    }

    public void ResumeGame(bool raiseEvent)
    {
        Time.timeScale = 1f;
        if (raiseEvent)
            onGameResume.Raise();
    }

    public void ToggleGamePause(bool raiseEvent)
    {
        Time.timeScale = (IsPaused) ? 1f : 0f;

        if (raiseEvent)
        {
            if(IsPaused)
                onGamePause.Raise();
            else
                onGameResume.Raise();
        }
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
