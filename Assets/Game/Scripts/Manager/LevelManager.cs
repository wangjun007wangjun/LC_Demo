using System;
using System.Collections.Generic;
using BaseFramework;
using Newtonsoft.Json;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private const string AB_NAME = "config/level";
    private const string RES_NAME = "level_config";

    private LoadingStatus _loadingStatus;

    private List<ThemeBean> _themeBeans;

    #region Init

    public void Load(float progress)
    {
        if (_loadingStatus != LoadingStatus.Idle)
            return;

        _loadingStatus = LoadingStatus.Loading;

        float startTime = Time.realtimeSinceStartup;
        Log.I(this, $"load start at {startTime}");
        ResHelper.LoadAssetAsync<TextAsset>(AB_NAME,
                                            RES_NAME,
                                            (it) =>
                                            {
                                                float endTime = Time.realtimeSinceStartup;
                                                Log.I(this, $"load end at {endTime}, total use {endTime - startTime}");

                                                _loadingStatus = LoadingStatus.Finished;

                                                ParseConfig(it);

                                                EventManager.instance.DispatchEvent(new LoadingPregressEvent(progress));
                                            },
                                            true);
    }

    private void ParseConfig(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Log.W(this, "text asset is null");
            return;
        }

        if (textAsset.text.IsNullOrEmpty())
        {
            Log.W(this, "text is null");
            return;
        }

        try
        {
            _themeBeans = JsonConvert.DeserializeObject<List<ThemeBean>>(textAsset.text);

            InitDatas();

            Log.I(this, "parse data success");
        }
        catch (Exception e)
        {
            Log.W(this, $"parse data error! {e.Message}");
        }
    }

    private void InitDatas()
    {
        int envelopeIndex = 0;
        int levelIndex = 0;

        _themeBeans.ForEach((themeIndex, theme) =>
                            {
                                int levelCount = 0;
                                int levelIndexInTheme = 0;
                                theme.es.ForEach((envelopeIndexInTheme, envelope) =>
                                                 {
                                                     ColorUtility.TryParseHtmlString(theme.c, out Color color);
                                                     envelope.Init(envelopeIndex++,
                                                                   envelopeIndexInTheme,
                                                                   themeIndex,
                                                                   color);

                                                     envelope.ls.ForEach((levelIndexInEnvelop, level) =>
                                                                         {
                                                                             level.Init(levelIndex++,
                                                                                        levelIndexInTheme++,
                                                                                        levelIndexInEnvelop,
                                                                                        themeIndex,
                                                                                        envelopeIndexInTheme,
                                                                                        color);

                                                                             ++levelCount;
                                                                         });
                                                 });

                                theme.Init(themeIndex, levelCount);
                            });
    }

    #endregion

    #region Get

    public List<ThemeBean> GetThemes()
    {
        return _themeBeans;
    }

    public int _themeCount = -1;

    public int themeCount
    {
        get
        {
            if (_themeCount == -1)
            {
                _themeCount = _themeBeans?.Count ?? 0;
            }

            return _themeCount;
        }
    }

    private int _allEnvelopCount = -1;

    public int allEnvelopCount
    {
        get
        {
            if (_allEnvelopCount == -1)
            {
                _allEnvelopCount = 0;
                _themeBeans.ForEach((index, it) => _allEnvelopCount += it.envelopeCount);
            }

            return _allEnvelopCount;
        }
    }

    public int _allLevelCount = -1;

    public int allLevelCount
    {
        get
        {
            if (_allLevelCount == -1)
            {
                _allLevelCount = 0;
                _themeBeans.ForEach((index, it) => _allLevelCount += it.levelCount);
            }

            return _allLevelCount;
        }
    }

    public ThemeBean GetTheme(int index)
    {
        if (_themeBeans.IsNullOrEmpty())
        {
            Log.W(this, "theme is null or empry!");
            return null;
        }

        if (index >= 0 && index < _themeBeans.Count)
            return _themeBeans[index];

        Log.W(this, "GetTheme index out of bound");
        return null;
    }

    public ThemeBean GetTheme(EnvelopeBean envelopeBean)
    {
        return envelopeBean == null
                   ? null
                   : GetTheme(envelopeBean.themeId);
    }

    public ThemeBean GetTheme(LevelBean levelBean)
    {
        return levelBean == null
                   ? null
                   : GetTheme(levelBean.themeId);
    }

    public EnvelopeBean GetEnvelope(int themeId, int index)
    {
        ThemeBean themeBean = GetTheme(themeId);
        if (themeBean == null)
        {
            return null;
        }

        if (index >= 0 && index < themeBean.es.Length)
            return themeBean.es[index];

        Log.W(this, "GetEnvelope index out of bound");
        return null;
    }

    public EnvelopeBean GetEnvelope(ThemeBean themeBean, int index)
    {
        return themeBean == null
                   ? null
                   : GetEnvelope(themeBean.id, index);
    }

    public EnvelopeBean GetEnvelope(LevelBean levelBean)
    {
        return levelBean == null
                   ? null
                   : GetEnvelope(levelBean.themeId, levelBean.envelopeId);
    }

    public LevelBean GetLevel(int themeId, int envelopeId, int index)
    {
        EnvelopeBean envelopeBean = GetEnvelope(themeId, envelopeId);
        if (envelopeBean == null)
        {
            return null;
        }

        if (index >= 0 && index < envelopeBean.ls.Length)
            return envelopeBean.ls[index];

        Log.W(this, "GetLevel index out of bound");
        return null;
    }

    public LevelBean GetLevel(ThemeBean themeBean, int envelopIndex, int index)
    {
        return themeBean == null
                   ? null
                   : GetLevel(themeBean.id, envelopIndex, index);
    }

    public LevelBean GetLevel(EnvelopeBean envelopeBean, int index)
    {
        return envelopeBean == null
                   ? null
                   : GetLevel(envelopeBean.themeId, envelopeBean.idInTheme, index);
    }

    #endregion

    #region status

    public LevelStatus GetThemeStatus(int themeIndex)
    {
        if (themeIndex == PlayerDataManager.levelInfo.unlockThemeIndex)
            return LevelStatus.Current;
        return themeIndex < PlayerDataManager.levelInfo.unlockThemeIndex
                   ? LevelStatus.Unlock
                   : LevelStatus.Lock;
    }

    public LevelStatus GetThemeStatus(ThemeBean themeBean)
    {
        return themeBean == null
                   ? LevelStatus.Lock
                   : GetThemeStatus(themeBean.id);
    }

    public LevelStatus GetEnvelopStatus(int themeIndex, int envelopIndex)
    {
        LevelStatus themeStatus = GetThemeStatus(themeIndex);
        if (themeStatus != LevelStatus.Current)
            return themeStatus;

        if (envelopIndex == PlayerDataManager.levelInfo.unlockEnvelopeIndex)
            return LevelStatus.Current;
        return envelopIndex < PlayerDataManager.levelInfo.unlockEnvelopeIndex
                   ? LevelStatus.Unlock
                   : LevelStatus.Lock;
    }

    public LevelStatus GetEnvelopStatus(EnvelopeBean envelopeBean)
    {
        return envelopeBean == null
                   ? LevelStatus.Lock
                   : GetEnvelopStatus(envelopeBean.themeId, envelopeBean.idInTheme);
    }

    public LevelStatus GetLevelStatus(int themeIndex, int envelopIndex, int levelIndex)
    {
        LevelStatus envelopStatus = GetEnvelopStatus(themeIndex, envelopIndex);
        if (envelopStatus != LevelStatus.Current)
            return envelopStatus;

        if (levelIndex == PlayerDataManager.levelInfo.unlockLevelIndex)
            return LevelStatus.Current;
        return levelIndex < PlayerDataManager.levelInfo.unlockLevelIndex
                   ? LevelStatus.Unlock
                   : LevelStatus.Lock;
    }

    public LevelStatus GetLevelStatus(LevelBean levelBean)
    {
        return levelBean == null
                   ? LevelStatus.Lock
                   : GetLevelStatus(levelBean.themeId, levelBean.envelopeId, levelBean.idInEnvelope);
    }

    public LevelStatus GetStatus(int themeIndex, int envelopIndex = -1, int levelIndex = -1)
    {
        if (levelIndex != -1)
            return GetLevelStatus(themeIndex, envelopIndex, levelIndex);

        return envelopIndex != -1 ? GetEnvelopStatus(themeIndex, envelopIndex) : GetThemeStatus(themeIndex);
    }

    #endregion

    #region is latest

    public bool IsLatestTheme(int themeIndex)
    {
        return themeIndex == PlayerDataManager.levelInfo.unlockThemeIndex;
    }

    public bool IsLatestTheme(ThemeBean themeBean)
    {
        return themeBean != null
               && IsLatestTheme(themeBean.id);
    }

    public bool IsLatestEnvelop(int themeIndex, int envelopIndex)
    {
        return IsLatestTheme(themeIndex)
               && envelopIndex == PlayerDataManager.levelInfo.unlockEnvelopeIndex;
    }

    public bool IsLatestEnvelop(EnvelopeBean envelopeBean)
    {
        return envelopeBean != null
               && IsLatestEnvelop(envelopeBean.themeId, envelopeBean.idInTheme);
    }

    public bool IsLatestLevel(int themeIndex, int envelopIndex, int levelIndex)
    {
        return IsLatestEnvelop(themeIndex, envelopIndex)
               && levelIndex == PlayerDataManager.levelInfo.unlockLevelIndex;
    }

    public bool IsLatestLevel(LevelBean levelBean)
    {
        return levelBean != null
               && IsLatestLevel(levelBean.themeId,
                                levelBean.envelopeId,
                                levelBean.idInEnvelope);
    }

    public bool IsLatest(int themeIndex, int envelopIndex = -1, int levelIndex = -1)
    {
        if (levelIndex != -1)
            return IsLatestLevel(themeIndex, envelopIndex, levelIndex);

        return envelopIndex != -1 ? IsLatestEnvelop(themeIndex, envelopIndex) : IsLatestTheme(themeIndex);
    }

    #endregion

    #region is first

    public bool IsFirstTheme(int themeIndex)
    {
        return themeIndex == 0;
    }

    public bool IsFirstTheme(ThemeBean themeBean)
    {
        return themeBean != null && IsFirstTheme(themeBean.id);
    }

    public bool IsFirstEnvelop(int envelopIndex)
    {
        return envelopIndex == 0;
    }

    public bool IsFirstEnvelop(EnvelopeBean envelopeBean)
    {
        return envelopeBean != null && IsFirstEnvelop(envelopeBean.id);
    }

    public bool IsFirstEnvelopInTheme(int envelopIndexInTheme)
    {
        return envelopIndexInTheme == 0;
    }

    public bool IsFirstEnvelopInTheme(EnvelopeBean envelopeBean)
    {
        return envelopeBean != null && IsFirstEnvelopInTheme(envelopeBean.idInTheme);
    }

    public bool IsFirstLevel(int levelIndex)
    {
        return levelIndex == 0;
    }

    public bool IsFirstLevel(LevelBean levelBean)
    {
        return levelBean != null && IsFirstLevel(levelBean.id);
    }

    public bool IsFirstLevelInEnvelop(int levelIndexInEnvelop)
    {
        return levelIndexInEnvelop == 0;
    }

    public bool IsFirstLevelInEnvelop(LevelBean levelBean)
    {
        return levelBean != null && IsFirstLevelInEnvelop(levelBean.idInEnvelope);
    }

    public bool IsFirstLevelInTheme(int levelIndexInTheme)
    {
        return levelIndexInTheme == 0;
    }

    public bool IsFirstLevelInTheme(LevelBean levelBean)
    {
        return levelBean != null && IsFirstLevelInTheme(levelBean.idInTheme);
    }

    #endregion

    #region is last

    public bool IsLastTheme(int themeIndex)
    {
        return themeIndex == themeCount - 1;
    }

    public bool IsLastTheme(ThemeBean themeBean)
    {
        return themeBean != null && IsLastTheme(themeBean.id);
    }

    public bool IsLastEnvelop(int envelopIndex)
    {
        return envelopIndex == allEnvelopCount - 1;
    }

    public bool IsLastEnvelop(EnvelopeBean envelopeBean)
    {
        return envelopeBean != null && IsLastEnvelop(envelopeBean.id);
    }

    public bool IsLastEnvelopInTheme(int themeIndex, int envelopIndexInTheme)
    {
        ThemeBean themeBean = GetTheme(themeIndex);
        return themeBean != null && envelopIndexInTheme == themeBean.envelopeCount - 1;
    }

    public bool IsLastEnvelopInTheme(EnvelopeBean envelopeBean)
    {
        return envelopeBean != null
               && IsLastEnvelopInTheme(envelopeBean.themeId, envelopeBean.idInTheme);
    }

    public bool IsLastLevel(int levelIndex)
    {
        return levelIndex == allLevelCount - 1;
    }

    public bool IsLastLevel(LevelBean levelBean)
    {
        return levelBean != null && IsLastLevel(levelBean.id);
    }

    public bool IsLastLevelInEnvelop(int themeIndex, int envelopIndex, int levelIndexInEnvelop)
    {
        EnvelopeBean envelopeBean = GetEnvelope(themeIndex, envelopIndex);
        return envelopeBean != null && levelIndexInEnvelop == envelopeBean.levelCount - 1;
    }

    public bool IsLastLevelInEnvelop(LevelBean levelBean)
    {
        return levelBean != null
               && IsLastLevelInEnvelop(levelBean.themeId,
                                       levelBean.envelopeId,
                                       levelBean.idInEnvelope);
    }

    public bool IsLastLevelInTheme(int themeIndex, int levelIndexInTheme)
    {
        ThemeBean themeBean = GetTheme(themeIndex);
        return themeBean != null && levelIndexInTheme == themeBean.levelCount - 1;
    }

    public bool IsLastLevelInTheme(LevelBean levelBean)
    {
        return levelBean != null && IsLastLevelInTheme(levelBean.themeId, levelBean.idInTheme);
    }

    #endregion

    #region LevelPass

    public void LevelPass(LevelBean levelBean)
    {
        ThemeBean themeBean = GetTheme(levelBean.themeId);
        EnvelopeBean envelopeBean = GetEnvelope(themeBean.id, levelBean.envelopeId);

        if (++PlayerDataManager.levelInfo.currentLevelIndex == envelopeBean.levelCount)
        {
            PlayerDataManager.levelInfo.currentLevelIndex = 0;

            if (++PlayerDataManager.levelInfo.currentEnvelopeIndex == themeBean.envelopeCount)
            {
                PlayerDataManager.levelInfo.currentEnvelopeIndex = 0;
                ++PlayerDataManager.levelInfo.currentThemeIndex;
            }
        }

        if (!IsLatestLevel(levelBean)) return;
        PlayerDataManager.levelInfo.unlockThemeIndex = PlayerDataManager.levelInfo.currentThemeIndex;
        PlayerDataManager.levelInfo.unlockEnvelopeIndex = PlayerDataManager.levelInfo.currentEnvelopeIndex;
        PlayerDataManager.levelInfo.unlockLevelIndex = PlayerDataManager.levelInfo.currentLevelIndex;
    }

    #endregion
}