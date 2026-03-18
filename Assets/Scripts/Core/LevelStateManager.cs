using System;
using UnityEngine;

public class LevelStateManager : MonoBehaviour
{
    public IState CurrentState => _stateMachine.CurrentState;
    private int _moveCount;
    public int MoveCount
    {
        get { return _moveCount; }
        set
        {
            _moveCount = value;
            OnMoveCountChanged?.Invoke(_moveCount);
        }
    }
   
    private IStateMachine _stateMachine;
    public event Action<int> OnMoveCountChanged;


    private GameManager _gameManager;
    private ITutorialPresenter _tutorial;
    public ITutorialPresenter Tutorial => _tutorial;
    public GameManager GameManager => _gameManager;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _tutorial = new TutorialPresenter();
        _gameManager = FindFirstObjectByType<GameManager>();
    }


    public void Initialize(IState initialState)
    {
        _stateMachine.Initialize(initialState);
        MoveCount = 0;
    }

    public void ChangeState(IState state)
    {
        _stateMachine.SetState(state);
    }
    private void Update()
    {
        _stateMachine.Update();
    }
    


    public void IncrementMoveCount()
    {
        MoveCount++;
    }

    public void Reset()
    {
        _stateMachine.SetState(null);
        MoveCount = 0;
        
    }

}