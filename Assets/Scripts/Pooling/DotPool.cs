using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DotPoolObject : MonoBehaviour
{
    public DotType DotType { get; set; }
    public IDotPresenter Presenter { get; set; }
}
public class DotPool : Pool
{
    private readonly Queue<DotPoolObject> _dotPool = new();
    private IBoardPresenter _board;

    private void Awake()
    {
        _board = FindFirstObjectByType<BoardPresenter>();
    }

    public override void Fill(int size)
    {
        // var types = LevelLoader.Level.dotsToSpawn.Select(d => LevelLoader.FromJsonType<DotType>(d.Type));
        // foreach (var type in types)
        // {
        //     for (int i = 0; i < size; i++)
        //     {
        //         var dot = _board.CreateDotPresenter(new DotsObject()
        //         {
        //             Type = type.ToString(),
        //             Col = 0,
        //             Row = 0,
        //         });
        //         DotPoolObject dotPoolObject = new GameObject().AddComponent<DotPoolObject>();
        //         dotPoolObject.DotType = type;
        //         dotPoolObject.Presenter = dot;
        //         _dotPool.Enqueue(dotPoolObject);
        //     }
        // }
    }



    public override T Get<T>()
    {
        if (typeof(T) != typeof(DotPoolObject))
        {
            throw new ArgumentException($"Type {typeof(T)} is not a {typeof(IDotPresenter)}");
        }

        if (_dotPool.Count == 0)
        {
            Debug.LogWarning("Dot pool is empty!");
            return null;
        }
        DotPoolObject dotPoolObject = _dotPool.Dequeue();
        dotPoolObject.gameObject.SetActive(true);
        return dotPoolObject as T;
    }

    public override void Return<T>(T dotPoolObject)
    {
        if (typeof(T) != typeof(IDotPresenter))
        {
            throw new ArgumentException($"Type {typeof(T)} is not a {typeof(IDotPresenter)}");
        }
        _dotPool.Enqueue(dotPoolObject as DotPoolObject);
        dotPoolObject.gameObject.SetActive(false);
    }

    public override void Clear<T>(Queue<T> pool)
    {
        base.Clear(pool);
    }
}