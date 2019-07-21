using System;
using System.Threading;

namespace BaseFramework
{
    public class RunableTask : Task<RunableTask>
    {
        protected Thread thread;
        protected float delayTime;

        public RunableTask Delay(float delay)
        {
            delayTime = delay;
            return this;
        }

        internal override RunableTask ExecuteInternal()
        {
            if (status != TaskStatus.Unexecuted)
                return this;

            Start();

            void ThreadCallback()
            {
                Finish();
            }

            thread = new Thread(Run) {IsBackground = true};
            thread.Start((Action) ThreadCallback);

            return this;
        }

        protected void Run(object obj)
        {
            if (delayTime > 0)
                Thread.Sleep((int)(delayTime * 1000));

            Do();
            Action callback = obj as Action;
            callback?.Invoke();
        }

        protected override void DoCancel()
        {
            thread.Abort();
            base.DoCancel();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            thread = null;
        }

        public override void Dispose()
        {
            this.Recycle();
        }
    }
}
