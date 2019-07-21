using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseFramework
{
    public abstract class TransitionBase : MonoBehaviour
    {
        public Action<TransitionBase> onEnterCallback;
        public Action<TransitionBase> onExitCallback;
        protected Sequence tweener;

        protected virtual void OnDestroy()
        {
            KillTween();
            onEnterCallback = null;
            onExitCallback = null;
        }

        protected void KillTween()
        {
            if (tweener == null) return;
            tweener.Kill(false);
            tweener = null;
        }

        public abstract void Enter();

        public abstract void Exit();

        protected virtual void OnEnterComplete()
        {
            onEnterCallback?.Invoke(this);
        }

        protected virtual void OnExitComplete()
        {
        }
    }
}
