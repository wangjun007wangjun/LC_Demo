using BaseFramework;

public class LetterPanelEvent : Singleton<LetterPanelEvent>
{
    public ELetterPanelEventType type;
    public string word;
    public float delay;
    public char letter;

    public LetterPanelEvent Set(ELetterPanelEventType type)
    {
        this.type = type;
        return this;
    }

    public LetterPanelEvent SetDelay(float delay)
    {
        this.type = ELetterPanelEventType.ClearDelay;
        this.delay = delay;
        return this;
    }

    public LetterPanelEvent SetLetter(char letter)
    {
        this.type = ELetterPanelEventType.AddLetter;
        this.letter = letter;
        return this;
    }

    public LetterPanelEvent SetWord(string word)
    {
        this.type = ELetterPanelEventType.Seleceted;
        this.word = word;
        return this;
    }
}

public enum ELetterPanelEventType
{
    RemoveLast,
    ClearDelay,
    ClearImmediately,
    AddLetter,
    Seleceted
}