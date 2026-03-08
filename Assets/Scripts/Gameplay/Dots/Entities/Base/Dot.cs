using System;
using System.Collections.Generic;
using UnityEngine;

public class Dot : IBoardEntity
{
    private readonly Dictionary<Type, IDotModel> _models = new();

    private Vector2Int _gridPosition;
    private readonly DotType _dotType;
    public DotType DotType => _dotType;
    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set => _gridPosition = value;
    }
    public string ID { get; private set; }

    public Dot(DotType type, Vector2Int position)
    {
        _dotType = type;
        _gridPosition = position;
        ID = Guid.NewGuid().ToString();
    }

   

    public T AddModel<T>(T component) where T : class,IDotModel
    {
        _models.Add(typeof(T), component);
        return component;
    }
    public T GetModel<T>() where T : class, IDotModel
    {
        if (TryGetModel(out T model))
        {
            return model;
        }
        Debug.LogError($"Model {typeof(T)} not found");
        return null;
    }
    public bool TryGetModel<T>(out T model) where T : class,IDotModel
    {

        // First, try to find an exact type match
        if (_models.TryGetValue(typeof(T), out IDotModel tModel))
        // If not found, search the components for a value that is compatible (handles subtypes/interfaces)
        {
            model = tModel as T;
            return true;
        }
        else
        {
            foreach (var kvp in _models)
            {
                if (kvp.Value is T tModelValue)
                {
                    model = tModelValue;
                    return true;
                }
            }
        }
        model = null;
        return false;
    }
    public void RemoveModel<T>() where T : class, IDotModel
    {
        _models.Remove(typeof(T));
    }
}