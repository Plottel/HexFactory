using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    [ReadOnly] public Vector2 index;

    [HideInInspector] public HexGrid grid;
    [HideInInspector] public Vector2[] vertices;
    [HideInInspector] public List<HexCell> neighbours;
    [HideInInspector] public HexPiece piece;

    public bool IsEmpty
    {
        get { return piece == null; }
    }

    // [NOTE] Matt: Only cells with pieces can be considered isolated.
    public bool IsIsolated
    {
        get
        {
            if (IsEmpty || piece.Type == HexType.Magnet) // Magnet can't be isolated.
                return false;

            // Search neighbours outwards to see if this cell's piece is connected to the magnet.
            var toCheck = new Stack<HexCell>();
            var haveChecked = new List<HexCell>();

            toCheck.Push(this);
            HexCell current;

            while (toCheck.Count > 0)
            {
                current = toCheck.Pop();
                haveChecked.Add(current);

                foreach (HexCell neighbour in current.neighbours)
                {
                    if (haveChecked.Contains(neighbour) || toCheck.Contains(neighbour))
                        continue;

                    if (!neighbour.IsEmpty)
                    {
                        if (neighbour.piece.Type == HexType.Magnet)
                            return false;

                        toCheck.Push(neighbour);
                    }
                }
            }

            // No magnet piece was found, therefore cell is isolated.
            return true;
        }
    }

    // TODO : This is a potential performance hit. Shouldn't need to create a list every time this is queried.
    public HexCell GetRandomEmptyNeighbour()
    {
        var emptyNeighbours = new List<HexCell>();

        foreach (HexCell cell in neighbours)
        {
            if (cell.IsEmpty)
                emptyNeighbours.Add(cell);
        }

        return emptyNeighbours.Count == 0 ? null : emptyNeighbours[Random.Range(0, emptyNeighbours.Count)];
    }
}