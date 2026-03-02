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
        RegisterPools();

    }
    private void RegisterPools()
    {
        LinePool linePool = FindFirstObjectByType<LinePool>();
        linePool.transform.SetParent(transform);
        _pools.TryAdd(typeof(LinePool), linePool);
    }

    public void FillPool<T>(int size) where T : Pool
    {
        
        var pool = _pools[typeof(T)];
        pool.Fill(size);
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