using System;
using BaseFramework;
using UnityEngine;

[Serializable]
public abstract class ALevelItemViewWrapBean
{
    public LevelItemType type { get; private set; }

    public GameObject prefab { get; private set; }

    public string prefabName => prefab?.name;

    public float size { get; set; }
    
    public int index { get; private set; }

    public ALevelItemViewWrapBean(LevelItemType type,
                                  GameObject prefab,
                                  int index,
                                  float size)
    {
        this.type = type;
        this.prefab = prefab;
        this.index = index;
        this.size = size;
    }
}