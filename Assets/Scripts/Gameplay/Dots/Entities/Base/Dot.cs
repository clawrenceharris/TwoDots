using System;
using System.Collections.Generic;
using UnityEngine;

public class Dot : IBoardEntity
{
    private readonly Dictionary<Type, IModel> _components = new();

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

   

    public T AddModel<T>(T component) where T : class,IModel
    {
        _components.Add(typeof(T), component);
        return component;
    }
    public T GetModel<T>() where T : class, IModel
    {
        // First, try to find an exact type match
        if (_components.TryGetValue(typeof(T), out IModel component))
        // If not found, search the components for a value that is compatible (handles subtypes/interfaces)
        {
            return component as T;
        }
        else
        {
            foreach (var kvp in _components)
            {
                if (kvp.Value is T tComponent)
                {
                    return tComponent;
                }
            }
        }
        throw new ArgumentException($"Component {typeof(T)} not found for dot");
    }
    public bool TryGetModel<T>(out T component) where T : class,IModel
    {
        component = GetModel<T>();
        return component != null;
    }
    public void RemoveModel<T>() where T : class, IModel
    {
        _components.Remove(typeof(T));
    }
}