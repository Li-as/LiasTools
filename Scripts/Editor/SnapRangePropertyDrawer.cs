using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SnapRangeAttribute))]
public class SnapRangePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Float)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = attribute as SnapRangeAttribute;
            float value = EditorGUI.Slider(position, label, property.floatValue, attrib.Min, attrib.Max);
            
            if (attrib.Snap > 0)
            {
                float interval = attrib.Snap;
                value = Mathf.Round(value / interval) * interval;
                value = Mathf.Clamp(value, attrib.Min, attrib.Max);
            }
            
            property.floatValue = value;
            EditorGUI.EndProperty();
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = attribute as SnapRangeAttribute;
            int min = (int)attrib.Min;
            int max = (int)attrib.Max;
            int interval = (int)attrib.Snap;
            int value = EditorGUI.IntSlider(position, label, property.intValue, min, max);
            
            if (interval > 0)
            {
                value = (int)System.Math.Round(value / attrib.Snap, 0, System.MidpointRounding.AwayFromZero) * interval;
                value = Mathf.Clamp(value, min, max);
            }
            
            property.intValue = value;
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "[SnapRange] attribute can only be used with float/int types!");
        }
    }
}

