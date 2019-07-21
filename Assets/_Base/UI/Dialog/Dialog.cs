using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BaseFramework.UI
{
    [RequireComponent(typeof(Image))]
    public abstract class Dialog : MonoBehaviour, IPointerDownHandler
    {
        public enum DialogStatus
        {
            None,
            Init,
            Opening,
            Opened,
            Closing,
            Disable,
            Closed
        }

        public DialogStatus dialogStatus;
        public bool ignoreRaycaster = false;
        public bool dontDestroyOnLoad = false;
        public bool destroyOnClose = true;
        public bool closeByBack = true;
        public bool closeByClickBlank = false;
        public bool playOpenSound = true;
        public bool playCloseSound = true;
        public bool playOpenAnimation = true;
        public bool playCloseAnimation = false;
        public float openAnimationTime = 0.5f;
        public float closeAnimationTime = 0.25f;

        public string dialogName { get; private set; }

        protected Transform fitTransform;
        protected Transform contentTransform;
        protected Image bgImage;
        protected GraphicRaycaster graphicRaycaster;

        private Action<Dialog> _onOpened;
        private Action<Dialog> _onClose;
        private Action<Dialog> _onDisable;
        private Action<Dialog> _onDestroy;

        protected virtual void Awake()
        {
            dialogStatus = DialogStatus.None;
            
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            bgImage = GetComponent<Image>();
            fitTransform = transform.Find("FitPanel");
            contentTransform = transform.Find("ContentPanel");
            if (contentTransform == null)
            {
                contentTransform = transform.Find("FitPanel/ContentPanel");
            }

            transform.SetAsLastSibling();

            SetIgnoreRaycaster(ignoreRaycaster);

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            dialogStatus = DialogStatus.Init;
        }

        protected virtual void OnEnable()
        {
            dialogStatus = DialogStatus.Opening;

            if (playOpenSound)
            {
                PlayOpenSound();
            }

            if (playOpenAnimation && contentTransform != null)
            {
                PlayOpenAnimation();
            }
            else
            {
                _onOpened?.Invoke(this);

                dialogStatus = DialogStatus.Opened;
            }
        }

        private void Update()
        {
            if (closeByBack && Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackClick();
            }
        }

        protected virtual void OnBackClick()
        {
            Close();
        }

        protected virtual void OnDisable()
        {
            dialogStatus = DialogStatus.Disable;

            _onDisable?.Invoke(this);

            _onOpened = null;
            _onClose = null;
            _onDisable = null;
        }

        protected virtual void OnDestroy()
        {
            dialogStatus = DialogStatus.None;

            _onDestroy?.Invoke(this);
            _onDestroy = null;
        }

        public virtual Dialog SetDialogName(string name)
        {
            this.dialogName = name;

            return this;
        }

        public virtual Dialog SetIgnoreRaycaster(bool ignoreRaycaster)
        {
            this.ignoreRaycaster = ignoreRaycaster;
            graphicRaycaster.enabled = !ignoreRaycaster;

            return this;
        }

        protected virtual void PlayOpenSound()
        {
        }

        protected virtual void PlayCloseSound()
        {
        }

        private void PlayOpenAnimation()
        {
            Tween anim = GetOpenAnimation();
            if (anim == null)
            {
                _onOpened?.Invoke(this);
                dialogStatus = DialogStatus.Opened;
            }
            else
            {
                anim.OnComplete(() =>
                                              {
                                                  _onOpened?.Invoke(this);
    
                                                  dialogStatus = DialogStatus.Opened;
                                              });
            }
        }

        protected virtual Tween GetOpenAnimation()
        {
            return null;
        }

        private void PlayCloseAnimation()
        {  
            Tween anim = GetCloseAnimation();
            if (anim == null)
            {
                DoClose();
            }
            else
            {
                anim.OnComplete(DoClose);
            }
        }

        protected virtual Tween GetCloseAnimation()
        {
            return null;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (closeByClickBlank && eventData.pointerEnter == gameObject)
            {
                Close();
            }
        }

        public virtual Dialog Open()
        {
            if (!IsOpened())
            {
                gameObject.SetActive(true);
            }

            return this;
        }

        public virtual bool IsOpened()
        {
            if (this == null || gameObject == null)
            {
                return false;
            }

            return dialogStatus == DialogStatus.Opening && gameObject.activeSelf;
        }

        public virtual void Close()
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            if (dialogStatus != DialogStatus.Opened) return;
            dialogStatus = DialogStatus.Closing;

            _onClose?.Invoke(this);

            if (playCloseSound)
            {
                PlayCloseSound();
            }

            if (playCloseAnimation)
            {
                PlayCloseAnimation();
            }
            else
            {
                DoClose();
            }
        }

        private void DoClose()
        {
            /*if (destroyOnClose)
            {
                gameObject.Destroy();
            }
            else
            {
                gameObject.Inactive();
            }*/
            gameObject.Inactive();
            dialogStatus = DialogStatus.Closed;
        }

        public Dialog OnOpend(Action<Dialog> action)
        {
            _onOpened += action;

            return this;
        }

        public Dialog OnClose(Action<Dialog> action)
        {
            _onClose += action;

            return this;
        }

        public Dialog OnDisable(Action<Dialog> action)
        {
            _onDisable += action;

            return this;
        }

        public Dialog OnDestroy(Action<Dialog> action)
        {
            _onDestroy += action;

            return this;
        }
    }
}
