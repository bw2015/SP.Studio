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

        public string[] args { get; set; }

        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// 线程执行的等待时间
        /// </summary>
        public int TheadDelay { get; set; }

        /// <summary>
        /// 执行
        /// </summary>
        public Task<TaskResult> ExecuteAsync();
    }
}
