using System;
using Dots.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour
{
    public static event Action<IDotPresenter> OnDotSelected;
    public static event Action<IDotPresenter> OnDotConnected;
    public static event Action OnDotSelectionEnded;
    public static event Action<Vector3> OnPointerDragged;
    private Camera _cam;
    private IBoardPresenter _board;
    private static InputGate _gate;
    public static InputGate Gate => _gate ??= new InputGate();
    [SerializeField] private float _maxHitRadius =0.5f;
    public static event Action<IDotPresenter> OnDotDeselected;

    private bool _isPointerDown;
    private void Awake()
    {
        _cam = Camera.main;
        _gate = new InputGate();
        _board = FindFirstObjectByType<BoardPresenter>();
    }

    private void Update()
    {
        if (!_gate.Enabled) return;



        if (IsPointerPressDown())
        {
            RecordPointerDown();
        }
        else if (IsPointerPressUp())
        {
            HandlePointerUp();
        }
        else if (_isPointerDown)
        {
            EmitPointerDragPosition();
            RecordPointerMove();
        }



    }
    
    private void RecordPointerMove()
    {
        if (!TryGetPointerHit(out IDotPresenter dot)) return;
        OnDotConnected?.Invoke(dot);
    }

    private bool TryGetPointerHit(out IDotPresenter dot)
    {
        dot = null;
        if (!TryGetPointerScreenPosition(out Vector2 position)) return false;

        var worldPosition = _cam.ScreenToWorldPoint(position);
        var gridPosition = GridUtility.WorldToGrid(worldPosition);

        IDotPresenter bestDot = null;

        // Check the clicked cell and its neighbors
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                var candidateGrid = new Vector2Int(gridPosition.x + dx, gridPosition.y + dy);
                var candidate = _board.GetDotAt(candidateGrid);
                if (candidate == null) continue;

                var candidateWorld = candidate.View.transform.position;
                if (Mathf.Abs(candidateWorld.x - worldPosition.x) <= _maxHitRadius && Mathf.Abs(candidateWorld.y - worldPosition.y) <= _maxHitRadius)
                {
                    bestDot = candidate;
                }
            }
        }

        dot = bestDot;
        return dot != null;

    }

    private void RecordPointerDown()
    {
        _isPointerDown = true;
        if (!TryGetPointerHit(out IDotPresenter dot)) return;
        OnDotSelected?.Invoke(dot);
    }

    private void HandlePointerUp()
    {
        _isPointerDown = false;
        OnDotSelectionEnded?.Invoke();
        if (!TryGetPointerHit(out IDotPresenter dot)) return;
        OnDotDeselected?.Invoke(dot);
    }

    private void EmitPointerDragPosition()
    {
        if (!TryGetPointerScreenPosition(out Vector2 position)) return;
        var worldPosition = _cam.ScreenToWorldPoint(position);
        worldPosition.z = 0;
        OnPointerDragged?.Invoke(worldPosition);
    }
    private bool IsPointerPressDown()
    {
        return Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
    }

    private bool IsPointerPressUp()
    {
        return Pointer.current != null && Pointer.current.press.wasReleasedThisFrame;
    }

    private bool TryGetPointerScreenPosition(out Vector2 position)
    {
        position = Vector2.zero;
        if (Pointer.current == null) return false;
        position = Pointer.current.position.ReadValue();
        return true;
    }
}