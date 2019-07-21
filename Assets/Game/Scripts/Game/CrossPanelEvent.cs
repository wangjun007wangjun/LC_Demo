using BaseFramework;

public class CrossPanelEvent : Singleton<CrossPanelEvent>
{
    public EAnswerType type;
    public bool levelOnComplete = false;
    public CrossPanelEvent Set(EAnswerType type)
    {
        this.type = type;
        return this;
    }

    public CrossPanelEvent LevelOnComplete()
    {
        levelOnComplete = true;
        return this;
    }
}

public enum EAnswerType
{
    Answer,
    Bible,
    Bonus,
    Extra,
    Error,
    InvalidAnswer,
    InvalidBible,
    InvalidBonus,
    InvalidExtra,
    ExtraClose
}