using System.Collections.Generic;
using BaseFramework;
using UnityEngine;

public class LevelInfo : Singleton<LevelInfo>
{
    private const string UNLOCK_THEME_INDEX = "unlock_theme_index";
    private const string UNLOCK_ENVELOPE_INDEX = "unlock_envelope_index";
    private const string UNLOCK_LEVEL_INDEX = "unlock_level_index";
    private const string UNLOCK_ENVELOPE_ID = "unlock_envelope_id";

    private const string LEVEL_BOX_PROGRESS = "level_box_progress";

    private int _unlockThemeIndex = PlayerPrefs.GetInt(UNLOCK_THEME_INDEX);
    public int unlockThemeIndex
    {
        get => _unlockThemeIndex;
        set
        {
            _unlockThemeIndex = value;

            PlayerPrefs.SetInt(UNLOCK_THEME_INDEX, value);
        }
    }

    private int _unlockEnvelopeIndex = PlayerPrefs.GetInt(UNLOCK_ENVELOPE_INDEX);

    public int unlockEnvelopeIndex
    {
        get => _unlockEnvelopeIndex;
        set
        {
            _unlockEnvelopeIndex = value;

            PlayerPrefs.SetInt(UNLOCK_ENVELOPE_INDEX, value);
        }
    }
    
    private int _unlockEnvelopeId = PlayerPrefs.GetInt(UNLOCK_ENVELOPE_ID, -1);

    public int unlockEnvelopeId
    {
        get => _unlockEnvelopeId;
        set
        {
            _unlockEnvelopeId = value;

            PlayerPrefs.SetInt(UNLOCK_ENVELOPE_ID, value);
        }
    }

    private int _unlockLevelLevelIndex = PlayerPrefs.GetInt(UNLOCK_LEVEL_INDEX);

    public int unlockLevelIndex
    {
        get => _unlockLevelLevelIndex;
        set
        {
            _unlockLevelLevelIndex = value;

            PlayerPrefs.SetInt(UNLOCK_LEVEL_INDEX, value);
        }
    }

    private int _currentThemeIndex = -1;

    public int currentThemeIndex
    {
        get
        {
            if (_currentThemeIndex == -1)
            {
                _currentThemeIndex = _unlockThemeIndex;
            }

            return _currentThemeIndex;
        }
        set => _currentThemeIndex = value;
    }

    private int _currentEnvelopeIndex = -1;

    public int currentEnvelopeIndex
    {
        get
        {
            if (_currentEnvelopeIndex == -1)
            {
                _currentEnvelopeIndex = _unlockEnvelopeIndex;
            }

            return _currentEnvelopeIndex;
        }
        set => _currentEnvelopeIndex = value;
    }

    private int _currentLevelIndex = -1;

    public int currentLevelIndex
    {
        get
        {
            if (_currentLevelIndex == -1)
            {
                _currentLevelIndex = _unlockLevelLevelIndex;
            }

            return _currentLevelIndex;
        }
        set => _currentLevelIndex = value;
    }

    public void SaveLevelBoxProgress(List<string> data)
    {
        PlayerPrefsUtil.SetStrings(LEVEL_BOX_PROGRESS, data);
    }

    public List<string> GetLevelBoxProgress()
    {
        return PlayerPrefsUtil.GetStrings(LEVEL_BOX_PROGRESS);
    }

    public bool IsLevelBoxProgressEmpty()
    {
        List<string> progress = GetLevelBoxProgress();
        if (progress.IsNullOrEmpty()) return true;
        foreach (string s in progress)
        {
            if (s.Contains("1"))
                return false;
        }

        return true;
    }
}