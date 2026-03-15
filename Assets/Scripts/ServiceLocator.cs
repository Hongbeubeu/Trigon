using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        Debug.LogError($"[ServiceLocator] Service {typeof(T).Name} not registered.");
        return default;
    }

    public static bool TryGet<T>(out T service)
    {
        if (_services.TryGetValue(typeof(T), out var obj))
        {
            service = (T)obj;
            return true;
        }

        service = default;
        return false;
    }

    public static void Unregister<T>()
    {
        _services.Remove(typeof(T));
    }

    public static void Clear()
    {
        _services.Clear();
    }
}
