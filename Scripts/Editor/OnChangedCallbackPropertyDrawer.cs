using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OnChangedCallbackAttribute), true)]
public class OnChangedCallbackPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label, true);
        if (!EditorGUI.EndChangeCheck()) return;
        
        var targetObject = property.serializedObject.targetObject;
        
        var callAttribute = attribute as OnChangedCallbackAttribute;
        var methodName = callAttribute?.MethodName;

        var classType = targetObject.GetType();
        var methodInfo = classType.GetMethods().FirstOrDefault(info => info.Name == methodName);

        // Update the serialized field
        property.serializedObject.ApplyModifiedProperties();
        
        // If we found a public function with the given name that takes no parameters, invoke it
        if (methodInfo != null && !methodInfo.GetParameters().Any())
        {
            methodInfo.Invoke(targetObject, null);
        }
        else
        {
            Debug.LogError($"OnChangedCallback: There is no public function without arguments named {methodName} in class {classType.Name}!");
        }
    }
}

