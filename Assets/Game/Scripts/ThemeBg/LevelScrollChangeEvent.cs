public class LevelScrollChangeEvent
{
    public float item;
    public bool levelPass = false;
    public LevelScrollChangeEvent(float item,bool levelPass = false)
    {
        this.item = item;
        this.levelPass = levelPass;
    }
}