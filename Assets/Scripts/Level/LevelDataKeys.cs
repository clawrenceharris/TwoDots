



using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class LevelDataKeys
{
        public const string Type = "t";
        public const string Color = "c";

        public const string Colors = "colors";

        public const string Direction = "d";
        public const string Number = "n";
        public const string Active = "a";

        public const string Position = "p";
        public const string IsActive = "a";
        public const string Width = "w";
        public const string Moves = "moves";
        public const string LevelNum = "levelNum";

        public const string Height = "h";
        public const string InitDotsToSpawn = "initDotsToSpawn";
        public const string DotsToSpawn = "dotsToSpawn";

        public const string DotsOnBoard = "dotsOnBoard";
        public const string HitCount = "h";

        public const string TilesOnBoard = "tilesOnBoard";


        public static class  DotColors{
                public const string Red = "r";
                public const string Yellow = "y";
                public const string Green = "g";
                public const string Blue = "b";
                public const string Purple = "p";  

                public const string Blank = "x";

        public static readonly Dictionary<string, DotColor> dotColorMap = new Dictionary<string, DotColor>()
        {
            { Red, DotColor.Red },
            { Yellow, DotColor.Yellow },
            { Green, DotColor.Green },
            { Blue, DotColor.Blue },
            { Purple, DotColor.Purple },
            { Blank, DotColor.Blank } };
        public static DotColor GetDotColorFromKey(string cKey)
        {
            if (dotColorMap.TryGetValue(cKey, out DotColor value))
            {
                return value;
            }
            throw new ArgumentException($"Dot color key '{cKey}' not found.");
        }

        public static JToken  GetKeyFromDotColor(DotColor dotColor)
        {
            if (dotColorMap.TryGetValue(dotColor.ToString(), out DotColor value))
            {
                return new JValue(value);
            }
            throw new ArgumentException($"Dot color key '{dotColor}' not found.");
        }
    }
        public static class Types{
                public const string Normal = "n";
                public const string Beetle = "bt";
                public const string Bomb = "b";
                public const string Nesting = "nt";
                public const string Anchor = "a";
                public const string Monster = "m";
                public const string Lotus = "l";
                public const string Clock = "c";
                public const string SquareGem = "sg";
                public const string RectangleGem = "rg";

                public const string Blank = "x";
                public const string Ice = "i";
                public const string Water = "w";
                public const string Block = "b";

                public const string Circuit = "ct";
                public const string OneSidedBlock = "osb";
                public const string EmptyTile = "e";

        



         public static readonly Dictionary<string, DotType> dotTypeMap = new Dictionary<string, DotType>()
        {
            { Normal, DotType.Normal },
            { Clock, DotType.Clock },
            { Bomb, DotType.Bomb },
            { Blank, DotType.Blank },
            { Anchor, DotType.Anchor },
            { Nesting, DotType.Nesting },
            { Beetle, DotType.Beetle },
            { Monster, DotType.Monster },
            { Lotus, DotType.Lotus },
            { SquareGem, DotType.SquareGem },
            { RectangleGem, DotType.RectangleGem }
        };

        public static readonly Dictionary<string, TileType> tileTypeMap = new Dictionary<string, TileType>()
        {
            { OneSidedBlock, TileType.OneSidedBlock },
            { EmptyTile, TileType.EmptyTile },
            { Ice, TileType.Ice },
            { Water, TileType.Water },
            { Circuit, TileType.Circuit },
            { Block, TileType.Block }
        };

        public static DotType GetDotTypeFromKey(string key)
        {
            if (dotTypeMap.TryGetValue(key, out DotType value))
            {
                return value;
            }
            throw new ArgumentException($"Dot type key '{key}' not found.");
        }

        public static JToken GetKeyFromDotType(string type)
        {
            if (dotTypeMap.TryGetValue(type, out DotType value))
            {
                return new JValue(value);
            }
            throw new ArgumentException($"Dot type key '{type}' not found.");
        }

        public static JToken GetKeyFromTileType(string type)
        {
           
            if (tileTypeMap.TryGetValue(type, out TileType value))
            {
                return new JValue(value);
            }
            throw new ArgumentException($"Tile type key '{type}' not found.");
        }

        public static TileType GetTileTypeFromKey(string key)
        {
            if (tileTypeMap.TryGetValue(key, out TileType value))
            {
                return value;
            }
            throw new ArgumentException($"Tile type key '{key}' not found.");
        }
    }
}

