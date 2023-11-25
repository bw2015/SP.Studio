using Newtonsoft.Json;
using SP.StudioCore.Tasks.Scheduler.Utils;
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


        public TaskResult(ITaskScheduler taskScheduler) : this(taskScheduler.GetType().Name)
        {
        }

        public TaskResult(Type type) : this(type.Name)
        {

        }

        public TaskResult(string name)
        {
            this.Name = name;
            this.sw = Stopwatch.StartNew();
            this.logs = new List<string>();
            this.createAt = WebAgent.GetTimestamps();
        }

        /// <summary>
        /// 任务的开始时间
        /// </summary>
        public long createAt { get; private set; }

        private List<string> logs;

        public int Length => this.logs?.Count ?? 0;

        public string Name { get; private set; }


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

        public static implicit operator bool(TaskResult taskResult)
        {
            return taskResult.Length > 0;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.logs);
        }
    }
}
