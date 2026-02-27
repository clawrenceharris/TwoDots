using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelDataConverter : JsonConverter<LevelData>
{
    public override LevelData ReadJson(JsonReader reader, Type objectType, LevelData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Load JSON into a JObject to inspect its properties
        JObject jsonObject = JObject.Load(reader);
        // Deserialize common properties
        LevelData levelData = new()
        {
            levelNum = (int)jsonObject[LevelDataKeys.LevelNum],
            width = (int)jsonObject[LevelDataKeys.Width],
            height = (int)jsonObject[LevelDataKeys.Height],
            moves = (int)jsonObject[LevelDataKeys.Moves],
            colors = jsonObject[LevelDataKeys.Colors].ToObject<string[]>()

        };

        // Deserialize arrays
        JArray dotsToSpawnArray = (JArray)jsonObject[LevelDataKeys.DotsToSpawn];
        levelData.dotsToSpawn = DeserializeDotsArray(dotsToSpawnArray);

        JArray dotsOnBoardArray = (JArray)jsonObject[LevelDataKeys.DotsOnBoard];
        levelData.dotsOnBoard = DeserializeDotsArray(dotsOnBoardArray);

        
        JArray initialDotsToSpawn = (JArray)jsonObject[LevelDataKeys.InitDotsToSpawn];
        levelData.initDotsToSpawn = DeserializeDotsArray(initialDotsToSpawn);

        
        JArray tilesOnBoardArray = (JArray)jsonObject[LevelDataKeys.TilesOnBoard];
        levelData.tilesOnBoard = DeserializeTilesArray(tilesOnBoardArray);

        return levelData;
    }
    private DotsObject[] DeserializeDotsArray(JArray array)

    {

        DotsObject[] deserializedArray = new DotsObject[array.Count];
        for (int i = 0; i < array.Count; i++)
        {
            JObject itemObject = (JObject)array[i];
            deserializedArray[i] = DotDataFactory.CreateDotData(itemObject);
        }

        return deserializedArray;
    }


    private DotsObject[] DeserializeTilesArray(JArray array)

    {

        DotsObject[] deserializedArray = new DotsObject[array.Count];
        for (int i = 0; i < array.Count; i++)
        {
            JObject itemObject = (JObject)array[i];
            deserializedArray[i] = TileDataFactory.CreateTileData(itemObject);
        }

        return deserializedArray;
    }

    public override void WriteJson(JsonWriter writer, LevelData value, JsonSerializer serializer)
    {

        throw new NotImplementedException();

    }
}