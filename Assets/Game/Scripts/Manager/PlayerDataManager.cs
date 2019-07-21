using BaseFramework;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    public static VersionInfo versionInfo => VersionInfo.instance;

    public static LevelInfo levelInfo => LevelInfo.instance;
    
    public static ExtraInfo extraInfo => ExtraInfo.instance;
}