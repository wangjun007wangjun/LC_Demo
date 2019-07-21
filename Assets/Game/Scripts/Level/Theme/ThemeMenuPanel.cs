using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThemeMenuPanel : MonoBehaviour
{
    private enum MenuPanelStatus
    {
        Hide,
        Showing,
        Show,
        Hiding
    }

    public RectTransform moveRectTransform;
    public RectTransform contentRectTransform;
    public Image maskBgImg;
    public Image arrowImg;
    public float aniTime = 0.3f;

    private MenuPanelStatus _panelStatus;
    private Tween _tween;

    public void OnArrowClick()
    {
        switch (_panelStatus)
        {
            case MenuPanelStatus.Hide:
                ShowMenuPanel();
                break;
            case MenuPanelStatus.Show:
                HideMenuPanel();
                break;
        }
    }

    private void ShowMenuPanel()
    {
        _tween?.Kill();

        _tween = DOTween.Sequence()
                        .Append(
                                moveRectTransform
                                   .DOAnchorPosX(contentRectTransform.rect.width,
                                                 aniTime)
                                   .SetEase(Ease.OutSine))
                        .Insert(0, maskBgImg.DOFade(0.65f, aniTime))
                        .Insert(0, arrowImg.transform.DOLocalRotate(new Vector3(0, 0, 180), aniTime))
                        .OnStart(() =>
                                 {
                                     _panelStatus = MenuPanelStatus.Showing;
                                     contentRectTransform.Active();
                                     maskBgImg.Active();
                                 })
                        .OnComplete(() => _panelStatus = MenuPanelStatus.Show);
    }

    private void HideMenuPanel()
    {
        _tween?.Kill();

        _tween = DOTween.Sequence()
                        .Append(
                                moveRectTransform
                                   .DOAnchorPosX(0, aniTime)
                                   .SetEase(Ease.InSine))
                        .Insert(0, maskBgImg.DOFade(0, aniTime))
                        .Insert(0, arrowImg.transform.DOLocalRotate(new Vector3(0, 0, 0), aniTime))
                        .OnStart(() => _panelStatus = MenuPanelStatus.Hiding)
                        .OnComplete(() =>
                                    {
                                        _panelStatus = MenuPanelStatus.Hide;
                                        contentRectTransform.Inactive();
                                        maskBgImg.Inactive();
                                    });
    }
}