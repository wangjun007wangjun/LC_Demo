using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseFramework
{
    public class SceneHelper : Singleton<SceneHelper>
    {
        public event Action<string, string> beforeSceneChange;
        public event Action<string, string> afterSceneChange;

        private Dictionary<string, int> sceneCounter;
        private bool _isLoading = false;

        public string lastSceneName { get; private set; } = "";

        public string currentSceneName => SceneManager.GetActiveScene().name;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">目标场景</param>
        /// <param name="minTime">最小过度时间</param>
        /// <param name="startAction">开始事件</param>
        /// <param name="endAction">结束事件</param>
        /// <param name="wait">等待其他操作</param>
        public void LoadScene(string name,
                              float minTime,
                              Action startAction,
                              Action endAction,
                              Action<Action> wait = null)
        {
            if (_isLoading) return;
            lastSceneName = currentSceneName;

            float startTime = Time.realtimeSinceStartup;
            Log.I(this, "switch to {0} start time:{1}", name, startTime);

            if (wait != null)
            {
                wait(() => {
                         LoadScene(name, startTime, minTime, startAction, endAction);
                     });
            }
            else
            {
                LoadScene(name, startTime, minTime, startAction, endAction);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">目标场景</param>
        /// <param name="minTime">最小过度时间</param>
        /// <param name="startAction">开始事件</param>
        /// <param name="endAction">结束事件</param>
        /// <param name="wait">等待其他操作</param>
        public void LoadScene(string name,
                              float minTime,
                              Action startAction,
                              Action endAction,
                              IEnumerator wait)
        {
            if (_isLoading) return;
            lastSceneName = currentSceneName;

            float startTime = Time.realtimeSinceStartup;
            Log.I(this, "switch to {0} start time:{1}", name, startTime);
            
            LoadScene(name, startTime, minTime, startAction, endAction, wait);
        }

        private void LoadScene(string name,
                               float startTime,
                               float minTime,
                               Action startAction,
                               Action endAction,
                               IEnumerator wait = null)
        {
            beforeSceneChange?.Invoke(lastSceneName, name);

            TaskHelper.Create<CoroutineTask>()
                      .Do(startAction)
                      .Wait(wait)
                      .Wait(LoadSceneAsync(name, startTime, minTime))
                      .Do(endAction)
                      .Execute();
        }

        private IEnumerator LoadSceneAsync(string name,
                                   float startTime,
                                   float minTime)
        {
            _isLoading = true;

            AsyncOperation async = SceneManager.LoadSceneAsync(name);
            async.allowSceneActivation = false;
            while (async.progress < 0.9f)
            {
                yield return null;

            }
            while (Time.realtimeSinceStartup - startTime < minTime)
            {
                yield return null;
            }

            async.allowSceneActivation = true;

            Log.I(this, "switch to {0} end time:{1}", name, Time.realtimeSinceStartup);
            _isLoading = false;
            
            afterSceneChange?.Invoke(lastSceneName, name);
            AddCounter(name);
        }

        private void AddCounter(string name)
        {
            if (sceneCounter == null)
            {
                sceneCounter = new Dictionary<string, int>();
            }

            if (!sceneCounter.ContainsKey(lastSceneName))
            {
                sceneCounter.Add(lastSceneName, 1);
            }

            if (sceneCounter.ContainsKey(name))
            {
                sceneCounter[name]++;
            }
            else
            {
                sceneCounter.Add(name, 1);
            }
        }

        public int GetSceneCount(string name)
        {
            if (sceneCounter == null || !sceneCounter.ContainsKey(name))
                return 0;
            return sceneCounter[name];
        }
    }
}
