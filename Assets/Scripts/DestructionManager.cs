using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestructionManager : Singleton<DestructionManager>
{
    public enum DestructionState
    {
        LaserPieces,
        IsolatedPieces
    }

    [HideInInspector] public float DestructionShrinkEndScale;
    [HideInInspector] public float DestructionShrinkDuration;

    private Tweener _scaleTweener;
    private Vector3 _startingTweenScale;
    private Vector3 _currentTweenScale;

    private List<HexCell> _toDestroy;
    private DestructionState _destructionState;

    private void Start()
    {
        DestructionShrinkEndScale = GameManager.Settings.DestructionShrinkEndScale;
        DestructionShrinkDuration = GameManager.Settings.DestructionShrinkDuration;
    }

    public void DestroyConnectingCellsOfType(HexCell startingCell)
    {
        var toDestroy = new List<HexCell>();

        foreach (HexCell startingCellNeighbour in startingCell.neighbours)
        {
            if (startingCellNeighbour.IsEmpty || startingCellNeighbour.piece.Type == HexType.Magnet)
                continue;

            var toCheck = new Stack<HexCell>();
            var haveChecked = new List<HexCell>();
            HexType destructionType = startingCellNeighbour.piece.Type;

            toCheck.Push(startingCell);
            HexCell current;

            while (toCheck.Count > 0)
            {
                current = toCheck.Pop();

                haveChecked.Add(current);

                if (!toDestroy.Contains(current))
                    toDestroy.Add(current);

                foreach (HexCell neighbour in current.neighbours)
                {
                    if (haveChecked.Contains(neighbour) || toCheck.Contains(neighbour))
                        continue;

                    if (neighbour.piece != null && neighbour.piece.Type == destructionType && neighbour.piece.Type != HexType.Magnet) // Don't destroy the magnet...
                        toCheck.Push(neighbour);
                }
            }
        }        

        _destructionState = DestructionState.LaserPieces;
        BeginDestruction(toDestroy);
    }

    private void BeginDestruction(List<HexCell> toDestroy)
    {
        if (toDestroy == null || toDestroy.Count == 0)
            return;

        _toDestroy = toDestroy;

        if (_scaleTweener != null)
            _scaleTweener.Kill();

        _startingTweenScale = toDestroy[0].piece.transform.localScale;
        _currentTweenScale = _startingTweenScale;
        Vector3 finalScale = new Vector3(DestructionShrinkEndScale, DestructionShrinkEndScale, DestructionShrinkEndScale);

        _scaleTweener = DOTween.To(
            () => _currentTweenScale, 
            scale => _currentTweenScale = scale, 
            finalScale, 
            DestructionShrinkDuration)
            .OnUpdate(OnScaleTweenUpdate)
            .OnComplete(OnScaleTweenComplete);
    }

    private void OnScaleTweenUpdate()
    {
        foreach (HexCell cell in _toDestroy)
            cell.piece.transform.localScale = _currentTweenScale;
    }

    private void OnScaleTweenComplete()
    {
        GameManager.Instance.CurrentScore += _toDestroy.Count;

        foreach (HexCell cell in _toDestroy)
        {
            Destroy(cell.piece.gameObject);
            cell.piece = null;
        }

        if (_destructionState == DestructionState.LaserPieces)
        {
            // Begin phase 2 of destruction - destroy all isolated pieces.
            _destructionState = DestructionState.IsolatedPieces;
            _toDestroy.Clear();

            foreach (HexCell cell in GameManager.Instance.Grid)
            {
                if (cell.IsIsolated)
                    _toDestroy.Add(cell);
            }

            BeginDestruction(_toDestroy);
        }
    }
}
