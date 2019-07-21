using UnityEngine;
using BaseFramework;

public class MainCamera : MonoSingleton<MainCamera>
{

    private Camera _mainCamera;

    public override void OnSingletonInit()
    {
        base.OnSingletonInit();

        _mainCamera = GetComponent<Camera>();
    }

    public static Camera mainCamera => instance._mainCamera;
}
