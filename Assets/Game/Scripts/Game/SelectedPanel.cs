using System.Collections.Generic;
using BaseFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectedPanel : MonoBehaviour
{
    public SelectedBlockView selectedBlockViewPrefab;
    
    private Tween _tween;
    private CoroutineTask _coroutineTask;

    private Image _bgImage;
    private Image bgImage
    {
        get
        {
            _bgImage = _bgImage ?? GetComponent<Image>();
            return _bgImage;
        }
    }

    public List<SelectedBlockView> selectedLetters { get;} = new List<SelectedBlockView>();

    private void Awake()
    {
        EventManager.instance.RegistEvent<LetterPanelEvent>(OnLetterPanelEvent);
    }

    private void OnLetterPanelEvent(LetterPanelEvent @event)
    {
        switch (@event.type)
        {
            case ELetterPanelEventType.RemoveLast:
                RemoveLast();
                break;
            case ELetterPanelEventType.ClearDelay:
                ClearDelay(@event.delay);
                break;
            case ELetterPanelEventType.ClearImmediately:
                ClearImmediately();
                break;
            case ELetterPanelEventType.AddLetter:
                AddLetter(@event.letter);
                break;
            case ELetterPanelEventType.Seleceted:
                break;
        }
    }

    private void OnDestroy()
    {
        EventManager.instance.UnRegistEvent<LetterPanelEvent>(OnLetterPanelEvent);
    }

    private void AddLetter(char letter)
    {
        SelectedBlockView selectedBlockView = SelectedBlockView.Instance(selectedBlockViewPrefab,
                                                                         transform,
                                                                         letter);
        selectedLetters.Add(selectedBlockView);

        if (selectedLetters.Count == 1)
        {
            bgImage.enabled = true;
        }
    }

    private void RemoveLast()
    {
        if (selectedLetters.Count <= 0) return;
        
        selectedLetters[selectedLetters.Count - 1].Dispose();
        selectedLetters.RemoveAt(selectedLetters.Count - 1);
    }

    private void ClearDelay(float delay)
    {
        _coroutineTask = TaskHelper.Create<CoroutineTask>()
                           .SetMonoBehaviour(this)
                           .Delay(delay)
                           .Do(Clear)
                           .OnFinish(() => _coroutineTask = null)
                           .OnCancel(() => _coroutineTask = null)
                           .Execute();
    }
    
    private void ClearImmediately()
    {
        _tween?.Kill();
        transform.localScale = Vector3.one;
        _coroutineTask?.Cancel();

        Clear();
    }

    private void Clear()
    {
        selectedLetters.ForEach((_, item) => { item.Dispose(); });
        selectedLetters.Clear();

        bgImage.enabled = false;
    }
}