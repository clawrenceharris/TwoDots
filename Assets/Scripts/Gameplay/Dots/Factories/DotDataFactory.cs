using Newtonsoft.Json.Linq;

public class DotDataFactory
{

    public static DotsObject CreateDotData(JObject itemObject)
    {
        string type = (string)itemObject[LevelDataKeys.Type];
        JToken position = itemObject[LevelDataKeys.Position];
        JToken color = itemObject[LevelDataKeys.Color];
        JToken hitCount = itemObject[LevelDataKeys.HitCount];
        JToken direction = itemObject[LevelDataKeys.Direction];
        JToken number = itemObject[LevelDataKeys.Number];
        DotsObject dotData = new()
        {
            Type = type,
            HitCount = hitCount != null ? (int)hitCount : 0,
            Col = position != null ? position.ToObject<int[]>()[0]  : -1,
            Row = position != null ? position.ToObject<int[]>()[1]  : -1,

        };
        switch (type)
        {
            case LevelDataKeys.Types.Lotus:
            case LevelDataKeys.Types.Normal:
            case LevelDataKeys.Types.Blank:
                dotData.SetProperty(DotsObject.Property.Colors, color?.ToObject<string[]>());
                break;
            case LevelDataKeys.Types.Beetle:
                dotData.SetProperty(DotsObject.Property.Colors, color?.ToObject<string[]>());
                dotData.SetProperty(DotsObject.Property.Directions, direction.ToObject<int[,]>());
                break;
            case LevelDataKeys.Types.Clock:
                dotData.SetProperty(DotsObject.Property.Number, number.ToObject<int[]>());
                dotData.SetProperty(DotsObject.Property.Colors, color?.ToObject<string[]>());

                break;
            
            case LevelDataKeys.Types.Monster:
                dotData.SetProperty(DotsObject.Property.Colors, color?.ToObject<string[]>());
                dotData.SetProperty(DotsObject.Property.Number, number.ToObject<int[]>());

                break;
            
            case LevelDataKeys.Types.SquareGem:
            case LevelDataKeys.Types.RectangleGem:

                
                dotData.SetProperty(DotsObject.Property.Colors, color.ToObject<string[]>());
                dotData.SetProperty(DotsObject.Property.Directions, direction.ToObject<int[,]>());
                break;
        };
        return dotData;
    }
    
    
}
