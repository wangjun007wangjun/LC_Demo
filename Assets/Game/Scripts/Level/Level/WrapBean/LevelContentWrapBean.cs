using UnityEngine;

public class LevelContentWrapBean : ALevelItemViewWrapBean
{
    public System.Object bean { get; private set; }

    public LevelContentWrapBean(LevelItemType type,
                                GameObject prefab,
                                int index,
                                float size,
                                System.Object bean) : base(type, prefab, index, size)
    {
        this.bean = bean;
    }
}