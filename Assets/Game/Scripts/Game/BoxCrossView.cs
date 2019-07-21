using BaseFramework;
using UnityEngine;

public class BoxCrossView : MonoRecycleItem<BoxCrossView>
{
    public static BoxCrossView Instance(BoxCrossView prefab,
                                        Transform parentTransform)
    {
        BoxCrossView boxCrossView = PoolHelper.Create(prefab);
        boxCrossView.transform.SetParent(parentTransform, false);

        return boxCrossView;
    }

    public void Collect()
    {
        Dispose();
    }
}