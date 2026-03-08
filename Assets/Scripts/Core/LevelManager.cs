using UnityEngine;
using System;
using System.Collections.Generic;
public readonly struct LevelContext
{
    public readonly LevelData Level {get;}
    public readonly BoardPresenter Board {get;}
    
    public LevelContext(LevelData level, BoardPresenter board)
    {
        Level = level;
        Board = board;
    }
}
public class LevelManager : MonoBehaviour
{
    public static event Action OnLevelEnd;
    public static event Action OnLevelRestart;
    public LevelData Level { get; private set; }
    [SerializeField] private bool _isTutorial;
    private ColorSchemeService _colorSchemeService;
    [SerializeField] private ColorScheme[] _colorSchemes;
    private ConnectionPresenter _connectionPresenter;
    public bool IsTutorial
    {
        get { return _isTutorial; }
    }

    private BoardPresenter _board;
    private LevelStateManager _stateManager;
    private static ColorScheme theme;
    public ColorScheme Theme
    {
        get
        {
            if (theme == null)
            {
                theme = FindFirstObjectByType<ColorScheme>();
            }
            return theme;
        }
    }

    public static event Action<LevelContext> OnLevelSetupComplete;

    private void Awake()
    {
        _board = FindFirstObjectByType<BoardPresenter>();
        _colorSchemeService = new ColorSchemeService(_colorSchemes, 0);
        _connectionPresenter = FindFirstObjectByType<ConnectionPresenter>();
        _stateManager = FindFirstObjectByType<LevelStateManager>();
    }






    public void StartLevel(LevelData level)
    {
        Level = level;

        _connectionPresenter.Initialize(_board);

        _board.Init(level);

        _colorSchemeService.SetColorScheme(level.levelNum - 1);
        OnLevelSetupComplete?.Invoke(new LevelContext(level, _board));
        if(IsTutorial)
        {
            _stateManager.Initialize(new TutorialState(_stateManager));
        }
        else
        {
            _stateManager.Initialize(new PlayingState(_stateManager));
        }
    }
   


    public void LeaveLevel()
    {
        OnLevelEnd?.Invoke();

    }


    public void StartNextLevel()
    {
        OnLevelEnd?.Invoke();
        LevelData level = LevelLoader.LoadLevelData(Level.levelNum + 1);
        StartLevel(level);

    }


    public void Restart()
    {
        OnLevelRestart?.Invoke();
        StartLevel(Level);



    }
    
   



}
