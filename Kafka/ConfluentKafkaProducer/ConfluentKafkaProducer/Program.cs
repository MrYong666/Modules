using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfluentKafkaProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", getKafkaBroker() }
            };

            string topicName = getTopicName();

            using (Producer<Null, string> producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                Console.WriteLine("{" + producer.Name + "} producing on {" + topicName + "}. q to exit.");

                string text;
                while ((text = Console.ReadLine()) != "q")
                {
                    var msg = producer.ProduceAsync(topicName, null, text).Result;
                    string message = string.Empty;
                    if (msg.Error.Code == ErrorCode.NoError)
                    {
                        message = string.Format("值是：【{0}】，topic名是：【{1}】, Partition是：【{2}】，Offset是:【{3}】",
                                    msg.Value,
                                    msg.TopicPartitionOffset.Topic,
                                    msg.TopicPartitionOffset.Partition,
                                    msg.TopicPartitionOffset.Offset
                           );
                    }
                    else
                    {
                        message = msg.Error.Reason;
                    }

                    Console.WriteLine(message);
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

    }
}
