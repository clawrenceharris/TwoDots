using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class DotFactory
{

    public static Dot CreateDot(DotsObject data)
    {
        var colors = data.GetProperty<string[]>(DotsObject.Property.Colors);
        var levelColors = LevelLoader.Level.colors;
        var directions = data.GetProperty<int[,]>(DotsObject.Property.Directions);
        var type = LevelLoader.FromJsonType<DotType>(data.Type);

        switch (type)
        {
            case DotType.Normal:
                {
                    var validColors = colors ?? levelColors;
                    var color = validColors[Random.Range(0, validColors.Length)];
                    var dot = new Dot(DotType.Normal, new Vector2Int(data.Col, data.Row));
                    var colorable = dot.AddModel(new ColorableModel(dot));
                    colorable.Color = LevelLoader.FromJsonColor(color);
                  
                    return dot;
                }

                case DotType.Blank:
                {
                    var dot = new Dot(DotType.Blank, new Vector2Int(data.Col, data.Row));
                    dot.AddModel(new BlankColorableModel(dot));
                    return dot;
                }
            default: throw new ArgumentException("Invalid dot type: " + type);
        }
    }

    public static IDotPresenter CreateDotPresenter(Dot dot, DotView view)
    {
       switch (dot.DotType)
       {
        case DotType.Normal:{
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            return presenter;
        
        }
        case DotType.Beetle:
        {
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            return presenter;
        }
        case DotType.Blank:
        {
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            return presenter;
        }
        default: return new DotPresenter(dot, view);
       }
    }
}