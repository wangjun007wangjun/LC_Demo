using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ProgressCutterImage : MonoBehaviour
{
    private static readonly int HEIGHT_RATE = Shader.PropertyToID("_HeightRate");
    private static readonly int ANGLE = Shader.PropertyToID("_Angle");
    private static readonly int CUT_POS = Shader.PropertyToID("_CutPos");

    public float startProgress = 0;
    private Material _moveImgMat;
    private float _heightRate;
    private float _originAngle;
    private bool _isInitFinish = false;

    private void Init()
    {
        if (_isInitFinish) return;
        
        _isInitFinish = true;
        
        Image moveImg = GetComponent<Image>();
        Rect rect = moveImg.rectTransform.rect;
        _heightRate = rect.height / rect.width;
        _moveImgMat = moveImg.material;
        _moveImgMat.SetFloat(HEIGHT_RATE, _heightRate);
        _originAngle = _moveImgMat.GetFloat(ANGLE);
    }

    public float GetProgress()
    {
        Init();
        return _moveImgMat.GetFloat(CUT_POS);
    }

    public void SetProgress(float progress)
    {
        Init();
        progress = Mathf.Clamp01(progress);
        if (progress < _heightRate)
        {
            float angle = -(90 - _originAngle) / _heightRate * progress + 90;
            _moveImgMat.SetFloat(ANGLE, angle);
        }
        else
        {
            _moveImgMat.SetFloat(ANGLE, _originAngle);
        }

        _moveImgMat.SetFloat(CUT_POS, progress);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        Init();
        _moveImgMat.SetFloat(CUT_POS, startProgress);
    }

    private void OnDisable()
    {
        _moveImgMat.SetFloat(ANGLE, _originAngle);
        _moveImgMat.SetFloat(CUT_POS, startProgress);
    }
#endif
}