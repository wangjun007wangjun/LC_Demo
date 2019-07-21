using System;
using UnityEngine;

namespace BaseFramework
{
    public class ScreenFit : MonoBehaviour
    {
        [Range(0,1)]
        public float matchWidthOrHeight = 0.5f;
        [Tooltip("selected matchWidthOrHeight will not use")]
        public bool auto = true;
        public FitType fitType;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            Fit();
        }

        private void Fit()
        {
            if (!ScreenSizeManager.instance.ShouldFix()) return;
            if (auto)
            {
                matchWidthOrHeight = ScreenSizeManager.instance.GetStretchType() == StretchType.Height ? 1 : 0;
            }

            Vector2 scaleVector = ScreenSizeManager.instance.GetCanvasRealSize() / ScreenSizeManager.instance.GetIdealSize();
            float logWidth = Mathf.Log(scaleVector.x, 2f);
            float logHeight = Mathf.Log(scaleVector.y, 2f);
            float scale = Mathf.Pow(2f, Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight));

            switch (fitType)
            {
                case FitType.ChangeScale:
                    transform.LocalScale(transform.localScale * scale);
                    break;
                case FitType.WidthScale:
                    transform.LocalScaleX(transform.localScale.x * scale);
                    break;
                case FitType.HeightScale:
                    transform.LocalScaleY(transform.localScale.y * scale);
                    break;
                case FitType.ChangeSize:
                    _rectTransform.sizeDelta = _rectTransform.sizeDelta * scale;
                    break;
                case FitType.WidthSize:
                    _rectTransform.sizeDelta = _rectTransform.sizeDelta.NewX(_rectTransform.sizeDelta.x * scale);
                    break;
                case FitType.HeightSize:
                    _rectTransform.sizeDelta = _rectTransform.sizeDelta.NewY(_rectTransform.sizeDelta.y * scale);
                    break;                   
            }
        }
    }
}
