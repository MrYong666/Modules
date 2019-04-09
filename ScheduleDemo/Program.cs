using FluentScheduler;
using System;
using System.Collections.Generic;
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
            //启动线程
            Task.Factory.StartNew(() =>
            {
                RunOpsTotal();
            });
            Thread.Sleep(100000000);
        }
        public static void RunOpsTotal()
        {
            JobManager.AddJob(() =>
            {
                Console.WriteLine("定时任务开始了!");
            },
           (s) => s.ToRunNow().AndEvery(3000).Milliseconds());
        }
    }
}
