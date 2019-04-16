using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public static bool SkipStartMenuOnSceneLoad;

    public GameSettings GameSettings;

    public const float kHexGridCellSize = 0.59f;

    public HexPiece HexPiecePrefab;
    public HexCell HexCellPrefab;
    public Text CurrentScoreText;

    public HexGrid Grid;

    public event Action eventGameOver;

    public delegate void PauseEventHandler(bool paused);
    public event PauseEventHandler eventPause;

    private bool _isPaused;

    private int _currentScore;
    public int CurrentScore
    {
        get { return _currentScore; }
        set
        {
            _currentScore = value;
            CurrentScoreText.text = value.ToString();
        }
    }

    public static GameSettings Settings
    {
        get { return Instance.GameSettings; }
    }

    public bool IsPaused
    {
        get { return _isPaused; }
    }

    protected override void Awake()
    {
        base.Awake();
        Grid = Instance.Grid;
        GameSettings = Instance.GameSettings;
        CurrentScore = 0;

        SetPause(true);
    }

    public void SetPause(bool pause)
    {
        _isPaused = pause;
        Time.timeScale = pause ? 0 : 1;

        CurrentScoreText.gameObject.SetActive(!pause);

        if (eventPause != null)
            eventPause(pause);
    }

    public void ResetGame(bool skipStartMenu)
    {
        SkipStartMenuOnSceneLoad = skipStartMenu;
        CurrentScore = 0;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OnGameOver()
    {
        SetPause(true);

        if (eventGameOver != null)
            eventGameOver();
    }
}
