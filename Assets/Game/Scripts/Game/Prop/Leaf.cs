using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class Leaf : MonoRecycleItem<Leaf>
{
    public void Play(Vector3 start, Vector3 end, float time, float delay, System.Action onComplete = null)
    {
        transform.position = start;
        transform.DOMove(end, time).SetDelay(delay).OnComplete(() =>
                                                               {
                                                                   if (this != null)
                                                                   {
                                                                       this.Dispose();
                                                                   }

                                                                   if (onComplete != null)
                                                                   {
                                                                       onComplete.Invoke();
                                                                   }
                                                               });
    }
}
