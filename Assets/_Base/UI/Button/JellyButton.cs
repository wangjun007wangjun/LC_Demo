using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BaseFramework.UI
{
    public class JellyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // 按下时放大值
        public float pointDownScale = 1.1f;
        // 按下时放大时间
        public float pointDownScaleTime = 0.13f;
        // 果冻动画第一次缩小的时间
        public float time = 0.1f;
        // 果冻动画每次递减时间
        public float decreaseTime = 0.01f;
        // 果冻动画第一次缩小值
        public float shrink = 0.8f;
        // 果冻动画每次缩小比上次增加的值
        public float increaseShrink = 0.1f;
        // 果冻动画第一次放大值
        public float expand = 1.05f;
        // 果冻动画每次放大比上次放大减小的值
        public float decreaseExpand = 0.03f;

        public UnityEvent onPointDownEvent;
        public UnityEvent onPointUpEvent;

        private Vector3 _originScale;

        protected virtual void Awake()
        {
            _originScale = transform.localScale;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            transform.DOScale(_originScale * pointDownScale, pointDownScaleTime);

            onPointDownEvent?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            DOTween.Sequence()
                .Append(transform.DOScale(_originScale * shrink, time))
                .Append(transform.DOScale(_originScale * expand, time - decreaseTime))
                .Append(transform.DOScale(_originScale * (shrink + increaseShrink), time - decreaseTime * 2))
                .Append(transform.DOScale(_originScale * (expand - decreaseExpand), time - decreaseTime * 3))
                .Append(transform.DOScale(_originScale, time - decreaseTime * 4))
                .OnComplete(() =>
                            {
                                onPointUpEvent?.Invoke();
                            });
        }
    }
}
