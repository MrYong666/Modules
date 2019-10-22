using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisDemo
{
    class Program
    {
        protected static RedisHelper RedisHelper = new RedisHelper(0);
        static void Main(string[] args)
        {
            RedisHelper.StringSet("ddd", "asdf");
            Lock();

        }
        static void Lock()
        {
            Console.WriteLine("Start..........");
            RedisValue token = Environment.MachineName;
            var r1 = RedisHelper.LockTake("test", token, TimeSpan.FromSeconds(10));
            //實際項目秒殺此處可換成商品ID
            if (r1)
            {
                try
                {
                    Console.WriteLine("Working..........");
                    Thread.Sleep(5000);
                }
                finally
                {
                    RedisHelper.LockRelease("test", token);
                }
            }

            Console.WriteLine("Over..........");
        }
    }
    /// <summary>
    /// ConnectionMultiplexer对象管理帮助类
    /// </summary>
    public static class RedisConnectionHelp
    {

        //系统自定义Key前缀
        public static readonly string SysCustomKey = string.Empty; // ConfigurationManager.AppSettings["redisKey"] ?? "Ken:";

        //127.0.0.1:6379,allowadmin=true
        private static string RedisConnectionString
        {
            get
            {
                try
                {
                    return "172.26.10.219:6379,allowadmin=true";
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static readonly object locker = new object();
        private static ConnectionMultiplexer _instance;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        /// <summary>
        /// 单例获取
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 缓存获取
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            connectionString = connectionString ?? RedisConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                return null;
            var connect = ConnectionMultiplexer.Connect(connectionString);

            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件



    }

    public class RedisHelper
    {
        public const int MaxWriteRedisCountLimit = 5000;

        private int DbNum { get; }
        private readonly ConnectionMultiplexer _conn;

        #region 构造函数

        public RedisHelper(int dbNum = 0)
                : this(dbNum, null)
        {
        }

        public RedisHelper(int dbNum, string readWriteHosts)
        {
            DbNum = dbNum;
            _conn =
                string.IsNullOrWhiteSpace(readWriteHosts) ?
                RedisConnectionHelp.Instance :
                RedisConnectionHelp.GetConnectionMultiplexer(readWriteHosts);
        }

        #endregion 构造函数

        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return Do(db => db.StringSet(key, value, expiry));
        }
        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry)
        {
            return Do(db => db.LockTake(key, value, expiry));
        }
        public bool LockRelease(RedisKey key, RedisValue value)
        {
            return Do(db => db.LockRelease(key, value));
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _conn.GetDatabase(DbNum);
            return func(database);
        }

    }
}
