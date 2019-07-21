using System.Collections.Generic;
using BaseFramework;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class LevelListPanel : MonoBehaviour
{
    public ItemPrefabConfData[] itemPrefabConfDatas;
    public float minItemScale = 1f;
    public float maxItemScale = 1.4f;
    public float maxScaleDistance = 100;
    private ThemeBean _themeBean;
    private LoopListView2 _loopListView2;
    private List<ALevelItemViewWrapBean> _levelItemViewBeans;
    private int _initSelectedIndex;
    private float _totalSize;
    private ALevelBaseItemView _selectedItemView;

    private ALevelBaseItemView selectedItemView
    {
        get => _selectedItemView;
        set
        {
            if (_selectedItemView != null)
            {
                _selectedItemView.isSelected = false;
            }

            _selectedItemView = value;
            if (_selectedItemView != null)
            {
                _selectedItemView.isSelected = true;
            }
        }
    }

    private bool _initFinished;

    public void Init(ThemeBean themeBean)
    {
        if (_initFinished) return;

        _themeBean = themeBean;

        if (_themeBean == null) return;

        _initFinished = true;

        InitData();
        InitList();
        RegistEvent();
    }

    private void RegistEvent()
    {
        EventManager.instance.RegistEvent<LevelBaseItemClickEvent>(OnItemClick);
        EventManager.instance.RegistEvent<CardCloseEvent>(OnCardClose);
    }

    private void OnDestroy()
    {
        UnRegistEvent();
    }

    private void UnRegistEvent()
    {
        if (!_initFinished) return;

        EventManager.instance.UnRegistEvent<LevelBaseItemClickEvent>();
        EventManager.instance.UnRegistEvent<CardCloseEvent>();
    }

    private void InitData()
    {
        _levelItemViewBeans = new List<ALevelItemViewWrapBean>();

        bool isFirstTheme = LevelManager.instance.IsFirstTheme(_themeBean);
        bool isLastTheme = LevelManager.instance.IsLastTheme(_themeBean);

        float transmitSize = itemPrefabConfDatas[1].mItemPrefab.GetComponent<RectTransform>().rect.width;
        float envelopSize = itemPrefabConfDatas[2].mItemPrefab.GetComponent<RectTransform>().rect.width;
        float levelSize = itemPrefabConfDatas[3].mItemPrefab.GetComponent<RectTransform>().rect.width;

        int index = 0;
        _levelItemViewBeans.Add(new LevelEmptyWrapBean(LevelItemType.Empty,
                                                       itemPrefabConfDatas[0].mItemPrefab,
                                                       index++));
        if (!isFirstTheme)
        {
            _levelItemViewBeans.Add(new LevelTransmitWrapBean(LevelItemType.Transmit,
                                                              itemPrefabConfDatas[1].mItemPrefab,
                                                              index++,
                                                              transmitSize,
                                                              _themeBean.id - 1));
            _totalSize += transmitSize + itemPrefabConfDatas[1].mPadding;
        }

        _themeBean.es.ForEach((envelopIndexInTheme, envelop) =>
                              {
                                  LevelContentWrapBean envelopWrapBean =
                                      new LevelContentWrapBean(LevelItemType.Envelope,
                                                               itemPrefabConfDatas[2].mItemPrefab,
                                                               index++,
                                                               envelopSize,
                                                               envelop);
                                  _levelItemViewBeans.Add(envelopWrapBean);

                                  _totalSize += envelopSize + itemPrefabConfDatas[2].mPadding;;

                                  envelop.ls.ForEach((levelIndexInEnvelop, level) =>
                                                     {
                                                         LevelContentWrapBean levelWrapBean =
                                                             new LevelContentWrapBean(LevelItemType.Level,
                                                                                      itemPrefabConfDatas[3]
                                                                                         .mItemPrefab,
                                                                                      index++,
                                                                                      levelSize,
                                                                                      level);

                                                         _levelItemViewBeans.Add(levelWrapBean);

                                                         _totalSize += levelSize + itemPrefabConfDatas[3].mPadding;;
                                                     });
                              });
        if (!isLastTheme)
        {
            _levelItemViewBeans.Add(new LevelTransmitWrapBean(LevelItemType.Transmit,
                                                              itemPrefabConfDatas[1].mItemPrefab,
                                                              index++,
                                                              transmitSize,
                                                              _themeBean.id + 1));
            _totalSize += levelSize + itemPrefabConfDatas[1].mPadding;;
        }

        _levelItemViewBeans.Add(new LevelEmptyWrapBean(LevelItemType.Empty,
                                                       itemPrefabConfDatas[0].mItemPrefab,
                                                       index++));

        float firstSize = (ScreenSizeManager.instance.GetCanvasRealSize().x - _levelItemViewBeans[1].size) / 2;
        float lastPadding = isLastTheme
                                ? itemPrefabConfDatas[2].mPadding
                                : itemPrefabConfDatas[3].mPadding;
        float lastSize = (ScreenSizeManager.instance.GetCanvasRealSize().x -
                          _levelItemViewBeans[index - 2].size) / 2 - lastPadding;
        ((LevelEmptyWrapBean) _levelItemViewBeans[0]).size = firstSize;
        ((LevelEmptyWrapBean) _levelItemViewBeans[index - 1]).size = lastSize;

        _totalSize += firstSize + lastSize;

        _initSelectedIndex = isFirstTheme ? 1 : 2;
        if (_themeBean.id != PlayerDataManager.levelInfo.unlockThemeIndex) return;

        LevelBean levelBean = LevelManager.instance.GetLevel(_themeBean.id,
                                                             PlayerDataManager.levelInfo.unlockEnvelopeIndex,
                                                             PlayerDataManager.levelInfo.unlockLevelIndex);
        _initSelectedIndex += levelBean.idInTheme + levelBean.envelopeId;

        EnvelopeBean envelopeBean = LevelManager.instance.GetEnvelope(levelBean);
        if (envelopeBean.id == PlayerDataManager.levelInfo.unlockEnvelopeId)
        {
            _initSelectedIndex++;
        }
    }

    private void InitList()
    {
        _loopListView2 = GetComponent<LoopListView2>();

        itemPrefabConfDatas.ForEach(it => _loopListView2.mItemPrefabDataList.Add(it));

        LoopListViewInitParam initParam = LoopListViewInitParam.CopyDefaultInitParam();
        initParam.mSmoothDumpRate = 0.1f;
        initParam.mSnapFinishThreshold = 2f;
        _loopListView2.mOnBeginDragAction = OnBeginDragAction;
        _loopListView2.mOnSnapItemFinished = OnSnapItemFinished;

        _loopListView2.InitListView(_levelItemViewBeans.Count, OnGetItemByIndex, initParam);
        _loopListView2.SetContentSize(_totalSize);
        Scrollbar scrollbar = _loopListView2.ScrollRect.horizontalScrollbar;
        scrollbar.onValueChanged.AddListener((v) =>
                                             {
                                                 EventManager
                                                    .instance
                                                    .DispatchEvent(new
                                                                       LevelScrollChangeEvent(v));
                                             });

        this.Delay(0, () =>
                      {
                          _initFinished = true;

                          MoveToItemIndex(_initSelectedIndex);
                      });
    }

    private void LateUpdate()
    {
        if (!_initFinished) return;

        _loopListView2.UpdateAllShownItemSnapData();
        int count = _loopListView2.ShownItemCount;
        for (int i = 1; i < count - 1; ++i)
        {
            LoopListViewItem2 item = _loopListView2.GetShownItemByIndex(i);
            if (item == null) continue;

            LevelEnvelopeItemView levelEnvelopeItemView = item.GetComponent<LevelEnvelopeItemView>();
            if (levelEnvelopeItemView != null)
            {
                levelEnvelopeItemView.UpdateStatus();
                continue;
            }

            LevelItemView levelItemView = item.GetComponent<LevelItemView>();
            if (levelItemView == null) continue;

            levelItemView.UpdateStatus();
            if (levelItemView.levelStatus == LevelStatus.Lock) continue;

            Transform contentTransForm = levelItemView.contentTransform;
            if (contentTransForm == null) continue;

            float centerDistance = Mathf.Abs(item.DistanceWithViewPortSnapCenter);
            float scale = minItemScale +
                          (maxItemScale - minItemScale) * (maxScaleDistance - centerDistance) / maxScaleDistance;
            scale = Mathf.Clamp(scale, minItemScale, maxItemScale);

            contentTransForm.localScale = new Vector3(scale, scale, 1);
        }
    }

    private void OnSnapItemFinished(LoopListView2 loopListView2, LoopListViewItem2 selectedItemView)
    {
        if (_initFinished)
        {
            this.selectedItemView = selectedItemView.GetComponent<ALevelBaseItemView>();
        }
    }

    private void OnBeginDragAction()
    {
        selectedItemView = null;
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 loopListView2, int index)
    {
        LoopListViewItem2 viewItem = loopListView2.NewListViewItem(_levelItemViewBeans[index].prefabName);
        viewItem.name = index.ToString();
        InitView(viewItem, index);

        return viewItem;
    }

    private void InitView(LoopListViewItem2 viewItem, int index)
    {
        ALevelBaseItemView levelBaseItemView = viewItem.GetComponent<ALevelBaseItemView>();
        ALevelItemViewWrapBean wrapBean = _levelItemViewBeans[index];
        levelBaseItemView?.Init(wrapBean);
    }

    private void MoveToItemIndex(int index)
    {
        float itemSize = _levelItemViewBeans[index].size;
        float offset = (((RectTransform) transform).rect.width - itemSize) / 2;
        _loopListView2.MovePanelToItemIndex(index, offset);
    }

    private void ScrollToItem(int index)
    {
        _loopListView2.SetSnapTargetItemIndex(index);
    }

    private void OnItemClick(LevelBaseItemClickEvent @event)
    {
        ScrollToItem(@event.index);
    }

    private void OnCardClose(CardCloseEvent @event)
    {
        ScrollToItem(@event.itemView.bean.index + 1);
    }
}