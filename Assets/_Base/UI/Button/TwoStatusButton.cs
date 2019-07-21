using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework.UI
{
    [RequireComponent(typeof(Button))]
    public class TwoStatusButton : MonoBehaviour
    {
        public bool isActive = true;
        public Sprite inactiveSprite;

        private Sprite _activeSprite;
        private Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();

            _activeSprite = _button.image.sprite;

            _button.onClick.AddListener(() => {
                isActive = !isActive;
                _button.image.sprite = isActive ? _activeSprite : inactiveSprite;
            });
        }
    }
}
