using UnityEngine;

public class LevelEmptyWrapBean : ALevelItemViewWrapBean
{
    public LevelEmptyWrapBean(LevelItemType type,
                              GameObject prefab,
                              int index,
                              float size = 0) : base(type, prefab, index, size)
    {
    }
}