using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.ExecuteEvents;

public static class EventManager
{
    private static List<GameObject> _listeners = new List<GameObject>();

    public static void AddListener(GameObject listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public static void RemoveListener(GameObject listener)
    {
        _listeners.Remove(listener);
    }

    public static void ExecuteEvent<T>(EventFunction<T> functor) where T : IEventSystemHandler
    {
        foreach (GameObject gameObject in _listeners.ToArray())
        {
            Execute(gameObject, null, functor);
        }
    }
}