using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SP.StudioCore.Tasks.Scheduler.Attributes
{

    /// <summary>
    /// 执行间隔调度
    /// 星号（*）：代表所有可能的值，例如month字段如果是星号，则表示在满足其它字段的制约条件后每月都执行该命令操作。
    /// 逗号（,）：可以用逗号隔开的值指定一个列表范围，例如，“1,2,5,7,8,9”
    /// 中杠（-）：可以用整数之间的中杠表示一个整数范围，例如“2-6”表示“2,3,4,5,6”
    /// 正斜线（/）：可以用正斜线指定时间的间隔频率，例如“0-23/2”表示每两小时执行一次。同时正斜线可以和星号一起使用，例如*/10，如果用在minute字段，表示每十分钟执行一次。
    /// </summary>
    public struct Crontab
    {
        public string second;

        public string minute;

        public string hour;

        public string day;

        public string month;

        public string week;

        public static implicit operator bool(Crontab crontab)
        {
            foreach (string value in new[] { crontab.second, crontab.minute, crontab.hour, crontab.day, crontab.month, crontab.week })
            {
                if (string.IsNullOrEmpty(value)) return false;
            }
            return true;
        }

        public static implicit operator Crontab(string value)
        {
            string[] args = value.Split(' ');

            if (args.Length == 5)
            {
                return new Crontab()
                {
                    second = "*",
                    minute = args[0],
                    hour = args[1],
                    day = args[2],
                    month = args[3],
                    week = args[4]
                };
            }

            if (args.Length == 6)
            {
                return new Crontab()
                {
                    second = args[0],
                    minute = args[1],
                    hour = args[2],
                    day = args[3],
                    month = args[4],
                    week = args[5]
                };
            }

            return default;
        }

        /// <summary>
        /// 是否到了执行的时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="key">执行的key</param>
        /// <returns></returns>
        public bool IsTime(DateTime time)
        {
            int second = time.Second;
            if (!new CrontabValue(this.second).IsMatch(second)) return false;

            int minute = time.Minute;
            if (!new CrontabValue(this.minute).IsMatch(minute)) return false;

            int hour = time.Hour;
            if (!new CrontabValue(this.hour).IsMatch(hour)) return false;

            int day = time.Day;
            if (!new CrontabValue(this.day).IsMatch(day)) return false;

            int month = time.Month;
            if (!new CrontabValue(this.month).IsMatch(month)) return false;

            int week = (int)time.DayOfWeek;
            if (!new CrontabValue(this.week).IsMatch(week)) return false;

            return true;
        }
    }

    /// <summary>
    /// 定时器的格式
    /// </summary>
    public struct CrontabValue
    {
        private string content;

        public CrontabValue(string value)
        {
            this.content = value;
        }

        public bool IsMatch(int value)
        {
            if (this.content == "*")
            {
                return true;
            }

            Regex regex;
            string content;

            // 单独数字
            regex = new Regex(@"^\d+$");
            if (regex.IsMatch(this.content))
            {
                return int.Parse(this.content) == value;
            }

            // 逗号隔开
            regex = new Regex(@"^\d[,\d+]+$");
            if (regex.IsMatch(this.content))
            {
                content = regex.Match(this.content).Value;
                int[] values = content.Split(',').Select(t => int.Parse(t)).ToArray();
                return values.Any(t => t == value);
            }

            // 范围
            regex = new Regex(@"^\d+\-\d+");
            if (regex.IsMatch(this.content))
            {
                content = regex.Match(this.content).Value;
                string[] strings = content.Split(",");
                int startValue = int.Parse(strings[0]);
                int endValue = int.Parse(strings[1]);
                if (value < startValue || value > endValue) return false;
            }

            // 定时运行
            regex = new Regex(@"\/\d+$");
            if (regex.IsMatch(this.content))
            {
                content = regex.Match(this.content).Value;
                int interval = int.Parse(content[1..]);
                if (value % interval != 0) return false;
            }

            return true;
        }
    }
}
