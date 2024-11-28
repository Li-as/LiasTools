using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class LoggerSettings : EditorWindow
{
    [SerializeField] private bool _logsEnabled;

    private readonly IReadOnlyList<NamedBuildTarget> _namedBuildTargets = new[] { NamedBuildTarget.Standalone, NamedBuildTarget.Server, NamedBuildTarget.Android };
    private readonly Dictionary<NamedBuildTarget, string> _defineSymbols = new Dictionary<NamedBuildTarget, string>();

    private const string _logsDirective = "LOG_ENABLED";

    private SerializedObject _loggerSettingsObject;
    private SerializedProperty _logsEnabledProperty;

    private void OnEnable()
    {
        if (_loggerSettingsObject == null)
            _loggerSettingsObject = new SerializedObject(this);
        if (_logsEnabledProperty == null)
            _logsEnabledProperty = _loggerSettingsObject.FindProperty("_logsEnabled");

        _logsEnabledProperty.boolValue = IsLogDefineActive();
        _loggerSettingsObject.ApplyModifiedProperties();
    }

    [MenuItem("Tools/Logger settings")]
    public static void ShowWindow()
    {
        GetWindow<LoggerSettings>();
    }

    private void OnGUI()
    {
        bool valueBeforeChange = _logsEnabled;
        EditorGUILayout.PropertyField(_logsEnabledProperty, true);
        _loggerSettingsObject.ApplyModifiedProperties();

        if (_logsEnabled != valueBeforeChange)
        {
            SaveStartSymbols();

            Action<string> requiredAction = _logsEnabled ? AddDirective : RemoveDirective;
            requiredAction?.Invoke(_logsDirective);

            SaveChangedSymbols();
            ForceRecompile();
        }
    }

    private bool IsLogDefineActive()
    {
        SaveStartSymbols();

        foreach (NamedBuildTarget target in _namedBuildTargets)
        {
            if (_defineSymbols[target].Contains(_logsDirective) == false)
                return false;
        }

        return true;
    }

    private void ForceRecompile()
    {
        DirectoryInfo dataPathInfo = new DirectoryInfo(Application.dataPath);

        foreach (FileInfo file in dataPathInfo.GetFiles("Logger.cs", SearchOption.AllDirectories))
        {
            string relativePath = "Assets/" + Path.GetRelativePath(dataPathInfo.FullName, file.FullName);
            AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
            break;
        }
    }

    private void SaveStartSymbols()
    {
        _defineSymbols.Clear();

        foreach (NamedBuildTarget target in _namedBuildTargets)
        {
            if (_defineSymbols.ContainsKey(target))
            {
                Debug.Log($"StartDefineSymbols already contains {target} target");
                continue;
            }

            string curSymbols = PlayerSettings.GetScriptingDefineSymbols(target);
            _defineSymbols.Add(target, curSymbols);
        }
    }

    private void SaveChangedSymbols()
    {
        foreach (NamedBuildTarget target in _namedBuildTargets)
            PlayerSettings.SetScriptingDefineSymbols(target, _defineSymbols[target]);
    }

    private void AddDirective(string directive)
    {
        if (string.IsNullOrEmpty(directive))
            throw new ArgumentNullException(nameof(directive), "Cannot add empty directive");

        foreach (NamedBuildTarget target in _namedBuildTargets)
        {
            if (_defineSymbols[target].Contains(directive) == false)
                _defineSymbols[target] += $";{directive}";
        }
    }

    private void RemoveDirective(string directive)
    {
        if (string.IsNullOrEmpty(directive))
            throw new ArgumentNullException(nameof(directive), "Cannot remove empty directive");

        foreach (NamedBuildTarget target in _namedBuildTargets)
        {
            if (_defineSymbols[target].Contains(directive) == false)
                continue;

            string directiveWithSeparator = ";" + directive;
            string curSymbols = _defineSymbols[target];
            bool withSeparator = curSymbols.Contains(directiveWithSeparator);
            string resultStrToReplace = withSeparator ? directiveWithSeparator : directive;
            _defineSymbols[target] = curSymbols.Replace(resultStrToReplace, null);
        }
    }
}
