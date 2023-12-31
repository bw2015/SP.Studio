﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SP.StudioCore.Tasks.Scheduler.Attributes
{
    /// <summary>
    /// 任务的特性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskSchedulerAttribute : Attribute
    {
        public TaskSchedulerAttribute(string? crontab = null, bool isThread = false)
        {
            Crontab = crontab ?? string.Empty;
            IsThread = isThread;
        }

        /// <summary>
        /// 定时器的格式
        /// </summary>
        public Crontab Crontab { get; set; }

        /// <summary>
        /// 是否允许多线程执行
        /// </summary>
        public bool IsThread { get; set; }
    }
}
