using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework
{
    public static class ImageExtension
    {
        public static Image FillAmount(this Image self, float fillAmount)
        {
            if(self)
            {
                self.fillAmount = fillAmount;
            }
            return self;
        }
    }
}