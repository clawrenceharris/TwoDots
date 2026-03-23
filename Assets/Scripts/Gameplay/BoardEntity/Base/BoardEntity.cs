using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardEntity : IBoardEntity
{
    public string ID { get; private set; }
    private Vector2Int _gridPosition;
    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set => _gridPosition = value;
    }
    private readonly Dictionary<Type, IModel> _models = new();
    public BoardEntity(Vector2Int gridPosition)
    {
        ID = Guid.NewGuid().ToString();
        GridPosition = gridPosition;
    }
    public abstract T GetEntityType<T>() where T : Enum, IConvertible;
    public T AddModel<T>(T component) where T : class, IModel
    {
        _models.Add(typeof(T), component);
        return component;
    }
    public T GetModel<T>() where T : class, IModel
    {
        if (TryGetModel(out T model))
        {
            return model;
        }
        Debug.LogError($"Model {typeof(T)} not found");
        return null;
    }
    public bool TryGetModel<T>(out T model) where T : class,IModel
    {

        // First, try to find an exact type match
        if (_models.TryGetValue(typeof(T), out IModel tModel))
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
    public void RemoveModel<T>() where T : class, IModel
    {
        _models.Remove(typeof(T));
    }

}