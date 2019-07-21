using System.Collections;
using System.Collections.Generic;
using BaseFramework;
using UnityEngine;
using UnityEngine.UI;

public class ExtraWord : MonoRecycleItem<ExtraWord>
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Init(string word)
    {
        text.text = word;
    }
}
