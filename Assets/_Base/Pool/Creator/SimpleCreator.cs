using System;
using System.Linq.Expressions;

namespace BaseFramework
{
    public class SimpleCreator<T> : ICreator<T>
    {
        private readonly Func<T> _createFunc;

        public SimpleCreator()
        {
            // throw exception while call createFunc if no public Constructor 
            _createFunc = Activator.CreateInstance<T>;
        }

        public SimpleCreator(Func<T> func)
        {
            _createFunc = func;
        }

        public T Create()
        {
            return _createFunc();
        }
    }
}