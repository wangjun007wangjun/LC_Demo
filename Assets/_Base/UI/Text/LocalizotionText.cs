using UnityEngine;
using TMPro;

namespace BaseFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizotionText : MonoBehaviour
    {
        public string key;
        private void Awake()
        {
            if (!string.IsNullOrEmpty(key))
            {
                GetComponent<TextMeshProUGUI>().text = Localization.instance.GetText(key);
            }
        }
    }
}
