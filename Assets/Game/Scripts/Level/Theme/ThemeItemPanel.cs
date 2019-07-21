using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThemeItemPanel : MonoBehaviour
{
    public Image iconImg;
    public Text nameText;
    public Text progressText;
    public Image progressImage;

    public void Init(ThemeBean themeBean)
    {
        iconImg.sprite = SpriteManager.instance.GetIconSprite(themeBean.iconName);
        int progress = 0;
        bool forceShow = false;
        if (PlayerDataManager.levelInfo.unlockThemeIndex > themeBean.id)
        {
            progress = 100;
        }
        else if (PlayerDataManager.levelInfo.unlockThemeIndex == themeBean.id)
        {
            forceShow = true;
            LevelBean levelBean = LevelManager.instance.GetLevel(PlayerDataManager.levelInfo.unlockThemeIndex,
                                                                 PlayerDataManager.levelInfo.unlockEnvelopeIndex,
                                                                 PlayerDataManager.levelInfo.unlockLevelIndex);
            if (levelBean != null)
            {
                progress = (int)(levelBean.idInTheme * 100f / themeBean.levelCount);
            }
        }

        if (forceShow || progress > 0)
        {
            progressText.enabled = true;
            progressText.text = progress + "%";
        }
        else
        {
            progressText.enabled = false;
        }
        progressImage.fillAmount = progress / 100f;
        
        nameText.text = themeBean.n;
    }

    public void Init(int index)
    {
        nameText.text = index + "";
    }
}