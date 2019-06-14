using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleDemo
{
    class Program : Registry
    {
        static void Main(string[] args)
        {
            Scheduler.StartUp();
            //   RunOpsTotal();
            //启动线程
            Task.Factory.StartNew(() =>
            {
                //  RunOpsTotal();
            });
            Console.WriteLine("程序执行中,请勿关闭...");
            Console.ReadLine();
        }
        public static void RunOpsTotal()
        {
            JobManager.AddJob(() =>
            {
                Console.WriteLine("开始执行...!");
            },
           (s) => s.ToRunEvery(1).Days().At(14, 59));
            // (s) => s.ToRunNow().AndEvery(3000).Milliseconds());    每隔三秒执行
        }
    }
    internal class Demo : IJob
    {
        void IJob.Execute()
        {
            Console.WriteLine("开始定时任务了，现在时间是：" + DateTime.Now);
        }
    }
    /// <summary>
    /// 待处理的作业工厂，在构造函数中设置好各个Job的执行计划。
    /// 参考【https://github.com/fluentscheduler/FluentScheduler】
    /// </summary>
    internal class ApiJobFactory : Registry
    {
        public ApiJobFactory()
        {
            // 立即执行每两秒一次的计划任务。（指定一个时间间隔运行，根据自己需求，可以是秒、分、时、天、月、年等。）
            Schedule<Demo>().ToRunNow().AndEvery(2).Seconds();

            // 在一个指定时间执行计划任务（最常用。这里是在每天的下午 1:10 分执行）
            Schedule<Demo>().ToRunEvery(1).Days().At(13, 10);
        }
    }
    public static class Scheduler
    {
        /// <summary>
        /// 启动定时任务
        /// </summary>
        public static void StartUp()
        {
            JobManager.Initialize(new ApiJobFactory());
        }

        /// <summary>
        /// 停止定时任务
        /// </summary>
        public static void Stop()
        {
            JobManager.Stop();
        }
    }
}
