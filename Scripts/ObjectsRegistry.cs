using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectsRegistry
{
    private static readonly Dictionary<Type, List<MonoBehaviour>> _objects = new Dictionary<Type, List<MonoBehaviour>>();
    private const string _debugStart = "[" + nameof(ObjectsRegistry) + "] ";

    public static bool TryGetObjectsOfType(Type type, out IReadOnlyList<MonoBehaviour> behaviours)
    {
        behaviours = null;

        if (_objects.ContainsKey(type) == false)
        {
            Logger.LogWarning(_debugStart + $"There is no objects with type <{type}>! Returning a null instead.");
            return false;
        }


        if (_objects[type].Count == 0)
        {
            Logger.LogWarning(_debugStart + $"There is no objects with type <{type}>! Returning a null instead.");
            return false;
        }

        behaviours = _objects[type].AsReadOnly();
        return true;
    }

    public static bool TryGetObjectsOfInterfaceType(Type interfaceType, out IReadOnlyList<MonoBehaviour> behaviours)
    {
        behaviours = null;
        List<MonoBehaviour> foundBehaviours = new List<MonoBehaviour>();

        foreach (Type objectsType in _objects.Keys)
        {
            if (interfaceType.IsAssignableFrom(objectsType) == false)
                continue;
            foundBehaviours.AddRange(_objects[objectsType]);
        }

        if (foundBehaviours.Count == 0)
        {
            Logger.LogWarning(_debugStart + $"There is no objects implementing <{interfaceType}>! Returning a null instead.");
            return false;
        }

        behaviours = foundBehaviours.AsReadOnly();
        return true;
    }

    public static bool TryGetObjectOfType(Type type, out MonoBehaviour behaviour)
    {
        behaviour = null;

        if (_objects.ContainsKey(type) == false)
        {
            Logger.LogWarning(_debugStart + $"There is no objects with type <{type}>! Returning a null instead.");
            return false;
        }

        if (_objects[type].Count == 0)
        {
            Logger.LogWarning(_debugStart + $"There is no objects with type <{type}>! Returning a null instead.");
            return false;
        }

        behaviour = _objects[type][0];
        return true;
    }

    public static void Register(MonoBehaviour behaviour, Type type)
    {
        if (behaviour == null)
        {
            Logger.LogError(_debugStart + $"Behaviour you are trying to register is null! Type: <{type}>");
            return;
        }

        if (_objects.ContainsKey(type))
        {
            if (_objects[type].Contains(behaviour))
            {
                Logger.LogError(_debugStart + $"Can't register! This behaviour is already registered. Type: <{type}>");
                return;
            }

            _objects[type].Add(behaviour);
            Logger.Log(_debugStart + $"Behaviour <{type}> on object {behaviour.name} is succesfully registered");
            return;
        }

        _objects.Add(type, new List<MonoBehaviour>());
        _objects[type].Add(behaviour);
        Logger.Log(_debugStart + $"Behaviour <{type}> on object {behaviour.name} is succesfully registered");
    }

    public static void Unregister(MonoBehaviour behaviour, Type type)
    {
        if (behaviour == null)
        {
            Logger.LogError(_debugStart + $"Behaviour you are trying to unregister is null! Type: <{type}>");
            return;
        }

        if (_objects.ContainsKey(type) == false)
        {
            Logger.LogError(_debugStart + $"Can't unregister! There is no objects registered with this type. Type: <{type}>");
            return;
        }

        if (_objects[type].Contains(behaviour) == false)
        {
            Logger.LogError(_debugStart + $"Can't unregister! This behaviour is not registered. Type: <{type}>");
            return;
        }

        _objects[type].Remove(behaviour);
        Logger.Log(_debugStart + $"Behaviour <{type}> on object {behaviour.name} is succesfully unregistered");
    }

    public static bool ContainsType(Type type) => _objects.ContainsKey(type);
}
