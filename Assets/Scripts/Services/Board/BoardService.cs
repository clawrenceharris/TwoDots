using System;
using UnityEngine;
public class BoardService : MonoBehaviour
{
    public static BoardService Instance { get; private set; }
    private BoardPresenter _boardPresenter;
    public IBoardPresenter BoardPresenter => _boardPresenter;
    private BoardView _boardView;
    private DotSpawner _dotSpawner;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _boardView = FindFirstObjectByType<BoardView>();
        _dotSpawner = FindFirstObjectByType<DotSpawner>();
       
    }
    
    public void Initialize()
    {
        _boardPresenter = new BoardPresenter();
        _boardPresenter.Initialize(_boardView, _dotSpawner);
    }

    public void SetupBoard(LevelData level)
    {
        if (_boardPresenter == null)
        {
            Debug.LogError("[BoardService] BoardPresenter is null");
            return;
        }
        _boardPresenter.SetupBoard(level);
    }
}