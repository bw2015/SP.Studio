using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SP.StudioCore.Tasks.Scheduler
{
    /// <summary>
    /// 任务执行的日志
    /// </summary>
    public struct TaskResult
    {
        /// <summary>
        /// 计时器
        /// </summary>
        public Stopwatch sw { get; set; }

        private ITaskScheduler taskScheduler;

        public TaskResult(ITaskScheduler taskScheduler)
        {
            this.sw = Stopwatch.StartNew();
            this.taskScheduler = taskScheduler;
            this.logs = new List<string>();
        }

        private List<string> logs;

        public int Length => this.logs?.Count ?? 0;

        public string Name => this.taskScheduler.GetType().Name;

        public void Add(string log)
        {
            this.logs ??= new List<string>();
            this.logs.Add(log);
        }

        public void Clear()
        {
            this.logs?.Clear();
        }

        public static implicit operator string(TaskResult taskResult)
        {
            return taskResult.ToString();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.logs);
        }
    }
}
