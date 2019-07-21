using System.Collections.Generic;
using BaseFramework;
using UnityEngine;
using UnityEngine.Experimental.UIElements;


public class CrossWord : SimpleRecycleItem<CrossWord>
{
    public string word;

    private List<BoxView> _boxViews;

    public bool isAnswered { get; private set; }

    public List<BoxView> boxViews => _boxViews;
    public CrossWord(string word)
    {
        this.word = word;
    }

    public void AddBoxView(BoxView boxView)
    {
        _boxViews = _boxViews ?? new List<BoxView>(word.Length);

        _boxViews.Add(boxView);
    }

    public void Answer(List<SelectedBlockView> selectedBlockViews)
    {
        isAnswered = true;
        
        int delayIndex = 0;
        _boxViews.ForEach((index, it) =>
                          {
                              if(it.isAnswered) return;

                              it.Answer(delayIndex++ * 0.05f, selectedBlockViews[index]);
                          });
    }
    
    public void Shake()
    {
        _boxViews.ForEach((index,it) => it.Shake(0.01f * index));
    }

    public void CheckIsAnswered()
    {
        bool isAnsweredTemp = true;
        _boxViews.ForEach(it =>
                                           {
                                               isAnsweredTemp = isAnsweredTemp && it.isAnswered;
                                           });
        isAnswered = isAnsweredTemp; 
    }
}