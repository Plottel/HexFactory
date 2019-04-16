using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GameManager))]
public class GameManagerInspector : Editor
{
    private GameManager gameManager;
    private HexGrid grid;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        gameManager = target as GameManager;
        grid = gameManager.Grid;

        if (GUILayout.Button("Create Grid Magnet", GUILayout.Width(150)))
            CreateGridMagnet();

        if (GUILayout.Button("Center Grid To Pivot", GUILayout.Width(200)))
            CenterGridToPivot();
    }

    private void OnSceneGUI()
    {
        gameManager = target as GameManager;
        grid = gameManager.Grid;

        Handles.SphereHandleCap(0, grid.transform.parent.position, Quaternion.identity, 0.35f, EventType.Repaint);
    }

    private void CreateGridMagnet()
    {
        grid.ResetGrid();

        HexCell centerCell = grid.GetCenterCell();

        HexPiece centerMagnetPiece = Instantiate(gameManager.HexPiecePrefab, centerCell.transform);
        centerMagnetPiece.Type = HexType.Magnet;
        centerCell.piece = centerMagnetPiece;

        //foreach (var neighbour in centerCell.neighbours)
        //{
        //    HexPiece magnetPiece = Instantiate(gameManager.HexPiecePrefab, neighbour.transform);
        //    magnetPiece.Type = HexType.Magnet;
        //    neighbour.piece = magnetPiece;
        //}
    }

    private void CenterGridToPivot()
    {
        grid.SetCenterPos(grid.transform.parent.position);
    }
}
