using UnityEngine;

public class OnChangedCallbackAttribute : PropertyAttribute
{
    public string MethodName;

    public OnChangedCallbackAttribute(string methodName) => MethodName = methodName;
}

