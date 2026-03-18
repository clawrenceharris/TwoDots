using UnityEngine;

/// <summary>
/// Interface for a board entity.
/// </summary>
public interface IBoardEntity
{
    string ID { get; }
    Vector2Int GridPosition { get; set; }
    T GetModel<T>() where T : class, IModel;
    bool TryGetModel<T>(out T model) where T : class, IModel;
    void RemoveModel<T>() where T : class, IModel;
    T AddModel<T>(T component) where T : class, IModel;
}