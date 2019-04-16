using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum HexType
{
    Basic1 = 0,
    Basic2 = 1,
    Basic3 = 2,
    Basic4 = 3,
    Basic5 = 4,
    Laser = 5,
    Magnet = 6
}

public class HexPiece : MonoBehaviour
{
    [SerializeField][SerializeProperty("Type")] private HexType _type;
    [SerializeField] [SerializeProperty("ColliderSize")] private float _colliderSize;

    public HexClump clump;

    public HexType Type
    {
        get { return _type; }
        set
        {
            _type = value;

#if UNITY_EDITOR
            // (HACK) Matt: GameManager Singleton is not initialized in the editor >.<
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(GameSettings).Name);
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            GameSettings gameSettings = AssetDatabase.LoadAssetAtPath<GameSettings>(path);
            GetComponent<SpriteRenderer>().sprite = gameSettings.PieceSprites[(int)_type];
#else
            GetComponent<SpriteRenderer>().sprite = GameManager.Settings.PieceSprites[(int)_type];
#endif
        }
    }    

    public float ColliderSize
    {
        get { return _colliderSize; }
        set
        {
            if (value == _colliderSize)
                return;
            
            _colliderSize = value > 0 ? value : 0.05f;

            var collider = GetComponent<PolygonCollider2D>();
            Vector2 pos = transform.position;
            Vector2[] newPoints = collider.points;

            // Collider vertices are stored as offsets. Scale each offset to scale the collider.
            for (int i = 0; i < newPoints.Length; ++i)
                newPoints[i] = newPoints[i].normalized * _colliderSize;

            collider.points = newPoints;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Type == HexType.Magnet) return; // Magnet should just spin, doesn't handle any snapping.
        if (clump == null) return;          // Pieces already attached to the magnet shouldn't handle collisions.

        var hexCell = collision.GetComponent<HexCell>();

        // What did we collide with?
        if (hexCell == null) return;
        if (hexCell.piece == null) return;
        if (hexCell.piece.clump != null) return; // If it has a clump, it's not attached to the magnet. Prevent clump-on-clump collisions.
        if (hexCell.grid != GameManager.Instance.Grid) return; // Prevent collisions with clump grid cells.

        // If we collided with a piece attached to the magnet, tell our clump we've collided.
        this.clump.OnClumpPieceCollision(this, hexCell.piece);
    }
}
