using System;
using Dots.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// The InputRouter class handles user input for the game, translating pointer and touch events 
/// into high-level actions such as selecting/deselecting dots, connecting dots, and emitting drag positions.
/// 
/// This component is responsible for raising input-related events consumed by other systems (e.g., ConnectionPresenter).
/// 
/// Notably, InputRouter implements a throttling mechanism to prevent multiple rapid selections
/// of the same dot. The throttle window is controlled by <see cref="_selectionThrottleTime"/>, ensuring that
/// after a dot selection, subsequent selection events are ignored for a short period.
/// </summary>

public class InputRouter : MonoBehaviour
{
    public static event Action<DotPresenter> OnDotSelected;
    public static event Action<DotPresenter> OnDotConnected;
    public static event Action OnDotSelectionEnded;
    public static event Action<Vector3> OnPointerDragged;
    private Camera _cam;
    private BoardService _board;
    private static InputGate _gate;
    public static InputGate Gate => _gate ??= new InputGate();
    [SerializeField] private float _maxHitRadius =0.5f;
    public static event Action<DotPresenter> OnDotDeselected;
    private bool _isPointerDown;
    private float _lastSelectionTime = -100f;
    private IDotPresenter _lastSelectedDot;
    [SerializeField] private float _selectionThrottleTime = 0.08f;

    private void Awake()
    {
        _cam = Camera.main;
        _gate = new InputGate();
    }
    private void Start()
    {
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return;
        _board = boardService;
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
            EndSelection();
        }
        else if (_isPointerDown)
        {   
            EmitPointerDragPosition();
            RecordPointerMove();
        }



    }
    private void SelectDot(DotPresenter dot)
    {
        if (dot.Dot.ID == _lastSelectedDot?.Dot.ID) return;

        if (_isPointerDown && Time.unscaledTime - _lastSelectionTime < _selectionThrottleTime) return;
        _isPointerDown = true;
        _lastSelectedDot = dot;
        _lastSelectionTime = Time.unscaledTime;
        OnDotSelected?.Invoke(dot);
    }
    private void ConnectDot(DotPresenter dot)
    {
        if (dot.Dot.ID == _lastSelectedDot?.Dot.ID) return;

        if (_isPointerDown && Time.unscaledTime - _lastSelectionTime < _selectionThrottleTime) return;
        _lastSelectedDot = dot;
        _lastSelectionTime = Time.unscaledTime;
        OnDotConnected?.Invoke(dot);


    }
    
    private void RecordPointerMove()
    {
        if (!TryGetPointerHit(out DotPresenter dot)) return;
        ConnectDot(dot);
    }

    private bool TryGetPointerHit(out DotPresenter dot)
    {
        dot = null;
        if (!TryGetPointerScreenPosition(out Vector2 position)) return false;

        var worldPosition = _cam.ScreenToWorldPoint(position);
        var gridPosition = GridUtility.WorldToGrid(worldPosition);

        DotPresenter bestDot = null;

        // Check the clicked cell and its neighbors
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                var candidateGrid = new Vector2Int(gridPosition.x + dx, gridPosition.y + dy);
                var candidate = _board.BoardPresenter.GetDotAt(candidateGrid);
                if (candidate == null) continue;

                var candidateWorld = candidate.DotView.transform.position;
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
        if (!TryGetPointerHit(out DotPresenter dot)) return;
        SelectDot(dot);
    }

    private void EndSelection()
    {
        
        _isPointerDown = false;
        _lastSelectedDot = null;
        _lastSelectionTime = -100f;
        OnDotSelectionEnded?.Invoke();
        TryGetPointerHit(out DotPresenter dot);
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