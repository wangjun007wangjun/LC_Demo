using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FingerButton : MonoBehaviour
{
    public FingerDialogPanel fingerUI;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(CreatFingerUI);
    }

    private void CreatFingerUI()
    {
        FingerDialogPanel fingerUi = GameDialogManager.instance.Open("FingerDialog") as FingerDialogPanel;
        fingerUi.transform.SetAsLastSibling();
        
         fingerUi.Init();
         
         EventManager.instance.DispatchEvent(BoxViewButtonEvent.instance.onClick());//设置所有Box View Button        
    }
}
