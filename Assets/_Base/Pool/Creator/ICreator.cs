namespace BaseFramework
{
    public interface ICreator<out T>
    {
        T Create();
    }
}