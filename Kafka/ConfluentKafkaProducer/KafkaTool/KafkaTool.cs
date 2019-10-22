using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaCommon
{
    public class KafkaTool
    {
        /// <summary>
        /// 删除Topic
        /// </summary>
        /// <param name="dbType">类型：D1，M1，G1等等</param>
        /// <param name="expireInterval">过期时间间隔</param>
        public static void DeletedTopics(string KafkaBroker, IEnumerable<string> topics)
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = KafkaBroker }).Build())
            {
                if (topics != null && topics.Count() > 0)
                {
                    adminClient.DeleteTopicsAsync(topics).Wait();
                }
            }
        }
        public static Metadata GetMetaData(string kafkaBroker)
        {
            Metadata metadata = null;
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = kafkaBroker }).Build())
            {
                metadata = adminClient.GetMetadata(new TimeSpan(0, 0, 5));//获取所有Topic
            }
            return metadata;
        }
        public static void GetOffSet(string kafkaBroker)
        {
            //using (var adminClient = new ProducerBuilder(new AdminClientConfig { BootstrapServers = kafkaBroker }).Build())
            //{
            //}
        }
        public static void Consumer(string kafkaBroker)
        {
            var consumerConfig = new ConsumerConfig
            {
                //分组
                GroupId = "Test",
                //BrokerServer
                BootstrapServers = kafkaBroker,
                //Session超时时间
                SessionTimeoutMs = 6000,
                //分区信息统计间隔
                StatisticsIntervalMs = 10000,
                //是否自动提交Offset； 手动提交Offset坑很多
                EnableAutoCommit = true,
                //offset读取方式；
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig)
                  // 设置Value的反序列话方式
                  .SetValueDeserializer(Deserializers.Utf8)
                  .SetErrorHandler((_, e) => { Console.WriteLine("ErrorHandler:" + e.Reason); })
                  //统计分区状态执行事件
                  .SetStatisticsHandler((_, json) =>
                  {
                      // Console.WriteLine($"Statistics: {json}");
                  })
                  //分配Consumer分区的时候执行
                  // 设置订阅的分区和Offset，默认为全部
                  //.SetPartitionsAssignedHandler((c, partitions) =>
                  //{
                  //    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                  //    //   return partitions.Where(w => w.Partition == 0).Select(tp => new TopicPartitionOffset(tp, 5));
                  //    var t1 = partitions.Where(w => w.Partition == 0).Select(tp => new TopicPartitionOffset(tp, 0));
                  //    return t1;
                  //})
                  .SetPartitionsRevokedHandler((c, partitions) =>
                  {
                      Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                  })
                  .Build())
            {
                //consumer.Position
            }
        }
    }
}
