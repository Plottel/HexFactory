using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputMethod
{
    TapLeftRight,
    SwipeRotate
}

public class HexController : MonoBehaviour
{
    [HideInInspector] public float TurnSpeed;

    private InputMethod _inputMethod;
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
        _inputMethod = GameManager.Settings.InputMethod;
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

        if (_inputMethod == InputMethod.TapLeftRight)
            HandleTapLeftRightInput(touch);
        else if (_inputMethod == InputMethod.SwipeRotate)
            HandleSwipeRotateInput(touch);
    }

    private void HandleTapLeftRightInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
        {
            float touchX = touch.position.x;

            if (touchX < Screen.width / 2)
                _gridPivot.Rotate(0, 0, TurnSpeed);
            else
                _gridPivot.Rotate(0, 0, -TurnSpeed);
        }
    }

    private void HandleSwipeRotateInput(Touch touch)
    {       
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
    }

    private Vector2 TouchWorldPos(Touch touch)
    {
        return Camera.main.ScreenToWorldPoint(touch.position);
    }
}
