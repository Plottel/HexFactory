using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexController : MonoBehaviour
{
    [HideInInspector] public float TurnSpeed;

    private HexGrid _grid;
    private Transform _gridPivot;

    private Vector2 _lastTouchVector;

    private void Awake()
    {
        _grid = GameManager.Instance.Grid;
        _gridPivot = _grid.transform.parent;
    }

    private void Start()
    {
        TurnSpeed = GameManager.Settings.TurnSpeed;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            _gridPivot.Rotate(0, 0, TurnSpeed);

        if (Input.GetKey(KeyCode.D))
            _gridPivot.Rotate(0, 0, -TurnSpeed);
#endif

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _lastTouchVector = TouchWorldPos(touch) - (Vector2)_grid.GetCenterCell().transform.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector2 currentTouchVector = TouchWorldPos(touch) - (Vector2)_grid.GetCenterCell().transform.position;
            float deltaAngle = Vector2.SignedAngle(_lastTouchVector, currentTouchVector);
            _lastTouchVector = currentTouchVector;

            _gridPivot.Rotate(0, 0, deltaAngle);
        }

        // If TouchPhase.Moved:
        //      - record position and convert it to an angle from the center of the grid
        //      - calculate difference between this angle and the last recorded angle
        //      - rotate the grid by the difference
    }

    private Vector2 TouchWorldPos(Touch touch)
    {
        return Camera.main.ScreenToWorldPoint(touch.position);
    }
}
