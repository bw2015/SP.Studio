using System;
using System.Collections.Generic;
using System.Text;

namespace SP.StudioCore.Tasks.Scheduler
{
    /// <summary>
    /// 业务逻辑实现类
    /// </summary>
    public interface ITaskSchedulerHandler
    {
        /// <summary>
        /// 在控制台打印
        /// </summary>
        /// <param name="result"></param>
        void Print(TaskResult result);

        void SaveLog(TaskResult result);

        /// <summary>
        /// 异常处理
        /// </summary>
        void Exception(Exception ex);
    }
}
