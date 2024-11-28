using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public static class Utils
{
    private static readonly Dictionary<float, WaitForSeconds> _timeWaitPairs = new Dictionary<float, WaitForSeconds>();

    public static Action EmptyAction => () => { };

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (_timeWaitPairs.ContainsKey(seconds))
            return _timeWaitPairs[seconds];

        _timeWaitPairs.Add(seconds, new WaitForSeconds(seconds));
        return _timeWaitPairs[seconds];
    }

    public static bool IsSubclassOrBase(Type type, Type baseType)
    {
        return type.IsSubclassOf(baseType) || type.ToString() == baseType.ToString();
    }

    public static Coroutine WaitAndPerform(float seconds, Action action, bool cyclic = false)
    {
        if (cyclic == false)
            return _utilsMB.StartCoroutine(WaitAndPerformRoutine(seconds, action));
        else
            return _utilsMB.StartCoroutine(WaitPerformCycleRoutine(seconds, action));
    }

    public static Coroutine WaitAndPerformCyclic(float seconds, int cycles, Action action)
    {
        return _utilsMB.StartCoroutine(WaitPerformCycleRoutine(seconds, action, cycles));
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        _utilsMB.StopCoroutine(coroutine);
    }

    public static Coroutine W1(Action action)
    {
        return WaitAndPerform(1, action);
    }
    
    public static Coroutine W2(Action action)
    {
        return WaitAndPerform(2, action);
    }
    
    public static Coroutine W1Frame(Action action)
    {
        return WaitAndPerform(0, action);
    }

    public static Coroutine W2Frames(Action action)
    {
        return _utilsMB.StartCoroutine(WaitFramesAndPerformRoutine(2, action));
    }

    private static IEnumerator WaitAndPerformRoutine(float seconds, Action action)
    {
        if (seconds > 0)
            yield return WaitForSeconds(seconds);
        else
            yield return null;

        action?.Invoke();
    }

    private static IEnumerator WaitPerformCycleRoutine(float delay, Action action, int cycles = 0)
    {
        for (;;)
        {
            if (delay > 0)
                yield return WaitForSeconds(delay);
            else
                yield return null;
            
            action?.Invoke();

            if (cycles == 0)
                continue;

            cycles--;
            if (cycles <= 0)
                break;
        }
    }

    private static IEnumerator WaitFramesAndPerformRoutine(int frames, Action action)
    {
        for (int i = frames; i > 0; i--)
            yield return null;
        
        action?.Invoke();
    }

    public static void WaitUntilSceneIsLoaded(string sceneName, Action action)
    {
        _utilsMB.StartCoroutine(WaitUntilSceneIsLoadedRoutine(sceneName, action));
    }

    public static IEnumerator WaitUntilSceneIsLoadedRoutine(string sceneName, Action action = null)
    {
        Scene sceneToMoveIn;
        do
        {
            yield return null;
            sceneToMoveIn = SceneManager.GetSceneByName(sceneName);
        } while (sceneToMoveIn.IsValid() == false || sceneToMoveIn.isLoaded == false);

        action?.Invoke();
    }

    public static void WaitUntilSceneIsActive(string sceneName, Action action)
    {
        _utilsMB.StartCoroutine(WaitUntilSceneIsActiveRoutine(sceneName, action));
    }

    public static IEnumerator WaitUntilSceneIsActiveRoutine(string sceneName, Action action = null)
    {
        do
        {
            yield return null;
        } while (SceneManager.GetActiveScene().name != sceneName);

        action?.Invoke();
    }

    public static void WaitUntil(Func<bool> predicate, Action action)
    {
        _utilsMB.StartCoroutine(WaitUntilRoutine(predicate, action));
    }

    public static IEnumerator WaitUntilRoutine(Func<bool> predicate, Action action = null)
    {
        yield return new WaitUntil(predicate);
        action?.Invoke();
    }


    public class UtilsMonoBehaviour : MonoBehaviour { }

    private static UtilsMonoBehaviour _utilsMB;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitMB()
    {
        if (_utilsMB == null)
        {
            GameObject gameObject = new GameObject("StaticUtils");
            _utilsMB = gameObject.AddComponent<UtilsMonoBehaviour>();
            MonoBehaviour.DontDestroyOnLoad(gameObject);
            Logger.Log("[Utils] Utils MonoBehaviour is initialized");
        }
    }

    public static Coroutine StartRoutine(IEnumerator routine) => _utilsMB.StartCoroutine(routine);

    public static AnimationCurve ScaleCurve(AnimationCurve curve, float maxX, float maxY)
    {
        AnimationCurve scaledCurve = new AnimationCurve();

        for (int i = 0; i < curve.keys.Length; i++)
        {
            Keyframe keyframe = curve.keys[i];
            keyframe.value = curve.keys[i].value * maxY;
            keyframe.time = curve.keys[i].time * maxX;
            keyframe.inTangent = curve.keys[i].inTangent * maxY / maxX;
            keyframe.outTangent = curve.keys[i].outTangent * maxY / maxX;

            scaledCurve.AddKey(keyframe);
        }

        return scaledCurve;
    }

    public static void Validate(this MonoBehaviour monoBehaviour, UnityEngine.Object target, string fieldName)
    {
        if (target == null)
        {
            string errorMessage = $"{fieldName} cannot be null!";

#if UNITY_EDITOR
            Debug.LogError(errorMessage, monoBehaviour);
            return;
#endif

            throw new ArgumentNullException(fieldName, errorMessage);
        }
    }

    public static void Validate(this MonoBehaviour monoBehaviour, string target, string fieldName)
    {
        if (string.IsNullOrEmpty(target))
        {
            string errorMessage = $"{fieldName} cannot be null or empty!";

#if UNITY_EDITOR
            Debug.LogError(errorMessage, monoBehaviour);
            return;
#endif

            throw new ArgumentNullException(fieldName, $"{fieldName} cannot be null or empty!");
        }
    }

    public static float DistanceTo(this GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    public static float DistanceTo(this MonoBehaviour from, MonoBehaviour to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    public static float DistanceTo(this Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position);
    }

    public static bool HasLayer(this LayerMask mask, int layer)
    {
        return ((1 << layer) & mask) != 0;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void UnloadAllScenesExcept(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneName)
                SceneManager.UnloadSceneAsync(scene);
        }
    }

    public static bool IsObjectsMovedToScene(IEnumerable<GameObject> gameObjects, Scene scene)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go == null)
                continue;
            if (go.scene.name != scene.name)
                return false;
        }

        return true;
    }

    public static void LoadScene(string scene, bool clearLightData)
    {
        if (clearLightData)
            LightmapSettings.lightProbes = null;

        SceneManager.LoadScene(scene);
    }

    public static void LoadScene(string scene, LoadSceneMode mode, bool clearLightData)
    {
        if (clearLightData)
            LightmapSettings.lightProbes = null;

        SceneManager.LoadScene(scene, mode);
    }

    public static AsyncOperation LoadSceneAsync(string scene, LoadSceneMode mode, bool clearLightData)
    {
        if (clearLightData)
            LightmapSettings.lightProbes = null;

        return SceneManager.LoadSceneAsync(scene, mode);
    }

    public static void Ping(string address, Action<int> callback)
    {
        if (_utilsMB != null)
            StartRoutine(WaitPingCoroutine(address, callback));
    }

    private static IEnumerator WaitPingCoroutine(string address, Action<int> callback)
    {
        var ping = new Ping(address);

        while (!ping.isDone)
        {
            yield return null;
        }

        bool success = ping.isDone;

        if (success)
            callback?.Invoke(ping.time);
        else
            Logger.Log($"Ping function was not completed successfully!");
    }

    public static float TotalDuration(this ParticleSystem ps)
    {
        float maxDuration = ps.main.duration;
        ParticleSystem[] children = ps.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < children.Length; i++)
        {
            float curDuration = children[i].main.duration;
            if (curDuration > maxDuration)
                maxDuration = curDuration;
        }

        return maxDuration;
    }
    
    public static void DebugAllObjects()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] allObjects = activeScene.GetRootGameObjects();
        Debug.Log($"[{nameof(Utils)}] DEBUGING ALL OBJECTS IN THE {activeScene.name} SCENE");
        
        for (int i = 0; i < allObjects.Length; i++)
            Debug.Log($"[{nameof(Utils)}] [{i+1}]. {allObjects[i].name}");
    }

    public static void DebugAllComponents(this GameObject source)
    {
        Component[] components = source.GetComponents(typeof(Component));
        Logger.Log($"=========================================================");
        for (int i = 0; i < components.Length; i++)
            Logger.Log($"[{source.name}] {i+1}. {components[i]}");
        Logger.Log($"=========================================================");
    }

    public static void RemoveUnorderedAt<T>(this List<T> list, int index)
    {
        list[index] = list[^1];
        list.RemoveAt(list.Count - 1);
    }

    public static Vector3 ToVector3(this string target)
    {
        if (string.IsNullOrEmpty(target))
            return Vector3.zero;
        
		if (target.StartsWith ("(") && target.EndsWith (")"))
			target = target[1..^1];

		string[] sArray = target.Split(',');

		Vector3 result = new Vector3(
			float.Parse(sArray[0]),
			float.Parse(sArray[1]),
			float.Parse(sArray[2]));

		return result;
    }

    public static Quaternion ToQuaternion(this string target)
    {
        if (string.IsNullOrEmpty(target))
            return Quaternion.identity;
        
		if (target.StartsWith ("(") && target.EndsWith (")"))
			target = target[1..^1];

		string[] sArray = target.Split(',');

		Quaternion result = new Quaternion(
			float.Parse(sArray[0]),
			float.Parse(sArray[1]),
			float.Parse(sArray[2]),
            float.Parse(sArray[3]));

		return result;
    }
}
