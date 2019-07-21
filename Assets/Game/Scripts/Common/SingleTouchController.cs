using UnityEngine;
using UnityEngine.EventSystems;
using BaseFramework;
using System;

public class SingleTouchController : MonoBehaviour
{

    public enum TouchStatus
    {
        Idle, PointerDown, Drag, PointerUp, Disable
    }

    public GameObject limitObj;
    public TouchStatus status = TouchStatus.Idle;

    public Action<Vector3> onPointerDown;
    public Action<Vector3> onDrag;
    public Action<Vector3> onPointerUp;

    public void Register(Action<Vector3> onPointerDown,
                         Action<Vector3> onDrag,
                         Action<Vector3> onPointerUp)
    {
        this.onPointerDown = onPointerDown;
        this.onDrag = onDrag;
        this.onPointerUp = onPointerUp;
    }

    public void UnRegister()
    {
        this.onPointerDown = null;
        this.onDrag = null;
        this.onPointerUp = null;
    }

    private void Update()
    {
        if (status == TouchStatus.Disable) return;
        
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (limitObj)
            {
                if (!EventSystem.current.IsPointerOverUIObject(Input.mousePosition, limitObj)) return;
            }
            else
            {
                if (EventSystem.current.IsPointerOverUIObject(Input.mousePosition)) return;
            }

            PointerDown(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Drag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            PointerUp(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (limitObj)
            {
                if (!EventSystem.current.IsPointerOverUIObject(Input.GetTouch(0).position, limitObj)) return;
            }
            else
            {
                if (EventSystem.current.IsPointerOverUIObject(Input.GetTouch(0).position)) return;
            }

            PointerDown(Input.GetTouch(0).position);
        }
        else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {

            Drag(Input.GetTouch(0).position);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            PointerUp(Input.GetTouch(0).position);
        }
#endif
    }

    private void PointerDown(Vector3 pointPosition)
    {
        if (status != TouchStatus.Idle) return;
        status = TouchStatus.PointerDown;
        onPointerDown?.Invoke(pointPosition);
    }

    private void Drag(Vector3 pointPosition)
    {
        if (status != TouchStatus.PointerDown && status != TouchStatus.Drag) return;
        status = TouchStatus.Drag;
        onDrag?.Invoke(pointPosition);
    }

    private void PointerUp(Vector3 pointPosition)
    {
        if (status != TouchStatus.PointerDown && status != TouchStatus.Drag) return;
        
        status = TouchStatus.PointerUp;
        onPointerUp?.Invoke(pointPosition);
        status = TouchStatus.Idle;
    }

    private void OnDestroy()
    {
        status = TouchStatus.Disable;

        onPointerDown = null;
        onDrag = null;
        onPointerUp = null;
    }
}
