using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BaseFramework
{
    public class MonoPoolManager : MonoSingleton<MonoPoolManager>
    {

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();

            instance.Inactive();
        }

        public static void Register(UnityAction<Scene> action)
        {
            SceneManager.sceneUnloaded += action;
        }

        public static void UnRegister(UnityAction<Scene> action)
        {
            SceneManager.sceneUnloaded -= action;
        }
    }
    
    public class MonoPool<T> : Pool<T>, ISingleton where T : class, IRecycleable
    {
        private GameObject _poolRootObj;

        #region singleton

        private static class SingletonHandler
        {
            /// <summary>
            /// 当一个类有静态构造函数时，它的静态成员变量不会被beforefieldinit修饰
            /// 就会确保在被引用的时候才会实例化，而不是程序启动的时候实例化
            /// </summary>
            static SingletonHandler()
            {
                Init();
            }

            private static void Init()
            {
                instance = new MonoPool<T>();
                instance.OnSingletonInit();
            }

            public static MonoPool<T> instance;
        }

        public static MonoPool<T> instance => SingletonHandler.instance;

        public virtual void OnSingletonInit()
        {
            _poolRootObj = new GameObject();
            _poolRootObj
                .Inactive()
                .Name($"{GetType().ReadableName()}_Singleton")
                .transform.SetParent(MonoPoolManager.instance.transform, false);
        }

        public virtual void OnSingletonDestroy()
        {
            _poolRootObj.Destroy();
            _poolRootObj = null;
            SingletonHandler.instance = null;
        }
        #endregion       

        protected override void OnItemRecycle(T item)
        {
            base.OnItemRecycle(item);

            (item as MonoBehaviour)?.transform
                                   .Inactive()
                                   .Parent(_poolRootObj.transform, false);
        }

        protected override bool CheckUseful(T t)
        {
            return t as MonoBehaviour != null;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSingletonDestroy();
        }
    }
}
