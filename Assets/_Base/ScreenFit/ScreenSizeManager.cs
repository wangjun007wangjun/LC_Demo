using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public enum FitType
    {
        ChangeScale,
        ChangeSize,
        WidthScale,
        HeightScale,
        WidthSize,
        HeightSize
    }

    public enum StretchType
    {
        None,
        Width,
        Height
    }

    public class ScreenSizeManager : Singleton<ScreenSizeManager>
    {
        // 是否是特殊的屏幕尺寸，比如iPhonX的刘海屏幕
        private bool _isSpecificScreen;

        // 特殊屏幕偏移
        private Vector2 _specificOffset;

        private bool _isInit = false;

        private StretchType _stretchType;


        private float _idealRatio;
        private Vector2 _idealSize;

        private float _screenRatio;
        private Vector2 _screenSize;

        private float _canvasRealRatio;
        private Vector2 _canvasRealSize;

        private Vector2 _canvasSize;

        private string _logString;

        public void Init()
        {
            Vector2 size = new Vector2(ScreenSetting.instance.idealScreenSizeMin,
                                       ScreenSetting.instance.idealScreenSizeMax);
            Init(size, ScreenSetting.instance.defaultMatch);
        }

        public void Init(Vector2 size, float math)
        {
            if (_isInit) return;
            _screenSize = new Vector2(Screen.width, Screen.height);
            _screenRatio = _screenSize.x / _screenSize.y;

            float min = size.x > size.y ? size.y : size.x;
            float max = size.x + size.y - min;
            if (IsPortrait())
            {
                _idealSize = new Vector2(min, max);
            }
            else if (IsLandscape())
            {
                _idealSize = new Vector2(max, min);
            }

            _idealRatio = _idealSize.x / _idealSize.y;

            // 计算canvas的大小
            float logWidth = Mathf.Log(_screenSize.x / _idealSize.x, 2f);
            float logHeight = Mathf.Log(_screenSize.y / _idealSize.y, 2f);
            float scaleFactor = Mathf.Pow(2f, Mathf.Lerp(logWidth, logHeight, math));
            _canvasRealSize = _screenSize / scaleFactor;
            _canvasSize = _canvasRealSize;

#if !UNITY_EDITOR
                Vector2 safeArea = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
                if(_screenSize != safeArea)
                {
                    _isSpecificScreen = true;
                    _specificOffset = _canvasRealSize - safeArea;
                    _canvasRealSize = safeArea;
                }
#endif

            _canvasRealRatio = _canvasRealSize.x / _canvasRealSize.y;

            if (_canvasRealRatio.Eq(_idealRatio))
            {
                _stretchType = StretchType.None; // 未拉伸
            }

            if (_canvasRealRatio.Gt(_idealRatio))
            {
                _stretchType = IsPortrait() ? StretchType.Height : StretchType.Width; // eg：iphoneX
            }
            else
            {
                _stretchType = IsPortrait() ? StretchType.Width : StretchType.Height; //eg: ipad
            }

            _isInit = true;
        }

        public bool IsPortrait()
        {
            return Screen.orientation == ScreenOrientation.Portrait
                   || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        }

        public bool IsLandscape()
        {
            return Screen.orientation == ScreenOrientation.Landscape
                   || Screen.orientation == ScreenOrientation.LandscapeLeft
                   || Screen.orientation == ScreenOrientation.LandscapeRight;
        }

        public float GetIdealRatio()
        {
            Init();
            return _idealRatio;
        }

        public Vector2 GetIdealSize()
        {
            Init();
            return _idealSize;
        }

        public float GetScreenRatio()
        {
            Init();
            return _screenRatio;
        }

        public Vector2 GetScreenSize()
        {
            Init();
            return _screenSize;
        }

        public float GetCanvasRealRatio()
        {
            Init();
            return _canvasRealRatio;
        }

        public Vector2 GetCanvasRealSize()
        {
            Init();
            return _canvasRealSize;
        }

        public Vector2 GetCanvasSize()
        {
            Init();
            return _canvasSize;
        }

        public StretchType GetStretchType()
        {
            Init();
            return _stretchType;
        }

        public bool ShouldFix()
        {
            Init();
            return _stretchType != StretchType.None;
        }

        public bool IsSpecificScreen()
        {
            Init();
            return _isSpecificScreen;
        }

        public Vector2 GetSpecificOffset()
        {
            Init();
            return _specificOffset;
        }

        public override string ToString()
        {
            Init();
            if (!string.IsNullOrEmpty(_logString)) return _logString;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("screenModle:").Append(Screen.orientation)
                         .Append("\n idealRatio:").Append(_idealRatio)
                         .Append("\n idealSize:").Append(_idealSize)
                         .Append("\n screenRatio:").Append(_screenRatio)
                         .Append("\n screenSize").Append(_screenSize)
                         .Append("\n canvasRealRatio:").Append(_canvasRealRatio)
                         .Append("\n canvasRealSize:").Append(_canvasRealSize);
            _logString = stringBuilder.ToString();
            return _logString;
        }
    }
}