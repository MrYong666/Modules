using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ProducerKafkaRC;
using System.Text.RegularExpressions;
using System.Globalization;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public static string KafkaBroker = "10.200.10.68:9092,10.200.10.69:9092,10.200.10.67:9092";
        public static string TopicFormat = string.Empty;
        /// <summary>
        /// 删除N天前的Urlpool程序池数据   
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var topics = GetUrlPoolTopics(0, "D1");
            //  KafkaTool.DeletedTopics(KafkaBroker, topics);
        }
        [TestMethod]
        public static void DeletedTopicTest()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = KafkaBroker }).Build())
            {
                var allTopics = adminClient.GetMetadata(new TimeSpan(0, 0, 2));//获取所有Topic
                var topicInfo = allTopics.Topics.Where(w => w.Topic.Equals("UrlPool_D1_Product_1_2_20190514183358")).FirstOrDefault();
                Thread.Sleep(TimeSpan.FromSeconds(2)); // git the topic some time to be created.
                adminClient.DeleteTopicsAsync(new List<string> { topicInfo.Topic }).Wait();
            }
        }
        [TestMethod]
        public void GetTopicInfos()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = KafkaBroker }).Build())
            {
                var allTopics = adminClient.GetMetadata(new TimeSpan(0, 0, 2));//获取所有Topic
                var topicInfo = allTopics.Topics.Where(w => w.Topic.Equals("UrlPool_D1_Product_2_22_20190802112205")).FirstOrDefault();
                // Thread.Sleep(TimeSpan.FromSeconds(2)); // git the topic some time to be created.
                var tt = adminClient.GetMetadata("UrlPool_D1_Product_2_22_20190802112205", new TimeSpan(10000));
            }
        }
        /// <summary>
        /// 获取程序池Topic
        /// </summary>
        /// <param name="dbType">程序池Topic类型</param>
        /// <returns>根据dbType过滤后的程序池Topic</returns>
        private IEnumerable<string> GetUrlPoolTopics(int interval, string dbType)
        {
            Regex regex = new Regex($@"UrlPool_{dbType}_\w+_");
            var metadata = KafkaTool.GetMetaData(KafkaBroker);
            return metadata.Topics.Where(w =>
               {
                   return TopicFilter(w.Topic, dbType, interval);
               })
               .Select(s =>
                 {
                     return s.Topic;
                 })
                 .ToList()
               ;
        }
        /// <summary>
        /// 根据规则过滤Topic
        ///  UrlPool_D1_Product_1_2_20190514183358      UrlPool_\w\d + _\w + _\d_\d_\d +
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="dbType"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private bool TopicFilter(string topic, string dbType, int interval)
        {
            DateTime topicCreateTime = new DateTime();
            IFormatProvider ifp = new CultureInfo("zh-CN", true);
            Regex regex = new Regex($@"UrlPool_{dbType}_\w+_");
            if (!regex.IsMatch(topic))
            {
                return false;
            }
            var dtStr = topic.Split('_').Last();
            var bflag = DateTime.TryParseExact(dtStr, "yyyyMMddHHmmss", ifp, DateTimeStyles.None, out topicCreateTime);
            if (!bflag)
            {
                return true;
            }
            if (DateTime.Now.AddDays(-interval) >= topicCreateTime.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
