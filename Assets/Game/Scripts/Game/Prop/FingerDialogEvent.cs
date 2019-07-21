using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using UnityEngine;

public class FingerDialogEvent : Singleton<FingerDialogEvent>
{
    public bool canDestroy = false;
    public FingerDialogEvent Destroy()
    {
        this.canDestroy = true;
        return this;
    }
}