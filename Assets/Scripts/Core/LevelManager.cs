using UnityEngine;
using System;
using System.Collections.Generic;
public readonly struct LevelContext
{
    public readonly LevelData Level {get;}
    
    public LevelContext(LevelData level)
    {
        Level = level;
    }
}
public class LevelManager : MonoBehaviour
{
    public static event Action OnLevelEnd;
    public static event Action OnLevelRestart;
    public LevelData Level { get; private set; }
    [SerializeField] private bool _isTutorial;
    public bool IsTutorial
    {
        get { return _isTutorial; }
    }
    [SerializeField]private TextAsset startingLevel;
    private CameraPresenter _camera;
    private LevelStateManager _stateManager;
    private CascadeRunner _cascadeRunner;
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

        _stateManager = FindFirstObjectByType<LevelStateManager>();
        _camera = FindFirstObjectByType<CameraPresenter>();
        _cascadeRunner = FindFirstObjectByType<CascadeRunner>();

       

        ServiceProvider.Instance.RegisterServices();
    }



    private void Start()
    {

       
        StartLevel();

    }

    public void StartLevel(LevelData level = null)
    {
        Level = level ?? LevelLoader.LoadLevelData(startingLevel);
        if (Level == null)
        {
            Debug.LogError("Level is null");
            return;
        }
        
        var colorScheme = ServiceProvider.Instance.GetService<ColorSchemeService>();
        var board = ServiceProvider.Instance.GetService<BoardService>();
        var connection = ServiceProvider.Instance.GetService<ConnectionService>();
        var pool = ServiceProvider.Instance.GetService<PoolService>();

        // Set up the services
        colorScheme.Initialize(Level.levelNum - 1);


        board.Initialize();
        connection.Initialize(board.BoardPresenter);
        _cascadeRunner.Init(connection.ActiveConnection);
        board.SetupBoard(Level);

        pool.RegisterPools();
        pool.FillPool<LinePool>();
        pool.FillPool<BombPool>();


        _camera.SetUpCameraForLevel(Level);
        if (IsTutorial)
        {
            _stateManager.Initialize(new TutorialState(_stateManager));
        }
        else
        {
            _stateManager.Initialize(new PlayingState(_stateManager));
        }

        OnLevelSetupComplete?.Invoke(new LevelContext(level));

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
