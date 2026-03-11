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
                    var colorable = dot.AddModel(new Colorable(dot));
                    dot.AddModel(new Hittable(dot, new Clearable(dot), hitMax: 1, conditions: new List<HitConditionType> { HitConditionType.Connection}, hitCount: 0));
                    colorable.Color = LevelLoader.FromJsonColor(color);
                  
                    return dot;
                }

                case DotType.Blank:
                {
                    var dot = new Dot(DotType.Blank, new Vector2Int(data.Col, data.Row));
                    dot.AddModel(new BlankColorableDot(dot));
                    dot.AddModel(new Hittable(dot, new Clearable(dot),hitMax: 1, conditions: new List<HitConditionType> { HitConditionType.Connection }, hitCount: 0));
                    return dot;
                }
                case DotType.Bomb:
                {
                    var dot = new Dot(DotType.Bomb, new Vector2Int(data.Col, data.Row));
                    dot.AddModel(new Clearable(dot, shouldClear: () => true));
                    return dot;
                }
                
            default: return new Dot(type, new Vector2Int(data.Col, data.Row));
        }
    }

    public static DotPresenter CreateDotPresenter(Dot dot, DotView view, IBoardPresenter board)
    {
       switch (dot.DotType)
       {
        case DotType.Normal:{
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            presenter.AddPresenter(new HittablePresenter(dot, view));
            presenter.AddPresenter(new ClearablePresenter(dot, view));
            return presenter;
        }
        case DotType.Beetle:
        {
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            presenter.AddPresenter(new HittablePresenter(dot, view, new ClearablePresenter(dot, view)));
            return presenter;
        }
        case DotType.Blank:
        {
            var presenter = new DotPresenter(dot, view);
            presenter.AddPresenter(new ConnectableDotPresenter(dot, view));
            presenter.AddPresenter(new HittablePresenter(dot, view));
            presenter.AddPresenter(new ClearablePresenter(dot, view));
            return presenter;
        }
        case DotType.Bomb:
        {
            if (view is not BombView bombView)
                    {
                Debug.LogError($"[DotFactory] View is not a BombView {view.name}");
                return null;
            }
            var presenter = new DotPresenter(dot, bombView);
            presenter.AddPresenter(new ClearablePresenter(dot, view));
            presenter.AddPresenter(new BombDotPresenter(dot, bombView));
            return presenter;   
        }
        default: return new DotPresenter(dot, view);
       }
    }
}