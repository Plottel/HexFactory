using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameSettings GameSettings;

    public const float kHexGridCellSize = 0.59f;

    public HexPiece HexPiecePrefab;
    public HexCell HexCellPrefab;

    public HexGrid Grid;

    public static GameSettings Settings
    {
        get { return Instance.GameSettings; }
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
        Time.timeScale = pause ? 0 : 1;
    }
}
