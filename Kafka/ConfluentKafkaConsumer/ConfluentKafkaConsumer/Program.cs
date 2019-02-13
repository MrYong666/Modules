using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfluentKafkaConsumer
{
    public class Program
    {
        public static int Count = 0;

        static void Main(string[] args)
        {
            //group.id 消费者分组id
            //同一Topic的一条消息只能被同一个Consumer Group内的一个Consumer消费，但多个Consumer Group可同时消费这一消息。这是Kafka用来实现一个Topic消息的广播（发给所有的Consumer）和单播（发给某一个Consumer）的手段，一个Topic可以对应多个Consumer Group。如果需要实现广播，只要每个Consumer有一个独立的Group就可以了。要实现单播只要所有的Consumer在同一个Group里。用Consumer Group还可以将Consumer进行自由的分组而不需要多次发送消息到不同的Topic。

            //bootstrap.servers：kafka集群消费地址

            //auto.commit.interval.ms：consumer向zookeeper提交offset的频率，单位是毫秒

            //auto.offset.reset：具体参数含义如下： 
            //earliest
            //当各分区下有已提交的offset时，从提交的offset开始消费；无提交的offset时，从头开始消费
            //latest
            //当各分区下有已提交的offset时，从提交的offset开始消费；无提交的offset时，消费新产生的该分区下的数据
            //none
            //topic各分区都存在已提交的offset时，从offset后开始消费；只要有一个分区不存在已提交的offset，则抛出异常
            //详细每个auto.offset.reset参数参考下面的blog： 
            var conf = new Dictionary<string, object>
                {
                  { "group.id", getKafkaGroupID() },
                  { "bootstrap.servers", getKafkaBroker() },
                  { "auto.commit.interval.ms", 5000 },
                  { "auto.offset.reset", "earliest" }
                };

            using (var consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnMessage += (_, msg)
                  => C(msg);

                consumer.OnError += (_, error)
                  => Console.WriteLine($"Error: {error}");

                consumer.OnConsumeError += (_, msg)
                  => Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");

                consumer.Subscribe(getTopicName());

                while (true)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }


        private static void C(Message<Null, string> msg)
        {
            string message = string.Format("值是：【{0}】，topic名是：【{1}】, Partition是：【{2}】，Offset是:【{3}】",
                    msg.Value, 
                    msg.TopicPartitionOffset.Topic, 
                    msg.TopicPartitionOffset.Partition, 
                    msg.TopicPartitionOffset.Offset
                );
            Console.WriteLine(message);
            Console.WriteLine(string.Format("共{0}条", ++Count));
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
