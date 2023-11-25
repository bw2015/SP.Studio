using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SP.StudioCore.Tasks.Scheduler
{
    public interface ITaskScheduler
    {
        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// 外部传入的参数
        /// </summary>
        public string[] args { get; }

        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadCount { get; }

        /// <summary>
        /// 线程执行的等待时间
        /// </summary>
        public int TheadDelay { get; }

        /// <summary>
        /// 执行
        /// </summary>
        public Task<TaskResult> ExecuteAsync();
    }
}
