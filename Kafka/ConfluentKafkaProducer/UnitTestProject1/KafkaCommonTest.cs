using System;
using KafkaCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class KafkaCommonTest
    {
        //UrlPool_T1_List_20190429165747
        public static string KafkaBroker = "10.200.10.68:9092,10.200.10.69:9092,10.200.10.67:9092";
        public static string TopicFormat = string.Empty;

        [TestMethod]
        public void TestMethod1()
        {
            var topics = KafkaTool.GetMetaData(KafkaBroker);
        }
    }
}
