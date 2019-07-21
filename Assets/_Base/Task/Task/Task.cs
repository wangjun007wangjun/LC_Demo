using System;

namespace BaseFramework
{
    public abstract class Task<T> : SimpleRecycleItem<T>, ITask<T> where T : class, IRecycleable
    {
        public string name;

        protected Action onStart = null;
        protected Action doAction = null;
        protected Action onFinish = null;
        protected Action onCancel = null;

        protected bool autoRecycle = true;

        protected TaskStatus status { get; set; }

        public T Name(string name)
        {
            this.name = name;

            return this as T;
        }

        public virtual T OnStart(params Action[] actions)
        {
            actions.ForEach(action => onStart += action);
            return this as T;
        }

        public virtual T Do(params Action[] actions)
        {
            actions.ForEach(action => doAction += action);
            return this as T;
        }

        public virtual T OnFinish(params Action[] actions)
        {
            actions.ForEach(action => onFinish += action);
            return this as T;
        }

        public virtual T OnCancel(params Action[] actions)
        {
            actions.ForEach(action => onCancel += action);
            return this as T;
        }

        public virtual T AutoRecycle(bool auto)
        {
            autoRecycle = auto;
            return this as T;
        }

        public virtual T Execute()
        {
            return ExecuteInternal();
        }

        internal abstract T ExecuteInternal();

        public T Cancel()
        {
            if (status == TaskStatus.Executing)
            {
                status = TaskStatus.Canceled;
                DoCancel();
            }
            else
            {
                Log.W(this,
                    "cancel failed! you can only cancel executing task, current status is {0}", status);
            }

            return this as T;
        }

        protected virtual void DoCancel()
        {
            onCancel?.Invoke();
            Dispose();
        }

        protected virtual void Start()
        {
            onStart?.Invoke();
            status = TaskStatus.Executing;
        }

        protected virtual void Do()
        {
            if (status == TaskStatus.Executing)
                doAction?.Invoke();
        }

        protected virtual void Finish()
        {
            if (status != TaskStatus.Executing) return;
            status = TaskStatus.Done;
            onFinish?.Invoke();

            if (autoRecycle)
                Dispose();
        }

        #region IRecycleable
        public override void OnReset()
        {
            status = TaskStatus.Unexecuted;
        }

        public override void OnRecycle()
        {
            onStart = null;
            doAction = null;
            onFinish = null;
            onCancel = null;
        }
        #endregion
    }
}
