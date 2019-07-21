using System.Collections.Generic;
using BaseFramework;
using Newtonsoft.Json;
using SuperScrollView;
using UnityEngine;

public class ThemeListPanel : MonoBehaviour
{
    public ItemPrefabConfData itemPrefabConfData;

    private List<ThemeBean> _themeBeans;
    private LoopListView2 _loopListView2;

    private void Awake()
    {
        InitData();
        InitList();
    }

    private void InitData()
    {
        _themeBeans = LevelManager.instance.GetThemes();
    }

    private void InitList()
    {
        _loopListView2 = GetComponent<LoopListView2>();
        _loopListView2.mItemPrefabDataList.Add(itemPrefabConfData);
        _loopListView2.InitListView(_themeBeans.Count, OnGetItemByIndex);
        
        this.Delay(0, () => MoveToItem(PlayerDataManager.levelInfo.unlockThemeIndex));
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 loopListView2, int index)
    {
        LoopListViewItem2 viewItem = loopListView2.NewListViewItem(itemPrefabConfData.mItemPrefab.name);

        ThemeItemPanel itemPanel = viewItem.GetComponent<ThemeItemPanel>();
        itemPanel.Init(_themeBeans[index]);
        
        return viewItem;
    }
    
    private void MoveToItem(int index)
    {
        float offset = (((RectTransform) transform).rect.height -
                        itemPrefabConfData.mItemPrefab.GetComponent<RectTransform>().rect.height) / 2;
        _loopListView2.MovePanelToItemIndex(index, offset);
    }
}