using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexCell), true)]
public class HexCellInspector : Editor
{
    static bool showNeighbours;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        showNeighbours = GUILayout.Toggle(showNeighbours, "Show Neighbours");
    }

    private void OnSceneGUI()
    {
        if (showNeighbours)
            DrawGUINeighbours(target as HexCell);
    }

    void DrawGUINeighbours(HexCell cell)
    {
        if (cell.neighbours == null)
            return;

        var oldColor = Handles.color;
        Handles.color = Color.green;

        HandleUtils.CenteredLabel(cell.transform.position, cell.index.ToStringInt());

        foreach (HexCell neighbour in cell.neighbours)
        {
            Handles.DrawLine(cell.transform.position, neighbour.transform.position);
            HandleUtils.CenteredLabel(neighbour.transform.position, neighbour.index.ToStringInt());
        }

        Handles.color = oldColor;
    }
}
