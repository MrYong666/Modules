using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfluentKafkaConsumer
{
    public class Program
    {
        public static int Count = 0;

        static void Main(string[] args)
        {
            var startIndex = 10;
            var endIndex = 10;
            var actionFlag = true;
            var partition = 1;
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
                EnableAutoCommit = false,
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
                    .SetPartitionsAssignedHandler((c, partitions) =>
                    {
                        Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                        // return partitions.Select(tp => new TopicPartitionOffset(tp, 5));
                        // var t1 = partitions.Where(w => w.Partition == 0).Select(tp => new TopicPartitionOffset(tp, 0));
                        // return t1;
                    })
                    //.SetPartitionsRevokedHandler((c, partitions) =>
                    //{
                    //    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                    //    //  return partitions.Select(tp => new TopicPartitionOffset(topic, 1, 0));
                    //})
                    .Build())
            {
                //要订阅的Topic
                //  consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        consumer.Assign(new List<TopicPartitionOffset>() { new TopicPartitionOffset(topic, 0, startIndex) });

                        while (actionFlag)
                        {
                            try
                            {
                                //进行订阅
                                var consumeResult = consumer.Consume(cts.Token);
                                //   consumer.Commit();
                                if (consumeResult != null)
                                {
                                    Console.WriteLine($"0  Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                                    if (consumeResult.Offset >= endIndex)
                                    {
                                        actionFlag = false;
                                    }
                                }
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Consume error: {e.Error.Reason}");
                            }
                        }
                        Console.WriteLine("跳出循环");
                        startIndex += 10;
                        endIndex += 10;
                        actionFlag = true;
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
