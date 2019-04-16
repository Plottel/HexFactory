using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static bool SkipStartMenuOnSceneLoad;

    public GameSettings GameSettings;

    public const float kHexGridCellSize = 0.59f;

    public HexPiece HexPiecePrefab;
    public HexCell HexCellPrefab;

    public HexGrid Grid;

    public event Action eventGameOver;

    private bool _isPaused;

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

        SetPause(true);
    }

    public void SetPause(bool pause)
    {
        _isPaused = pause;
        Time.timeScale = pause ? 0 : 1;
    }

    public void ResetGame(bool skipStartMenu)
    {
        SkipStartMenuOnSceneLoad = skipStartMenu;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void OnGameOver()
    {
        SetPause(true);

        if (eventGameOver != null)
            eventGameOver();
    }
}
