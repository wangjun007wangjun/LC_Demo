using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(CreatDialog);
    }

    private void CreatDialog()
    {
        ExtraDialog extraDialog = GameDialogManager.instance.Open("ExtraDialog") as ExtraDialog;
        extraDialog.transform.SetAsLastSibling();
        extraDialog.InitWordContent();
    }
}
