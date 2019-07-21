using BaseFramework;
using UnityEngine;

public class VersionInfo : Singleton<VersionInfo>
{
    private const string APP_VERSION = "app_version";

    public string appVersion
    {
        get => Application.version;
        set => PlayerPrefs.SetString(APP_VERSION, value);
    }

    public override void OnSingletonInit()
    {
        base.OnSingletonInit();

        if (PlayerPrefs.GetString(APP_VERSION, "") != Application.version)
        {
            appVersion = Application.version;
        }
    }
}