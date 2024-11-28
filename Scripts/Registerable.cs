using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registerable : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _behaviourToRegistry;
    
    private Type _behaviourType;

    private void Start()
    {
        _behaviourType = _behaviourToRegistry.GetType();
        Register();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    private void Register()
    {
        ObjectsRegistry.Register(_behaviourToRegistry, _behaviourType);
    }

    private void Unregister()
    {
        if (_behaviourType != null)
            ObjectsRegistry.Unregister(_behaviourToRegistry, _behaviourType);
    }
}
