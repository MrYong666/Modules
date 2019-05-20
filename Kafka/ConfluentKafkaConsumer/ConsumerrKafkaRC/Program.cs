using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerrKafkaRC
{
    class Program
    {
        static void Main(string[] args)
        {
            var topic = getTopicName();
            var consumerConfig = new ConsumerConfig
            {
                //分组
                GroupId = getKafkaGroupID(),
                //BrokerServer
                BootstrapServers = getKafkaBroker(),
                //Session超时时间
                SessionTimeoutMs = 6000,
                //分区信息统计间隔
                StatisticsIntervalMs = 10000,
                //是否自动提交Offset； 手动提交Offset坑很多
                EnableAutoCommit = true,
                //offset读取方式；
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            //用于取消进程的执行
            CancellationTokenSource cts = new CancellationTokenSource();
            //<key,value>: Ignore的话值为空
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
                //要订阅的Topic
                // consumer.Subscribe(topic);
                consumer.Assign(new List<TopicPartitionOffset>() { new TopicPartitionOffset("Test_2020", 2, 10) });

                try
                {
                    while (true)
                    {
                        try
                        {
                            //进行订阅
                            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
                            if (consumeResult != null)
                            {
                                Console.WriteLine($" 2 Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                            }
                            //if (consumeResult.IsPartitionEOF)
                            //{
                            //    Console.WriteLine(
                            //        $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                            //    continue;
                            //}

                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }


        private static string getKafkaBroker()
        {
            return ConfigurationManager.AppSettings["KafkaBroker"].ToString();
        }

        private static string getTopicName()
        {
            return ConfigurationManager.AppSettings["KafkaTopicName"].ToString();
        }

        private static string getKafkaGroupID()
        {
            return ConfigurationManager.AppSettings["KafkaGroupID"].ToString();
        }

    }
}
