using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerKafkaRelease
{
    class Program
    {
        static public int Count = 0;
        static void Main(string[] args)
        {
            BenchmarkConsumerImpl(getKafkaBroker(), getTopicName());
        }

        public static void BenchmarkConsumerImpl(string bootstrapServers, string topic)
        {
            var consumerConfig = new Dictionary<string, object>
            {
                { "group.id", getKafkaGroupID() },
                { "bootstrap.servers", getKafkaBroker() },
                { "session.timeout.ms", 6000 },
                { "auto.offset.reset", "earliest" },
                { "auto.commit.interval.ms", 5000 },
            };

            using (var consumer = new Consumer(consumerConfig))
            {
                //  TopicPartitionOffset par1 = new TopicPartitionOffset(topic, 0, 0);
                //  TopicPartitionOffset par2 = new TopicPartitionOffset(topic, 1, 0);
                //consumer.Assign(new List<TopicPartitionOffset>() { par1 });

                consumer.Subscribe(topic);

                Message msg;
                while (true)
                {
                    if (consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
                    {
                        C(msg);
                    }
                }
            }
        }
        private static void C(Message msg)
        {
            var values = System.Text.Encoding.Default.GetString(msg.Value);
            string message = string.Format("topic名是：【{0}】, Partition是：【{1}】，Offset是:【{2}】",
                    msg.TopicPartitionOffset.Topic,
                    msg.TopicPartitionOffset.Partition,
                    msg.TopicPartitionOffset.Offset
                );
            Console.WriteLine(message);
            Count++;
            Console.WriteLine("共" + Count.ToString() + "条");
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
