using Newtonsoft.Json.Linq;
using UnityEngine;

public class TileDataFactory
{
    

    public static DotsObject CreateTileData(JObject itemObject)
    {
        string type = (string)itemObject[LevelDataKeys.Type];
        JToken position = itemObject[LevelDataKeys.Position];
        JToken hitCount = itemObject[LevelDataKeys.HitCount];
        JToken isActive = itemObject[LevelDataKeys.Active];
        JToken direction = itemObject[LevelDataKeys.Direction];
        DotsObject tileData = new(){
            HitCount = hitCount != null ? (int)hitCount : 0,
            Type = type,
            Col = position != null ? position.ToObject<int[]>()[0] : -1,
            Row = position != null ? position.ToObject<int[]>()[1] : -1,
        };
        switch (type)
        {
            case LevelDataKeys.Types.OneSidedBlock:
                // Expecting direction to be a 1D array, not a 2D array
                tileData.SetProperty(DotsObject.Property.Directions,  direction.ToObject<int[]>());
                break;
            case LevelDataKeys.Types.Circuit:
                tileData.SetProperty(DotsObject.Property.Active, (bool)isActive);
                break;

        }
        return tileData;
    }
}
