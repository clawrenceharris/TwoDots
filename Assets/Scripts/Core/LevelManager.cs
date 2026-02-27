using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static event Action OnLevelEnd;
    public static event Action OnLevelRestart;
    public LevelData Level { get; private set; }
    [SerializeField] private bool isTutorial;
    private ColorSchemeManager colorSchemeManager;
    public static int MoveCount { get; private set; } = 0;
    public bool IsHighscore { get; private set; }
    public int score;
    [SerializeField] private ColorScheme[] colorSchemes;
    private ConnectionPresenter _connectionPresenter;
    public bool IsTutorial
    {
        get { return isTutorial; }
    }

    private BoardPresenter _board;

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

    private void Awake()
    {
        _board = FindFirstObjectByType<BoardPresenter>();
        colorSchemeManager = new ColorSchemeManager(colorSchemes, 0);
        _connectionPresenter = FindFirstObjectByType<ConnectionPresenter>();
    }



   


    public void StartLevel(LevelData level)
    {
        Level = level;

        _connectionPresenter.Initialize(new BaseConnectionRule(), _board);
        
        _board.Initialize(level);

        colorSchemeManager.SetColorScheme(level.levelNum - 1);

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
