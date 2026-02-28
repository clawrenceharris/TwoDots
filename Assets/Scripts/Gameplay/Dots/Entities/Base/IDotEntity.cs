using UnityEngine;

public interface IBoardEntity
{
    string ID { get; }
    Vector2Int GridPosition { get; set; }
}