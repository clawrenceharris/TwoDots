

using System;
using System.Collections.Generic;
using UnityEngine;

public class BombPool : Pool
{
    private readonly Queue<BombPoolObject> _pool = new();

    
    IBoardPresenter _board;

    void Awake() {
        _board = FindFirstObjectByType<BoardPresenter>();
    }

    public override void Fill(int size) {
        Clear(_pool);
        for (int i = 0; i < size; i++)
        {
            var bombDotObject = new DotsObject
            {
                Type = LevelLoader.ToJsonDotType(DotType.Bomb),
                Col = 0,
                Row = 0,
            };
            var presenter = _board.CreateDotPresenter(bombDotObject);
            Debug.Log($"Creating bomb pool object for {presenter.View.name}");
            var bombPoolObj = new GameObject("BombPoolObject").AddComponent<BombPoolObject>();
            bombPoolObj.Presenter = presenter;
            bombPoolObj.transform.SetParent(transform);
            bombPoolObj.Presenter.View.transform.SetParent(bombPoolObj.transform);
            _pool.Enqueue(bombPoolObj);
            bombPoolObj.gameObject.SetActive(false);
        }
    }

    public override T Get<T>()
    {
        if (typeof(T) != typeof(BombPoolObject))
        {
            throw new ArgumentException($"Type {typeof(T)} is not a {typeof(BombPoolObject)}");
        }
        if (_pool.Count == 0)
        {
            Debug.LogWarning("Bomb pool is empty!");
            return null;
        }
        BombPoolObject bombPoolObject = _pool.Dequeue();
        bombPoolObject.gameObject.SetActive(true);
        return bombPoolObject as T;
    }

    public override void Return<T>(T item)
    {
        if (item is not BombPoolObject bombPoolObject)
        {
            throw new ArgumentException($"Item is not a {typeof(BombPoolObject)}");
        }
        _pool.Enqueue(bombPoolObject);
        bombPoolObject.gameObject.SetActive(false);
    }
}