using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[Overlay(typeof(SceneView), nameof(MirrorTool), _displayName)]
[Icon("Assets/Textures/8pixel_circle.png")]
public class MirrorTool : ToolbarOverlay
{
    private const string _displayName = nameof(MirrorTool);
    private const string _logStart = "[" + nameof(MirrorTool) + "]";

    private Vector3Field _originField;
    [SerializeField] private Vector3 _origin;

    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new VisualElement();

        root.style.flexGrow = 0;
        root.style.flexDirection = FlexDirection.Column;
        root.style.width = new StyleLength(new Length(150, LengthUnit.Pixel));
        
        root.Add(new Button(() => Mirror(true, false, false)) { text = "Along All" });
        root.Add(GetNewFillRow());
        root.Add(new Button(() => Mirror(false, true, false)) { text = "Along X" });
        root.Add(GetNewFillRow());
        root.Add(new Button(() => Mirror(false, false, true)) { text = "Along Z" });
        root.Add(GetNewFillRow());
        
        Label originFieldLabel = new Label("Origin");
        _originField = new Vector3Field();
        _originField.value = _origin;
        _originField.RegisterValueChangedCallback(OnOriginChanged);
        root.Add(originFieldLabel);
        root.Add(_originField);
        
        return root;
    }

    private VisualElement GetNewFillRow()
    {
        VisualElement fillRow = new VisualElement();
        fillRow.style.height = new StyleLength(new Length(5, LengthUnit.Pixel));
        return fillRow;
    }

    public override void OnWillBeDestroyed()
    {
        if (_originField != null)
            _originField.UnregisterValueChangedCallback(OnOriginChanged);
        
        base.OnWillBeDestroyed();
    }

    private void OnOriginChanged(ChangeEvent<Vector3> value)
    {
        _origin = value.newValue;
        //Debug.Log($"{_logStart} Changed origin to {_origin}");
    }
    
    private void Mirror(bool alongAll, bool alongX, bool alongZ)
    {
        //Debug.Log($"{_logStart} In Mirror; Selected objects: {Selection.gameObjects.Length};");
        int selectedLength = Selection.gameObjects.Length;

        if (selectedLength == 0)
            return;

        for (int i = 0; i < selectedLength; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            Vector3 direction = _origin - obj.transform.position;
            Vector3 targetPos = obj.transform.position + direction + direction;
            targetPos.y = obj.transform.position.y;
            if (alongAll == false)
            {
                if (alongX)
                    targetPos.z = obj.transform.position.z;
                if (alongZ)
                    targetPos.x = obj.transform.position.x;
            }

            GameObject mirroredObj;
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                GameObject origObj = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                mirroredObj = (GameObject)PrefabUtility.InstantiatePrefab(origObj);
            }
            else
            {
                mirroredObj = Object.Instantiate(obj);
            }
            
            mirroredObj.transform.localScale = obj.transform.localScale;
            mirroredObj.transform.SetParent(obj.transform.parent);
            mirroredObj.transform.position = targetPos;
            
            MirrorRotation(obj.transform, mirroredObj.transform, alongX, alongAll);
        }
    }
    
    private void MirrorRotation(Transform source, Transform target, bool alongX, bool alongAll)
    {
        if (alongX)
        {
            target.localRotation = FlipRotation(source.localRotation);
            return;
        }
        
        Vector3 reflectNormal = _origin + Vector3.forward;
        
        if (alongAll)
        {
            Quaternion targetRot = ReflectRotation(reflectNormal, source.localRotation);
            targetRot = FlipRotation(targetRot);
            target.localRotation = targetRot;
            return;
        }
        
        target.localRotation = ReflectRotation(reflectNormal, source.localRotation);
    }
    
    private Quaternion FlipRotation(Quaternion source)
    {
        return new Quaternion(source.x,
                              source.y * -1f,
                              source.z * -1f,
                              source.w);
    }
    
    private Quaternion ReflectRotation(Vector3 normal, Quaternion source)
    {
        return Quaternion.LookRotation(Vector3.Reflect(source * Vector3.forward, normal), Vector3.Reflect(source * Vector3.up, normal));
    }
}

