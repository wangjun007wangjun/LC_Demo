using System.Collections.Generic;
using BaseFramework;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : Singleton<SpriteManager>
{
    private const string AB_THEME_ICON_NAME = "sprite/theme/icon";
    private const string RES_THEME_ICON_NAME = "theme_icon";
    private const string AB_ENVELOPE_CARD_NAME = "sprite/envelope/card";
    private const string RES_ENVELOPE_CARD_NAME = "envelope_card";


    private SpriteAtlas _iconSpriteAtlas;
    private SpriteAtlas _cardSpriteAtlas;
    private Dictionary<string, Sprite> _spriteCaches;
    
    public void LoadThemeIcon(float progress)
    {   
        float startTime = Time.realtimeSinceStartup;
        Log.I(this, $"load small start at {startTime}");
        
        ResHelper.LoadAssetAsync<SpriteAtlas>(AB_THEME_ICON_NAME,
                                              RES_THEME_ICON_NAME,
                                             (it) =>
                                             {
                                                 float endTime = Time.realtimeSinceStartup;
                                                 Log.I(this, $"load small end at {endTime}, total use {endTime - startTime}");
                                                 
                                                 _iconSpriteAtlas = it;

                                                 EventManager.instance.DispatchEvent(new LoadingPregressEvent(progress));
                                             },
                                             true);
    }
    
    public void LoadEnvelopeCard(float progress)
    {
        float startTime = Time.realtimeSinceStartup;
        Log.I(this, $"load card start at {startTime}");
        
        ResHelper.LoadAssetAsync<SpriteAtlas>(AB_ENVELOPE_CARD_NAME,
                                              RES_ENVELOPE_CARD_NAME,
                                              (it) =>
                                              {
                                                  float endTime = Time.realtimeSinceStartup;
                                                  Log.I(this, $"load card end at {endTime}, total use {endTime - startTime}");
                                                  
                                                  _cardSpriteAtlas = it;

                                                  EventManager.instance.DispatchEvent(new LoadingPregressEvent(progress));
                                              },
                                              true);
    }

    public Sprite GetIconSprite(string name)
    {
        if (_spriteCaches != null 
            && _spriteCaches.ContainsKey(name)
            && _spriteCaches[name] != null)
        {
            return _spriteCaches[name];
        }

        if (_iconSpriteAtlas == null)
            return null;

        Sprite sprite = _iconSpriteAtlas.GetSprite(name);
        if (_spriteCaches == null)
        {
            _spriteCaches = new Dictionary<string, Sprite>();
        }
        _spriteCaches[name] = sprite;

        return sprite;
    }
    
    public Sprite GetCardSprite(string name)
    {
        if (_spriteCaches != null 
            && _spriteCaches.ContainsKey(name)
            && _spriteCaches[name] != null)
        {
            return _spriteCaches[name];
        }

        if (_cardSpriteAtlas == null)
            return null;

        Sprite sprite = _cardSpriteAtlas.GetSprite(name);
        if (_spriteCaches == null)
        {
            _spriteCaches = new Dictionary<string, Sprite>();
        }
        _spriteCaches[name] = sprite;

        return sprite;
    }
}