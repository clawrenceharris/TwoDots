using System.Collections.Generic;
using System;
using UnityEngine;


// [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]

// public class Level : ScriptableObject
// {
//     public int LevelNumber;
//     public int Width;
//     public int Height;
//     public int MaxMoves;
//     public string[] LevelColors;

//     [NonReorderable]public List<DotSpawnData> InitialDotsToSpawn;
//     [NonReorderable]public List<DotSpawnData> DotsToSpawn;
//     [NonReorderable]public List<DotSpawnData> DotsOnBoard;
//     [NonReorderable]public List<TileSpawnData> TilesOnBoard;
//     public bool IsTutorial;
//     [NonReorderable]public TutorialStep[] TutorialSteps;
// }

[Serializable]
public class LevelData
{
    public int levelNum;
    public int width;
    public int height;
    public int moves;
    public string[] colors;

    [NonReorderable]public DotsObject[] initDotsToSpawn;

    [NonReorderable]public DotsObject[] dotsToSpawn;
    [NonReorderable]public DotsObject[] dotsOnBoard;
    [NonReorderable]public DotsObject[] tilesOnBoard;
    public bool isTutorial;
    [NonReorderable]public TutorialStep[] tutorialSteps;
}



[Serializable]
public class DotsObject
{
    public int Col;
    public int Row;
    public int HitCount;
    public string Type;
    
    public enum Property{
        Colors,
        Directions,
        Type,
        Number,

        Active,
        Position
    }
    // Dictionary to hold dynamic properties
    private readonly Dictionary<Property, object> properties = new();

    public T GetProperty<T>(Property key)
    {
        if (properties.ContainsKey(key))
        {
            return (T)properties[key];
        }
        return default;
    }

    public void SetProperty<T>(Property key, T value)
    {
        if (properties.ContainsKey(key))
        {
            properties[key] = value;
        }
        else
        {
            properties.Add(key, value);
        }
    }
}


[Serializable]
public class GoalData
{
    public string type;
    public int amount;
}

[Serializable]
public class TutorialStep
{
    public string topText;
    public string bottomText;
    public PositionData[] connection;
}

[Serializable]
public class PositionData
{
    public int row;
    public int column;
}



[Serializable]
public class TileSpawnData : SpawnData
{
    public TileType Type = TileType.None;
}

[Serializable]
public class DotSpawnData : SpawnData
{
    public DotType Type = DotType.None;
}



[Serializable]
public class SpawnData
{
    public Vector2Int GridPosition;   
    
    // Dictionary to hold dynamic properties
    public readonly Dictionary<string, object> Properties = new();

    public T GetProperty<T>(string key)
    {
        if (Properties.ContainsKey(key))
        {
            return (T)Properties[key];
        }
        return default;
    }

    public void SetProperty<T>(string key, T value)
    {
        if (Properties.ContainsKey(key))
        {
            Properties[key] = value;
        }
        else
        {
            Properties.Add(key, value);
        }
    }

}



