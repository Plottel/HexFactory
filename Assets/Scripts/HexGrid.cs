using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

// A simple wrapper to allow List<List<HexCell>> to be serialized.
[System.Serializable]
public class HexCellList
{
    public List<HexCell> cells = new List<HexCell>();
}

public class HexGrid : MonoBehaviour, IEnumerable<HexCell>, IEnumerable
{
    private static readonly float kRoot3 = Mathf.Sqrt(3); // Derived from Sin(60)

    private static readonly Vector2[][] kNeighbourIndexOffsets = new Vector2[][]
    {
        // Offset column
        new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(0, 1)
        },        

        // Normal column
        new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(1, -1),
            new Vector2(0, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 0),
            new Vector2(0, 1)
        }
    };

    [SerializeField]
    [HideInInspector]
    private List<HexCellList> _cellList;

    [SerializeField][SerializeProperty("CellSize")]
    private float _cellSize;

    [SerializeField][SerializeProperty("Columns")]
    private int _columns;

    [SerializeField][SerializeProperty("Rows")]
    private int _rows;

    public HexCell CellPrefab;

    public float CellSize
    {
        get { return _cellSize; }
        set
        {
            if (_cellSize == value)
                return;

            _cellSize = value;
            ResetGrid();
        }
    }

    private float HexWidth
    {
        get { return CellSize * 2; }
    }

    private float HexHeight
    {
        get { return CellSize * kRoot3; }
    }

    private float HexOffsetX
    {
        get { return HexWidth * 0.75f; }
    }

    private float HexOffsetY
    {
        get { return HexHeight; }
    }

    private float EvenRowOffsetY
    {
        get { return HexHeight / 2; }
    }

    public int Columns
    {
        get { return _columns; }
        set
        {
            if (_columns == value)
                return;

            _columns = value;
            ResetGrid();
        }
    }

    public int Rows
    {
        get { return _rows; }
        set
        {
            if (_rows == value)
                return;

            _rows = value;
            ResetGrid();
        }
    } 

    public void SetColumnsNoReset(int newColumnCount)
    {
        _columns = newColumnCount;
    }

    public void SetRowsNoReset(int newRowCount)
    {
        _rows = newRowCount;
    }

    public void SetCenterPos(Vector2 newCenterPos)
    {
        float halfWidth = (Columns / 2) * HexOffsetX;
        float halfHeight = (Rows / 2) * HexOffsetY;

        newCenterPos.x -= halfWidth;
        newCenterPos.y -= halfHeight;

        transform.position = newCenterPos;
    }

    public bool IsOffsetColumn(int columnIndex)
    {
        return columnIndex % 2 == 0;
    }

    public HexCell WorldPosToCell(Vector2 worldPos)
    {
        Collider2D[] collisions = Physics2D.OverlapPointAll(worldPos);

        foreach (var collider in collisions)
        {
            var cell = collider.GetComponent<HexCell>();

            if (cell != null && cell.grid == this)
                return cell;
        }

        return null;
    }

    public HexCell GetCenterCell()
    {
        int col = Mathf.FloorToInt(Columns / 2);
        int row = Mathf.FloorToInt(Rows / 2);

        return _cellList[col].cells[row];
    }

    public HexCell IndexToCell(Vector2 index)
    {
        return _cellList[(int)index.x].cells[(int)index.y];
    }

    public Vector3 IndexToCellPos(Vector2 index)
    {
        return _cellList[(int)index.x].cells[(int)index.y].transform.position;
    }

    #region GRID CONSTRUCTION

    public void ResetGrid()
    {
        //
        // Destroy the grid before recreating it.
        //
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
#if UNITY_EDITOR

            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        _cellList = new List<HexCellList>();

        //
        // Construct columns and rows.
        //
        for (int col = 0; col < Columns; ++col)
        {
            HexCellList newColumn = new HexCellList();

            bool shouldOffset = IsOffsetColumn(col);

            for (int row = 0; row < Rows; ++row)
            {
                Vector3 newCellLocalPos = new Vector3
                (
                    col * HexOffsetX,
                    row * HexOffsetY,
                    0
                );

                if (shouldOffset)
                    newCellLocalPos.y += EvenRowOffsetY;

                // Create cell and add to in-construction column.
                HexCell newCell = CreateCell(newCellLocalPos, col, row);
                newColumn.cells.Add(newCell);
            }

            // Add new column to cell storage.
            _cellList.Add(newColumn);
        }

        //
        // Calculate neighbours for each cell.
        //
        for (int col = 0; col < Columns; ++col)
        {
            int neighbourSetIndex = IsOffsetColumn(col) ? 0 : 1; // Normal neighbours or offset neighbours? 

            for (int row = 0; row < Rows; ++row)
            {
                List<HexCell> neighbours = new List<HexCell>();
                HexCell cell = _cellList[col].cells[row];

                foreach (Vector2 indexOffset in kNeighbourIndexOffsets[neighbourSetIndex])
                {
                    Vector2 index = cell.index + indexOffset;

                    if (index.x < 0 || index.x >= Columns || index.y < 0 || index.y >= Rows)
                        continue;

                    neighbours.Add(_cellList[(int)index.x].cells[(int)index.y]);
                }

                cell.neighbours = neighbours;
            }
        }
    }

    private HexCell CreateCell(Vector3 pos, int column, int row)
    {
        HexCell newCell = Instantiate(CellPrefab, transform);
        newCell.transform.localPosition = pos;
        newCell.grid = this;
        newCell.index = new Vector2(column, row);

        var colliderPoints = new Vector2[6]
        {
            CellColliderPoint(newCell, 0),
            CellColliderPoint(newCell, 60),
            CellColliderPoint(newCell, 120),
            CellColliderPoint(newCell, 180),
            CellColliderPoint(newCell, 240),
            CellColliderPoint(newCell, 300)
        };

        newCell.GetComponent<PolygonCollider2D>().points = colliderPoints;
        newCell.vertices = colliderPoints;

        return newCell;
    }

    private Vector2 CellColliderPoint(HexCell cell, float angleDegrees)
    {
        Vector3 center = cell.transform.localPosition;
        float angleRadians = Mathf.PI / 180 * angleDegrees;

        return new Vector2
        (
            CellSize * Mathf.Cos(angleRadians),
            CellSize * Mathf.Sin(angleRadians)
        );
    }

    private void OnDrawGizmos()
    {
        if (CellPrefab == null || Rows == 0 || Columns == 0)
            return;

        var oldColor = Gizmos.color;
        Gizmos.color = Color.white;

        for (int col = 0; col < Columns; ++col)
        {
            for (int row = 0; row < Rows; ++row)
                DrawCellGizmo(_cellList[col].cells[row]);
        }

        Gizmos.color = oldColor;
    }

    private void DrawCellGizmo(HexCell cell)
    {
        Vector2 pos = cell.transform.position;

        for (int i = 0; i < 5; ++i)
            Gizmos.DrawLine(pos + cell.vertices[i], pos + cell.vertices[i + 1]);

        Gizmos.DrawLine(pos + cell.vertices[5], pos + cell.vertices[0]);
    }

    #endregion

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerator<HexCell> GetEnumerator()
    {
        for (int col = 0; col < Columns; ++col)
        {
            for (int row = 0; row < Rows; ++row)
                yield return _cellList[col].cells[row];
        }
    }
}
