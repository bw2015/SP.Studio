using SP.StudioCore.Ioc;
using SP.StudioCore.Tasks.Scheduler.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SP.StudioCore.Tasks.Scheduler
{
    /// <summary>
    /// 调度工厂
    /// </summary>
    public static class SchedulerFactory
    {
        private static ITaskSchedulerHandler? handler = IocCollection.GetService<ITaskSchedulerHandler>();

        public static async Task RunAsync(Assembly assembly, params string[] args)
        {
            Console.WriteLine(assembly.FullName);
            // 定时器任务
            List<ITaskScheduler> timerTasks = new List<ITaskScheduler>();
            // 多线程任务
            List<ITaskScheduler> threadTasks = new List<ITaskScheduler>();

            foreach (Type type in assembly.GetTypes().Where(t => !t.IsAbstract))
            {
                if (typeof(ITaskScheduler).IsAssignableFrom(type))
                {
                    ITaskScheduler task = (ITaskScheduler)Activator.CreateInstance(type, new[] { args });
                    TaskSchedulerAttribute scheduler = type.GetCustomAttribute<TaskSchedulerAttribute>();
                    if (scheduler.Crontab)
                    {
                        timerTasks.Add(task);
                    }
                    else if (scheduler.IsThread)
                    {
                        for (int i = 0; i < task.ThreadCount; i++)
                        {
                            threadTasks.Add(task);
                        }
                    }
                }
            }

            // 定时器任务
            Timer timer = new Timer(1000);
            timer.Elapsed += async (object sender, ElapsedEventArgs e) =>
            {
                foreach (ITaskScheduler task in timerTasks)
                {
                    TaskSchedulerAttribute scheduler = task.GetType().GetCustomAttribute<TaskSchedulerAttribute>();
                    if (scheduler.Crontab.IsTime(DateTime.Now))
                    {
                        if (task.Running) continue;
                        try
                        {
                            task.Running = true;

                            TaskResult result = await task.ExecuteAsync();

                            // 写入服务日志
                            handler?.SaveLog(result);

                            // 显示
                            handler?.Print(result);
                        }
                        catch (Exception ex)
                        {
                            // 处理错误日志
                            handler?.Exception(ex);
                        }
                        finally
                        {
                            task.Running = false;
                        }
                    }
                }
            };
            timer.Start();

            // 需要多线程循环执行的任务
            Parallel.ForEach(threadTasks, async task =>
            {
                while (true)
                {
                    try
                    {
                        TaskResult result = await task.ExecuteAsync();
                        // 写入服务日志
                        handler?.SaveLog(result);

                        // 显示
                        handler?.Print(result);
                    }
                    catch (Exception ex)
                    {
                        // 处理错误日志
                        handler?.Exception(ex);
                    }
                    finally
                    {
                        await Task.Delay(task.TheadDelay);
                    }
                }
            });


            await Task.Delay(-1);
        }
    }
}
