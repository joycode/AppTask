using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace AppTask
{
    /// <summary>
    /// 在此处对系统线程做集中管理
    /// </summary>
    public class AppTaskExecutor
    {
        protected static AppTaskExecutor _instance = new AppTaskExecutor();

        protected object Sync_Locker = new object();
        Dictionary<IAppTask, IAppTaskAsyncResult> _tasks = new Dictionary<IAppTask, IAppTaskAsyncResult>();

        public static AppTaskExecutor Singleton
        {
            get { return _instance; }
        }

        protected AppTaskExecutor()
        {
        }

        public IAppTaskAsyncResult Execute(IAppTask task)
        {
            Debug.Assert(task != null);

            Thread th = new Thread(task.Run);
            th.IsBackground = true;
            th.Start();

            IAppTaskAsyncResult async_result = new AbstractAppTask.AppTaskAsyncResult(task);
            lock (Sync_Locker) {
                _tasks.Add(task, async_result);
            }

            return async_result;
        }
        public void Stop(IAppTask task)
        {
            Debug.Assert(task != null);

            IAppTaskAsyncResult async_result = null;
            lock (Sync_Locker) {
                if (_tasks.ContainsKey(task)) {
                    async_result = _tasks[task];
                    _tasks.Remove(task);
                }
            }

            if (async_result != null) {
                async_result.Interrupt();
                async_result.Join();
            }
        }

        public void StopAll()
        {
            lock (Sync_Locker) {
                foreach (IAppTaskAsyncResult async in _tasks.Values) {
                    async.Interrupt();
                }
                foreach (IAppTaskAsyncResult async in _tasks.Values) {
                    async.Join();
                }

                _tasks.Clear();
            }
        }
    }
}
