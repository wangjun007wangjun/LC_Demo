using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class CardPanel : MonoBehaviour
{
    public GameObject leftObj;
    public GameObject topObj;
    public Image image;

    private LevelEnvelopeItemView _itemView;

    public void Show(LevelEnvelopeItemView itemView)
    {
        gameObject.SetActive(true);
        topObj.Inactive();
        leftObj.Inactive();

        _itemView = itemView;


        image.sprite = SpriteManager.instance.GetCardSprite(itemView.envelopeBean.cardName);
    }

    public void Onclick()
    {
        topObj.Active();
        leftObj.Active();
        gameObject.Inactive();

        if (_itemView.levelStatus == LevelStatus.Current
            && _itemView.envelopeBean.id > PlayerDataManager.levelInfo.unlockEnvelopeId)
        {
            ++PlayerDataManager.levelInfo.unlockEnvelopeId;
        }

        EventManager.instance.DispatchEvent(new CardCloseEvent(_itemView));
    }
}