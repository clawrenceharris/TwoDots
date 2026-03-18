using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceProvider : MonoBehaviour
{
    public static ServiceProvider Instance { get; private set; }
    private readonly Dictionary<Type, object> _services = new();
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
    public T GetService<T>() where T : class
    {
        if (TryGetService(out T service))
        {
            return service;
        }
        else
        {
            Debug.LogError($"Service {typeof(T)} not found");
            return default;
        }
    }
    public bool TryGetService<T>(out T service) where T : class
    {
        if(_services.TryGetValue(typeof(T), out var tService))
        {
            service = tService as T;
            return true;
        }
       
        service = default;
        return false;
        
    }
    public void RegisterService<T>(T service)
    {
        _services.Add(typeof(T), service);
    }

    public void RegisterServices()
    {

        var colorScheme = FindFirstObjectByType<ColorSchemeService>();
        var board = FindFirstObjectByType<BoardService>();
        var connection = FindFirstObjectByType<ConnectionService>();
        var pool = FindFirstObjectByType<PoolService>();
        RegisterService(colorScheme);
        RegisterService(board);
        RegisterService(connection);
        RegisterService(pool);
       
    }
}