using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using BaseFramework.UI;
using UnityEngine;

public class FingerDialogPanel : Dialog
{
    public Transform content;

    public void Init()
    {
        EventManager.instance.RegistEvent<FingerDialogEvent>(OnFingerDialogEvent);
        GameController.GetInstance().crossPanel.emptyBoxPanel.SetParent(content);
        GameController.GetInstance().crossPanel.emptyBoxPanel.ChangeCanvasSorting("Dialog",2);
    }

    public void OnCloseButtonClick()
    {
        GameController.GetInstance().crossPanel.emptyBoxPanel.Parent(GameController.GetInstance().crossPanel.transform);
        
        EventManager.instance.DispatchEvent(BoxViewButtonEvent.instance.cancelClick());
        
        Close();
    }

    private void OnFingerDialogEvent(FingerDialogEvent @event)
    {
        if (@event.canDestroy)
        {
            GameController.GetInstance().crossPanel.emptyBoxPanel.Parent(GameController.GetInstance().crossPanel.transform);
            
            Close();
        }
    }
    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<FingerDialogEvent>(OnFingerDialogEvent);
    }
}
