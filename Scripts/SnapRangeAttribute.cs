using UnityEngine;

public class SnapRangeAttribute : PropertyAttribute
{
    public float Min;
    public float Max;
    public float Snap;
    
    public SnapRangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
        Snap = 0;
    }
    
    public SnapRangeAttribute(float min, float max, float snap)
    {
        Min = min;
        Max = max;
        Snap = snap;
    }
}

