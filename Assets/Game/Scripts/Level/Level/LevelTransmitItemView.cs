using BaseFramework;

public class LevelTransmitItemView : ALevelBaseItemView
{
    public LevelTransmitWrapBean bean => _bean as LevelTransmitWrapBean;

    protected override void OnClick()
    {
        if (!isSelected)
        {
            base.OnClick();
            return;
        }

        PlayerDataManager.levelInfo.currentThemeIndex = bean.transmitId;

        SceneHelper.instance.LoadScene(Const.Scene.LEVEL,
                                       0,
                                       BlindTransition.instance.Enter,
                                       BlindTransition.instance.Exit);
    }
}