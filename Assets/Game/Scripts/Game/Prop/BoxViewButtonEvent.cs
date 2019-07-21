using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using UnityEngine;

public class BoxViewButtonEvent : Singleton<BoxViewButtonEvent>
{
    public bool canClick;
    public BoxViewButtonEvent onClick()
    {
        this.canClick = true;
        return this;
    }

    public BoxViewButtonEvent cancelClick()
    {
        this.canClick = false;
        return this;
    }
}
