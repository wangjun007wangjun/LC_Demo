using BaseFramework;
using UnityEngine;

public class FirstSceneController : MonoBehaviour
{
    public GameObject loadingController;
    public GameObject homeController;


    private void Awake()
    {
        bool isFirstTime = SceneHelper.instance.GetSceneCount(Const.Scene.FIRST_SCENE) == 0;
        if (isFirstTime)
        {
            loadingController.SetActive(true);
            EventManager.instance.RegistEvent<LoadFinishedEvent>((_) => OnLoadFinished());
        }
        else
        {
            homeController.SetActive(true);
            Destroy(loadingController);
        }
    }

    private void OnLoadFinished()
    {
        Destroy(loadingController);
        homeController.Active();

        EventManager.instance.UnRegistEvent<LoadFinishedEvent>();
    }
}