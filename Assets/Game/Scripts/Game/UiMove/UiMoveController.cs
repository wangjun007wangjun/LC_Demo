using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseFramework;
using DG.Tweening;
using UnityEngine;

public class UiMoveController : MonoSingleton<UiMoveController>
{
    public MoveView topPanel;
    public MoveView levelTitlePanel;
    public MoveView[] leftPanel;
    public MoveView[] rightPanel;

    public void TopUiMoveIn(Action finish)
    {
        levelTitlePanel.PlayIn();     
        
        topPanel.PlayIn(()=> finish?.Invoke());
    }

    public void Hide(Action finish)
    {
        levelTitlePanel.PlayOut();     
        
        topPanel.PlayOut();
        LeftAndRightUiMoveout(()=> finish?.Invoke());
    }
    public void LeftAndRightUiMoveIn(Action finish)
    {
        float delay = 0;
        TaskHelper.Create<CoroutineTask>().Do(() =>
                                              {
                                                  leftPanel.ForEach((_, p) =>
                                                                    {
                                                                        p.PlayIn(null, delay);
                                                                        delay += 0.1f;
                                                                    });
                                                  delay = 0;
                                                  rightPanel.ForEach((_, p) =>
                                                                     {
                                                                         p.PlayIn(null, delay);
                                                                         delay += 0.1f;
                                                                     });
                                              }).OnFinish(() => finish?.Invoke())
                  .Execute();
    }

    public void LeftAndRightUiMoveout(Action finish)
    {
        float delay = 0;
        TaskHelper.Create<CoroutineTask>().Do(() =>
                                              {
                                                  leftPanel.ForEach((_, p) =>
                                                                    {
                                                                        p.PlayOut(null, delay);
                                                                        delay += 0.1f;
                                                                    });
                                                  delay = 0;
                                                  rightPanel.ForEach((_, p) =>
                                                                     {
                                                                         p.PlayOut(null, delay);
                                                                         delay += 0.1f;
                                                                     });
                                              }).OnFinish(() => finish?.Invoke())
                  .Execute();
    }
}
