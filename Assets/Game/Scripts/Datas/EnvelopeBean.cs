using System;
using BaseFramework;
using UnityEngine;

[Serializable]
public class EnvelopeBean
{
    private const int MAX_COUNT = 10;
    private const string SHAPE_NAME = "envelope_";
    private const string FEATURE_NAME = "envelope_feature_";
    private const string CARD_NAME = "card_";
    
    public string v;
    public LevelBean[] ls;
    
    public int id { get; private set; }
    public int idInTheme { get; private set; }
    
    public int themeId { get; private set; }
    
    public string featureName { get; private set; }

    public string shapeName { get; private set; }
    
    public string cardName { get; private set; }
    
    public Color color { get; private set; }

    public int levelCount => ls.Length;
    
    public void Init(int id, int idInTheme, int themeId, Color color)
    {
        this.id = id;
        this.color = color;

        this.themeId = themeId;
        this.idInTheme = idInTheme;

        shapeName = SHAPE_NAME + (id % MAX_COUNT + 1);
        ++id;
        featureName = FEATURE_NAME + id;
        cardName = CARD_NAME + id;
    }
}