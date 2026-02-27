
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class LevelLoader
{
    public static LevelData Level;
    public static LevelData LoadLevelData(TextAsset textAsset){

        var settings = new JsonSerializerSettings
        {
            Converters = { new LevelDataConverter() }
        };
        Level = JsonConvert.DeserializeObject<LevelData>(textAsset.text, settings);
        return Level;
    }

    
    public static LevelData LoadLevelData(int levelNum)
    {
        string path = Application.dataPath + "/Json/World " + (Game.Instance.WorldIndex + 1) + "/level_" + levelNum + ".json";
        string json = File.ReadAllText(path);

        var settings = new JsonSerializerSettings
        {
            Converters = { new LevelDataConverter() }
        };
        LevelData level = JsonConvert.DeserializeObject<LevelData>(json, settings);
        return level;
    }
   

 public static T FromJsonType<T>(string typeKey) where T : Enum
        {
            if (typeof(T) == typeof(DotType))
            {
                if (LevelDataKeys.Types.dotTypeMap.TryGetValue(typeKey, out DotType dotType))
                {
                    return (T)(object)dotType; // Correct cast
                }
            }
            else if (typeof(T) == typeof(TileType))
            {
                if (LevelDataKeys.Types.tileTypeMap.TryGetValue(typeKey, out TileType tileType))
                {
                    return (T)(object)tileType; // Correct cast
                }
            }

            throw new ArgumentException($"Type key '{typeKey}' is not a valid {typeof(T).Name} type.");
        }
    public static TileType FromJsonTileType(string type)
    {
        return type switch
        {
            LevelDataKeys.Types.Ice => TileType.Ice,
            "slime" => TileType.Slime,
            LevelDataKeys.Types.Block => TileType.Block,
            LevelDataKeys.Types.OneSidedBlock => TileType.OneSidedBlock,
            LevelDataKeys.Types.EmptyTile => TileType.EmptyTile,
           LevelDataKeys.Types.Water => TileType.Water,
            LevelDataKeys.Types.Circuit => TileType.Circuit,
            _ => TileType.None,
        };
    }

   
    public static DotColor FromJsonColor(string color)
    {
        return color switch
        {
            LevelDataKeys.DotColors.Red => DotColor.Red,
            LevelDataKeys.DotColors.Blue => DotColor.Blue,
            LevelDataKeys.DotColors.Yellow => DotColor.Yellow,
            LevelDataKeys.DotColors.Purple => DotColor.Purple,
            LevelDataKeys.DotColors.Green => DotColor.Green,
            LevelDataKeys.DotColors.Blank => DotColor.Blank,
            _ => throw new ArgumentException(),
        };
    }

    public static string ToJsonDotType(DotType type)
    {
        return type switch
        {
            DotType.Normal => LevelDataKeys.Types.Normal,
            DotType.Clock =>  LevelDataKeys.Types.Clock,
            DotType.Bomb => LevelDataKeys.Types.Bomb,
            DotType.Blank => LevelDataKeys.Types.Blank,
            DotType.Anchor => LevelDataKeys.Types.Anchor,
            DotType.Lotus => LevelDataKeys.Types.Lotus,

            DotType.Nesting => LevelDataKeys.Types.Nesting,
            DotType.Beetle => LevelDataKeys.Types.Beetle,
            DotType.SquareGem => LevelDataKeys.Types.SquareGem,
            DotType.RectangleGem => LevelDataKeys.Types.RectangleGem,
            _ => throw new ArgumentException(),
        };
    }
    public static string ToJsonTileType(TileType type)
    {
        return type switch
        {
            TileType.Slime => "slime",
            TileType.Ice => LevelDataKeys.Types.Ice,
            TileType.Block => LevelDataKeys.Types.Block,
            TileType.OneSidedBlock => LevelDataKeys.Types.OneSidedBlock,
            TileType.Water => LevelDataKeys.Types.Water,
            TileType.Circuit => LevelDataKeys.Types.Circuit,
            _ => throw new ArgumentException(),
        };
    }

    public static object ToJsonColor(DotColor color)
    {
        return color switch
        {
            DotColor.Red => LevelDataKeys.DotColors.Red,
            DotColor.Blue => LevelDataKeys.DotColors.Blue,
            DotColor.Yellow =>LevelDataKeys.DotColors.Yellow,
            DotColor.Purple => LevelDataKeys.DotColors.Purple,
            DotColor.Green => LevelDataKeys.DotColors.Green,
            _ => DotColor.Blank,
        };
    }
}
