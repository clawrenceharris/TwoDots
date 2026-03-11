using System;
using System.Collections.Generic;
using UnityEngine;
public class PoolService : MonoBehaviour
{
    public static PoolService Instance { get; private set; }
    private readonly Dictionary<Type, Pool> _pools = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        

    }
    public void RegisterPools()
    {
        LinePool linePool = FindFirstObjectByType<LinePool>();
        BombPool bombPool = FindFirstObjectByType<BombPool>();
        linePool.transform.SetParent(transform);
        _pools.TryAdd(typeof(LinePool), linePool);
        _pools.TryAdd(typeof(BombPool), bombPool);
    }

    public void FillPool<T>(int? size = null) where T : Pool
    {
        
        var pool = _pools[typeof(T)];
        pool.Fill(size ?? pool.Size);
    }
    public U GetFromPool<T, U>() where T : Pool where U : MonoBehaviour
    {
       
        if(!_pools.TryGetValue(typeof(T), out var pool))
        {
            Debug.LogError($"Pool {typeof(T)} not found");
            return null;
        }
        return pool.Get<U>();
    }
    public void ReturnToPool<T>(MonoBehaviour item) where T : Pool
    {
        if (!_pools.TryGetValue(typeof(T), out var pool))
        {
            Debug.LogError($"Pool {typeof(T)} not found");
            return;
        }
        pool.Return(item);
    }

}