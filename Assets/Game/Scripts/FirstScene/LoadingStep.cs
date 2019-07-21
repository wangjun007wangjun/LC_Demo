using System;

public class LoadingStep
{
    private readonly Action<float> _action;
    private readonly float _progress;

    public LoadingStep(Action<float> action, float progress)
    {
        this._action = action;
        this._progress = progress;
    }

    public void Invoke()
    {
        _action?.Invoke(_progress);
    }
}