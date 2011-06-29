using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppTask
{
    /// <summary>
    /// 管理任务并发执行的状态
    /// </summary>
    public interface IAppTaskAsyncResult
    {
        /// <summary>
        /// 等待任务的结束
        /// </summary>
        void Join();
        /// <summary>
        /// 手动中断任务
        /// </summary>
        void Interrupt();
    }
}
