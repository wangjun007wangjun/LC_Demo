using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BaseFramework
{
    public class CoroutineTaskExecutor : MonoSingleton<CoroutineTaskExecutor>
    {

    }

    public class CoroutineTaskManager : Singleton<CoroutineTaskManager>
    {
        private List<CoroutineTask> _tasks;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            _tasks = new List<CoroutineTask>();

            // 注册场景切换回调
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public override void OnSingletonDestroy()
        {
            base.OnSingletonDestroy();
            _tasks.ForEach(task => {
                task.Dispose();
            });
            _tasks = null;

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public CoroutineTask Execute(CoroutineTask task)
        {
            if(task.GetMonoBehaviour() == null)
            {
                task.SetMonoBehaviour(CoroutineTaskExecutor.instance);
            }
            _tasks.Add(task);

            task.ExecuteInternal();
            return task;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            CheckAndRecycleTask();
        }

        // 检查Task是否可用，不可用就回收
        public void CheckAndRecycleTask()
        {
            List<CoroutineTask> recycleTasks = new List<CoroutineTask>();
            _tasks.ForEach(it => {
                if(CheckTask(it))
                {
                    recycleTasks.Add(it);
                }
            });
            recycleTasks.ForEach(it => RecycleTask(it));

            recycleTasks.Clear();
            recycleTasks = null;
        }

        public void CheckAndRecycleTask(CoroutineTask task)
        {
            if (CheckTask(task))
            {
                RecycleTask(task);
            }
        }

        private bool CheckTask(CoroutineTask task)
        {
            return task != null && task.ShouldRecycle() && !task.isRecycled;
        }

        private void RecycleTask(CoroutineTask task)
        {
            Log.I(this, "Recycle coroutine task [{0}]", task.name);
            task.Dispose();

            _tasks.Remove(task);
        }
    }
}
