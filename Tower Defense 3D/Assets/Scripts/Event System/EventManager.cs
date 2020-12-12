using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.ExecuteEvents;

public static class EventManager
{
    private static HashSet<GameObject> _listeners = new HashSet<GameObject>();

    public static void AddListener(GameObject listener)
    {
        _listeners.Add(listener);
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