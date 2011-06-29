using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace AppTask
{
    /// <summary>
    /// 程序中可以并发执行的任务
    /// </summary>
    public abstract class AbstractAppTask : IAppTask
    {
        /// <summary>
        /// 管理任务并发执行的状态
        /// </summary>
        public class AppTaskAsyncResult : IAppTaskAsyncResult
        {
            IAppTask _task = null;

            public AppTaskAsyncResult(IAppTask task)
            {
                Debug.Assert(task != null);

                _task = task;
            }

            /// <summary>
            /// 终止任务的执行
            /// 不同任务的终止可能有不同的处理, 所以定义为 virtual
            /// </summary>
            public virtual void Interrupt()
            {
                AbstractAppTask task = _task as AbstractAppTask;
                Debug.Assert(task != null);

                lock (task.Sync_Locker) {
                    if (!task._executing) {
                        task._interrupted = true;
                        return;
                    }

                    if (task._interrupted) {
                        return;
                    }
                    task._interrupted = true;

                    if (task._thread == null) {
                        return;
                    }
                }

                task._thread.Interrupt();
            }

            public void Join()
            {
                AbstractAppTask task = _task as AbstractAppTask;
                Debug.Assert(task != null);

                lock (task.Sync_Locker) {
                    if (task._thread == null) {
                        return;
                    }
                }

                try {
                    if (task._thread == Thread.CurrentThread)
                    {
                        return;
                    }
                    else
                    {
                        task._thread.Join();
                    }
                }
                catch (ThreadInterruptedException) {
                }
            }
        }

        protected object Sync_Locker = new object();
        /// <summary>
        /// 任务开始执行标志
        /// </summary>
        protected bool _executing = false;
        /// <summary>
        /// 手动终止任务标志
        /// </summary>
        protected bool _interrupted = false;
        /// <summary>
        /// 负责执行任务的线程
        /// </summary>
        protected Thread _thread = null;

        protected AbstractAppTask()
        {
        }

        public void Run()
        {
            lock (Sync_Locker) {
                if (_interrupted) {
                    return;
                }

                _executing = true;
                _thread = Thread.CurrentThread;
            }

            this.Worker();
        } 

        /// <summary>
        /// 实际执行任务的函数
        /// </summary>
        protected virtual void Worker()
        {
            throw new NotImplementedException();
        }
    }
}
