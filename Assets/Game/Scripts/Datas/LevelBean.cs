using System;
using System.Collections.Generic;
using System.Linq;
using BaseFramework;
using UnityEngine;

[Serializable]
public class LevelBean
{
    public int r;
    public int c;
    public string w;
    public string[] @as;
    public string[] bs;
    public string[] vs;
    public string[] es;

    public int id { get; private set; }
    public int idInTheme { get; private set; }
    public int idInEnvelope { get; private set; }

    public int themeId { get; private set; }
    public int envelopeId { get; private set; }
    
    public Color color { get; private set; }
    
    public CrossWordBean[] answerWordBeans { get; private set; }
    public CrossWordBean[] bounsWordBeans { get; private set; }
    public CrossWordBean[] bibleWordBeans { get; private set; }
    public List<CrossWordBean> allCrossWordBeans { get; private set; }
    
    public string[] extraWords => es;

    public void Init(int id,
                     int idInTheme, 
                     int idInEnvelope,
                     int themeId,
                     int envelopeId,
                     Color color)
    {
        this.id = id;
        this.idInTheme = idInTheme;
        this.idInEnvelope = idInEnvelope;

        this.color = color;

        this.themeId = themeId;
        this.envelopeId = envelopeId;

        if (@as != null)
        {
            answerWordBeans = new CrossWordBean[@as.Length];
            @as.ForEach((index, it) =>
                        {
                            CrossWordBean crossWordBean = new CrossWordBean(it);
                            answerWordBeans[index] = crossWordBean;
                            allCrossWordBeans = allCrossWordBeans ?? new List<CrossWordBean>();
                            allCrossWordBeans.Add(crossWordBean);
                        });
        }
        else
        {
            Log.W(this, "answer word is null");
        }

        if (bs != null)
        {
            bounsWordBeans = new CrossWordBean[bs.Length];
            bs.ForEach((index, it) =>
                       {
                           CrossWordBean crossWordBean = new CrossWordBean(it);
                           bounsWordBeans[index] = crossWordBean;
                           allCrossWordBeans = allCrossWordBeans ?? new List<CrossWordBean>();
                           allCrossWordBeans.Add(crossWordBean);
                       });
        }
        else
        {
            Log.I(this, "bouns word is null");
        }

        if (vs != null)
        {
            bibleWordBeans = new CrossWordBean[vs.Length];
            vs.ForEach((index, it) =>
                       {
                           CrossWordBean crossWordBean = new CrossWordBean(it);
                           bibleWordBeans[index] = crossWordBean;
                           allCrossWordBeans = allCrossWordBeans ?? new List<CrossWordBean>();
                           allCrossWordBeans.Add(crossWordBean);
                       });
        }
        else
        {
            Log.I(this, "bible word is null");
        }
    }

    public bool IsBonus(CrossWordBean crossWordBean)
    {
        return bounsWordBeans.Any() && bounsWordBeans.Contains(crossWordBean);
    }

    public bool IsBible(CrossWordBean crossWordBean)
    {
        return bibleWordBeans.Any() && bibleWordBeans.Contains(crossWordBean);
    }
    
    public bool IsBonusWord(string word)
    {
        bool isBonusWord = false;
        if (bounsWordBeans.Any())
        {
            isBonusWord = Array.Find(bounsWordBeans, it => it.word == word) != null;
        }
        return isBonusWord;
    }

    public bool IsBibleWord(string word)
    {
        bool isBibleWord = false;
        if (bibleWordBeans.Any())
        {
            isBibleWord = Array.Find(bibleWordBeans, it => it.word == word) != null;
        }
        return isBibleWord;
    }

    public bool IsExtra(string word)
    {
        return extraWords.Any() && extraWords.Contains(word);
    }
}