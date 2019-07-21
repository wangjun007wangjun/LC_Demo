using System;
using UnityEngine;

namespace BaseFramework
{

    public class ScreenFitSpecific : MonoBehaviour
    {
#if UNITY_IOS || UNITY_EDITOR
        public enum FitType
        {
            Top, Bottom, Full
        }

        public FitType fitType = FitType.Full;
        private RectTransform _rectTransform;

        private void Awake()
        {
            if (!ScreenSizeManager.instance.IsSpecificScreen()) return;
            _rectTransform = transform as RectTransform;

            Vector2 offset = ScreenSizeManager.instance.GetSpecificOffset() / 2;

            switch (fitType)
            {
                case FitType.Full:
                    _rectTransform.offsetMax -= offset;
                    _rectTransform.offsetMin += offset;
                    break;
                case FitType.Top:
                    _rectTransform.offsetMax -= offset;
                    break;
                case FitType.Bottom:
                    _rectTransform.offsetMin += offset;
                    break;
            }
        }
#endif
    }
}
