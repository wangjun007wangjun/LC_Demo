using BaseFramework;
using UnityEngine;

public class HomeController : MonoBehaviour
{
    private static HomeController _instance;

    public static HomeController GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } 
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(_instance == this)
            _instance = null;
    }

    public void OnPlayClick()
    {
        PlayerDataManager.levelInfo.currentThemeIndex = PlayerDataManager.levelInfo.unlockThemeIndex;
        
        SceneHelper.instance.LoadScene(Const.Scene.LEVEL,
                                       0,
                                       BlindTransition.instance.Enter,
                                       BlindTransition.instance.Exit);
    }
}