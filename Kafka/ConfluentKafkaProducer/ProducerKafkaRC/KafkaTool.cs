using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerKafkaRC
{
    public class KafkaTool
    {
        /// <summary>
        /// 发送kafka消息
        /// </summary>
        public static void ProdeceKafka()
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", getKafkaBroker() }
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = getKafkaBroker(),
            };


            string topicName = getTopicName();
            using (var producer = new ProducerBuilder<string, string>(consumerConfig).Build())
            {
                Console.WriteLine("\n-----------------------------------------------------------------------");
                Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("To create a kafka message with UTF-8 encoded key and value:");
                Console.WriteLine("> key value<Enter>");
                Console.WriteLine("To create a kafka message with a null key and UTF-8 encoded value:");
                Console.WriteLine("> value<enter>");
                Console.WriteLine("Ctrl-C to quit.\n");

                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                while (!cancelled)
                {
                    Console.Write("> ");

                    string text;
                    try
                    {
                        text = Console.ReadLine();
                    }
                    catch (IOException)
                    {
                        // IO exception is thrown when ConsoleCancelEventArgs.Cancel == true.
                        break;
                    }
                    if (text == null)
                    {
                        // Console returned null before 
                        // the CancelKeyPress was treated
                        break;
                    }

                    string key = null;
                    string val = text;

                    // split line if both key and value specified.
                    int index = text.IndexOf(" ");
                    if (index != -1)
                    {
                        key = text.Substring(0, index);
                        val = text.Substring(index + 1);
                    }

                    try
                    {
                        // Note: Awaiting the asynchronous produce request below prevents flow of execution
                        // from proceeding until the acknowledgement from the broker is received (at the 
                        // expense of low throughput).
                        var deliveryReport = producer.ProduceAsync(
                            topicName, new Message<string, string> { Key = key, Value = val }).ContinueWith(c =>
                            {
                                var t = c.Result;
                                Console.WriteLine($"发送完成;Offset:{t.Offset},Patitton: {t.Partition}");
                            });

                        //   Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                }

                // Since we are producing synchronously, at this point there will be no messages
                // in-flight and no delivery reports waiting to be acknowledged, so there is no
                // need to call producer.Flush before disposing the producer.
            }
        }
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
        private static string getKafkaBroker()
        {
            return ConfigurationManager.AppSettings["KafkaBroker"].ToString();
        }
        private static string getTopicName()
        {
            return ConfigurationManager.AppSettings["KafkaTopicName"].ToString();
        }
    }
}
