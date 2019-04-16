using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid), true)]
public class HexGridInspector : Editor
{
    static bool showIndexes;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        showIndexes = GUILayout.Toggle(showIndexes, "Show Indexes");
    }

    private void OnSceneGUI()
    {
        HexGrid grid = target as HexGrid;        

        if (showIndexes)
            DrawGUIIndexes(grid);
    }

    private void DrawGUIIndexes(HexGrid grid)
    {
        foreach (HexCell cell in grid)
            HandleUtils.CenteredLabel(cell.transform.position, cell.index.ToStringInt());
    }
}
