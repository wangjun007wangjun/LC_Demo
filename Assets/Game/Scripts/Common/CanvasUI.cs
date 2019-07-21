using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class CanvasUI : MonoBehaviour
{
    public string showName = "MainUI";

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
#if UNITY_EDITOR
        canvas.worldCamera = Camera.main;
#else
        canvas.worldCamera = MainCamera.mainCamera;
#endif
    }

    public void Reset()
    {
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(ScreenSetting.instance.idealScreenSizeMin,
                                                       ScreenSetting.instance.idealScreenSizeMax);
        canvasScaler.matchWidthOrHeight = ScreenSetting.instance.defaultMatch;

        name = showName;
    }
}
