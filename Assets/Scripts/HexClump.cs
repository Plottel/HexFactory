using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ClumpShape
{
    Random,
    Triangular
}

public class HexClump : MonoBehaviour
{   
    public HexCell HexCellPrefab;
    public HexPiece HexPiecePrefab;

    [HideInInspector]public int MinClumpSize;
    [HideInInspector] public int MaxClumpSize;
    [HideInInspector] public float TimeToReachMagnet;
    [HideInInspector] public Ease TweenEasingType;
    [HideInInspector] public ClumpShape ClumpShape;

    private HexGrid _clumpGrid;
    private List<HexPiece> _pieces;
    private Dictionary<HexPiece, Vector3> _startingPositions;
    private bool _isLaser;

    private Tweener _moveTweener;

    void Awake()
    {
        InitializeGrid();
        MinClumpSize = GameManager.Settings.MinClumpSize;
        MaxClumpSize = GameManager.Settings.MaxClumpSize;
        TimeToReachMagnet = GameManager.Settings.TimeToReachMagnet;
        TweenEasingType = GameManager.Settings.ClumpTweeningType;
        ClumpShape = GameManager.Settings.ClumpShape;
    }

    public void InitializeGrid()
    {
        _clumpGrid = new GameObject().AddComponent<HexGrid>();
        _clumpGrid.name = "Hex Clump Grid";
        _clumpGrid.CellPrefab = HexCellPrefab;

        // Ensure odd grid size so center is actually center
        int gridSize = MaxClumpSize % 2 == 0 ? MaxClumpSize + 1 : MaxClumpSize;

        _clumpGrid.SetColumnsNoReset(gridSize);
        _clumpGrid.SetRowsNoReset(gridSize);

        _clumpGrid.CellSize = GameManager.kHexGridCellSize; // This will trigger _grid.ResetGrid()

        _clumpGrid.SetCenterPos(transform.position);
    }

    public void CreateLaser()
    {
        if (_clumpGrid == null)
            InitializeGrid();

        _clumpGrid.ResetGrid();
        _isLaser = true;

        HexPiece laserPiece = Instantiate(HexPiecePrefab, _clumpGrid.GetCenterCell().transform);
        laserPiece.Type = HexType.Laser;
        laserPiece.clump = this;

        _pieces = new List<HexPiece> { laserPiece };
    }

    public void CreateClump()
    {
        if (_clumpGrid == null)
            InitializeGrid();

        _clumpGrid.ResetGrid();

        _isLaser = false;
        _pieces = new List<HexPiece>();
        _startingPositions = new Dictionary<HexPiece, Vector3>();

        HexType clumpType = (HexType)Random.Range((int)HexType.Basic1, (int)HexType.Basic5 + 1); // Random.Range is Max _exclusive_ for ints
        int clumpSize = Random.Range(MinClumpSize, MaxClumpSize + 1);

        if (ClumpShape == ClumpShape.Random)
            CreateRandomShapedClump(clumpType, clumpSize);
        else if (ClumpShape == ClumpShape.Triangular)
            CreateTriangularShapedClump(clumpType, clumpSize);
    }

    private void CreateRandomShapedClump(HexType clumpType, int clumpSize)
    {
        HexCell currentCell = _clumpGrid.GetCenterCell();
        AddHexPieceToClump(clumpType, currentCell.transform);

        for (int i = 1; i < clumpSize; ++i) // For each remaining piece to be created
        {
            currentCell = currentCell.GetRandomEmptyNeighbour();
            AddHexPieceToClump(clumpType, currentCell.transform);
        }
    }

    // (NOTE) Matt: Clump Size here refers to triangle size.
    // E.g. clumpSize of 1 will create 3 HexPieces.
    private void CreateTriangularShapedClump(HexType clumpType, int clumpSize)
    {
        if (clumpSize != 1)
        {
            Debug.LogError("Only clump size of 1 is supported for Triangular Clumps.");
            return;
        }

        HexCell centerCell = _clumpGrid.GetCenterCell();
        AddHexPieceToClump(clumpType, centerCell.transform);

        int firstNeighbourIndex = Random.Range(0, 4);

        AddHexPieceToClump(clumpType, centerCell.neighbours[firstNeighbourIndex].transform);
        AddHexPieceToClump(clumpType, centerCell.neighbours[firstNeighbourIndex + 1].transform);
    }

    private HexPiece AddHexPieceToClump(HexType pieceType, Transform parent)
    {
        HexPiece newPiece = Instantiate(HexPiecePrefab, parent);
        newPiece.Type = pieceType;
        newPiece.clump = this;
        _pieces.Add(newPiece);
        _startingPositions[newPiece] = newPiece.transform.position;

        return newPiece;
    }

    public void Deploy()
    {
        HexCell centerCell = GameManager.Instance.Grid.GetCenterCell();
        float clumpGridY = _clumpGrid.transform.position.y;

        float targetY = clumpGridY + ((centerCell.transform.position.y - clumpGridY) * 1.5f); // Aim for past the Magnet center to account for randomized clump layout.

        _moveTweener = _clumpGrid.transform.DOMoveY(targetY, TimeToReachMagnet)
            .SetEase(TweenEasingType);
    }

    public void OnClumpPieceCollision(HexPiece clumpPiece, HexPiece collisionPiece)
    {
        _moveTweener.Kill();
        _moveTweener = null;

        if (_isLaser)
        {
            HexCell clumpCell = GameManager.Instance.Grid.WorldPosToCell(clumpPiece.transform.position);
            AttachPieceToCell(clumpPiece, clumpCell);

            DestructionManager.Instance.DestroyConnectingCellsOfType(clumpCell, collisionPiece.Type);
        }
        else
        {
            AttachClumpToGrid(clumpPiece);
        }

        Destroy(_clumpGrid.gameObject);
        Destroy(this.gameObject);
    }

    private void AttachClumpToGrid(HexPiece clumpPiece)
    {
        HexGrid gameGrid = GameManager.Instance.Grid;

        Vector3 posBeforeSnap = clumpPiece.transform.position;
        Vector3 collisionStartingPos = _startingPositions[clumpPiece];

        HexCell collisionCell = gameGrid.WorldPosToCell(clumpPiece.transform.position);

        AttachPieceToCell(clumpPiece, collisionCell);

        Vector3 snapDelta = clumpPiece.transform.position - posBeforeSnap;

        foreach (HexPiece piece in _pieces)
        {
            if (piece == clumpPiece)
                continue;

            Vector3 targetPos = piece.transform.position + snapDelta;

            AttachPieceToCell(piece, gameGrid.WorldPosToCell(targetPos));
        }
    }

    private void AttachPieceToCell(HexPiece piece, HexCell cell)
    {
        piece.transform.parent = cell.transform;
        piece.transform.localPosition = Vector3.zero;
        piece.transform.localRotation = Quaternion.Euler(0, 0, 30); // Flat top hex has z rotation of 30
        piece.clump = null;
        cell.piece = piece;
    }
}
