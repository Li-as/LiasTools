using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesInitializer
{
#if UNITY_EDITOR || !UNITY_SERVER
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
    private static void Init()
    {
        IEnumerable<IInitable> initables = Resources.LoadAll<ScriptableObject>("").OfType<IInitable>();

        foreach (IInitable initable in initables)
            initable.Init();

        Logger.Log($"[{nameof(ResourcesInitializer)}] Initialized");
    }
}

