using System;

namespace BaseFramework
{
    public interface ITask<out T> : IDisposable
    {
        T Execute();

        T Cancel();
    }
}