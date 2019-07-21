using System;

[Serializable]
public class ThemeBean
{
    private const string ICON_NAME = "icon_theme_";
    private const string TRANSMIT_NAME = "transmit_";

    public string n;
    public string c;
    public EnvelopeBean[] es;

    public int id { get; private set; }
    public string iconName { get; private set; }
    public string transmitName { get; private set; }
    
    public int levelCount { get; private set; }

    public int envelopeCount => es.Length;
    
    public void Init(int index, int levelCount)
    {
        this.id = index;
        ++index;
        iconName = ICON_NAME + index;
        transmitName = TRANSMIT_NAME + index;

        this.levelCount = levelCount;
    }
}