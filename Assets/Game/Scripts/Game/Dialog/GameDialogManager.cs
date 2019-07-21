using System;
using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using BaseFramework.UI;
using UnityEngine;

public class GameDialogManager : Singleton<GameDialogManager> {

    private Dictionary<string, Dialog> dialogDic = new Dictionary<string, Dialog>();
    private Dictionary<string, Dialog> dialogPool = new Dictionary<string, Dialog>();

    private Dialog currentShowDialog;

    public void Load()
    {
        InitDialogConfig();
    }

    void InitDialogConfig() {
        foreach (Dialog dialog in Resources.LoadAll<Dialog>("Dialog")) {
            dialogDic.Add(dialog.name, dialog);
        }
    }

    public Dialog Open(string name, Action<Dialog> onClose = null, Action<Dialog> OnDisable = null) {
        Dialog dialog = null;

        if (dialogPool.ContainsKey(name)) {
            dialog = dialogPool[name];
        } else if (dialogDic.ContainsKey(name)) {
            dialog = UnityEngine.Object.Instantiate(dialogDic[name]);
            dialog.name = name;

            dialogPool.Add(name, dialog);
        }
        if (dialog) {
            dialog.OnClose(onClose).OnDisable(OnDisable).Open();
        }

        currentShowDialog = dialog;

        return dialog;
    }

    public bool DestroyDialog(string name) {
        if (dialogPool.ContainsKey(name)) {
            dialogPool.Remove(name);

            return true;
        }

        return false;
    }

    public bool DestroyDialog(Dialog dialog) {
        if(dialog == currentShowDialog) {
            currentShowDialog = null;
        }
        return DestroyDialog(dialog.name);
    }

    public void DisPlayDialog(Dialog dialog) {
        if (dialog == currentShowDialog) {
            currentShowDialog = null;
        }
    }

    public void ClearDialogPool()
    {
        dialogPool.Clear();
    }

    public bool IsDialogShowing() {
        return currentShowDialog != null;
    }
}
