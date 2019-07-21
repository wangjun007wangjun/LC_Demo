using UnityEngine;
using BaseFramework;
using System.Collections.Generic;

public class ExtraInfo : Singleton<ExtraInfo>
{
    private const string EXTRA_WORDS = "ExtraWords";
    private const string EXTRA_CURRENT_COUNT = "ExtraCurrentCount";
    private const string EXTRA_TOTAL_COUNT = "ExtraTotalCount";
    private const string EXTRA_REWARD_COUNT = "ExtraRewardCount";
    
    private List<string> _extraWords;

    /// <summary>
    /// 未领取奖励的extra count
    /// </summary>
    private int _extraCurrentCount;
    public int extraCurrentCount
    {
        get => _extraCurrentCount;
        set
        {
            _extraCurrentCount = value;

            PlayerPrefs.SetInt(EXTRA_CURRENT_COUNT, _extraCurrentCount);
        }
    }

    /// <summary>
    /// extra 总个数
    /// </summary>
    private int _extraTotalCount;
    public int extraTotalCount
    {
        get => _extraTotalCount;
        private set
        {
            _extraTotalCount = value;

            PlayerPrefs.SetInt(EXTRA_TOTAL_COUNT, _extraTotalCount);
        }
    }

    /// <summary>
    /// 领取奖励的次数
    /// </summary>
    private int _extraGetRewardCount;
    public int extraGetRewardCount
    {
        get => _extraGetRewardCount;
        set
        {
            _extraGetRewardCount = value;

            PlayerPrefs.SetInt(EXTRA_REWARD_COUNT, _extraGetRewardCount);
        }
    }

    public int extraTargetCount => extraGetRewardCount < Const.Value.EXTRA_WORD_REWARD_LIMIT_COUNT
                                       ? Const.Value.EXTRA_WORD_TARGET_COUNT1
                                       : Const.Value.EXTRA_WORD_TARGET_COUNT2;

    public override void OnSingletonInit()
    {
        base.OnSingletonInit();

        _extraWords = PlayerPrefsUtil.GetStrings(EXTRA_WORDS);
        _extraCurrentCount = PlayerPrefs.GetInt(EXTRA_CURRENT_COUNT);
        _extraTotalCount = PlayerPrefs.GetInt(EXTRA_TOTAL_COUNT);
        _extraGetRewardCount = PlayerPrefs.GetInt(EXTRA_REWARD_COUNT);
    }

    public bool IsWordExist(string word)
    {
        return _extraWords.Contains(word);
    }

    public void AddWord(string word)
    {
        _extraWords.Add(word);
        PlayerPrefsUtil.SetStrings(EXTRA_WORDS, _extraWords);
        extraCurrentCount++;
        extraTotalCount++;
    }

    public List<string> GetWords()
    {
        return _extraWords;
    }

    public void ClearWords()
    {
        _extraWords.Clear();
        PlayerPrefsUtil.SetStrings<string>(EXTRA_WORDS, null);
    }
    public void ClearRewardWord()
    {
        int count = (_extraWords.Count) < extraTargetCount ? _extraWords.Count : extraTargetCount;
        _extraWords.RemoveRange(0,count);
    }
    public void ClearAll()
    {
        ClearWords();
        extraCurrentCount = 0;
        extraTotalCount = 0;
    }
}
