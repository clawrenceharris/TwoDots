using System.Collections.Generic;
using System;
using UnityEngine;


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

/// <summary>
/// DotsObject is a class that represents base dot data on the board. Lowest level form of a dot and used as a foundation for concrete dot implementations
/// </summary>

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



