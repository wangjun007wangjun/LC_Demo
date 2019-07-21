using BaseFramework;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    private static LoadingController _instance;

    public static LoadingController GetInstance()
    {
        return _instance;
    }
    
    public ProgressCutterBar progressCutterBar;

    private int _loadingIndex;
    private LoadingStep[] _loadingSteps;

    private float _startTime;

    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;
        } 
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        EventManager
           .instance
           .RegistEvent<LoadingPregressEvent>(Progress);

        InitLoading();

        _startTime = Time.realtimeSinceStartup;
        Log.I(this, $"load start at {_startTime}");
        Loading();
    }

    private void OnDisable()
    {
        EventManager
           .instance
           .UnRegistEvent<LoadingPregressEvent>();

        if(_instance == this)
        _instance = null;
    }

    private void Progress(LoadingPregressEvent it)
    {
        progressCutterBar.UpdateProgress(it.progress,
                                         0.5f,
                                         true,
                                         () => { EventManager.instance.DispatchEvent(new LoadFinishedEvent()); });

        if (it.progress.Ne(1))
        {
            Loading(++_loadingIndex);
        }
        else
        {
            float endTime = Time.realtimeSinceStartup;
            Log.I(this, $"load end at {endTime}, total use {endTime - _startTime}");
        }
    }

    private void InitLoading()
    {
        //test
        GameDialogManager.instance.Load();
        
        _loadingSteps = new[]
        {
            new LoadingStep(LevelManager.instance.Load, 0.6f),
            new LoadingStep(SpriteManager.instance.LoadThemeIcon, 0.7f),
            new LoadingStep(SpriteManager.instance.LoadEnvelopeCard, 1)
        };
    }

    private void Loading(int index = 0)
    {
        _loadingSteps[index].Invoke();
    }
}