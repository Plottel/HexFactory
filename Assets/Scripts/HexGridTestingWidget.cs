using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HexGridTestingWidget : MonoBehaviour
{
    public HexGrid grid;

    private HexCell _cellAtWidgetPos;

    private void OnDrawGizmos()
    {
        _cellAtWidgetPos = grid.WorldPosToCell(transform.position);

        if (_cellAtWidgetPos == null)
            return;

        Gizmos.DrawSphere(_cellAtWidgetPos.transform.position, 0.35f);
    }
}
