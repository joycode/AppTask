using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppTask
{
    /// <summary>
    /// 程序中所有可以并发执行的任务, 都通过该接口纳入管理
    /// </summary>
    public interface IAppTask
    {
        /// <summary>
        /// 开始运行任务
        /// </summary>
        void Run();
    } 
}
