using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchRelease
{
    public class ESHelper
    {
        #region ES Instance
        public static IElasticClient client
        {
            get
            {
                return GetClient();
            }
        }
        private static IElasticClient GetClient()
        {
            var uris = new[]
                               {
                                    new Uri("http://localhost:9200"),
                                    new Uri("http://localhost:9201"),
                                    new Uri("http://localhost:9202"),
                                };

            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool);
            var client = new ElasticClient(settings);
            return client;
        }
        #endregion

        /// <summary>
        /// 创建索引
        /// 索引名称必须为小写
        /// </summary>
        public static void CreateIndex()
        {
            IIndexState indexState = new IndexState()
            {
                Settings = new IndexSettings()
                {
                    NumberOfReplicas = 1,//副本数
                    NumberOfShards = 5//分片数
                }
            };
            client.CreateIndex("vendorpriceinfo", p => p.InitializeUsing(indexState));
        }
        /// <summary>
        /// 创建索引数据
        /// </summary>
        public static void CreateIndexData()
        {
            var vendorPriceInfo = new VendorPriceInfo
            {
                priceID = 1,
                oldID = 2,
                source = "ttt"
            };
            client.Index(vendorPriceInfo, o => o.Index("vendorpriceinfo").Type<VendorPriceInfo>());//使用 2）标记的类型
        }
        public static void GetMapping()
        {

        }
        public static void Search()
        {
            var searchResponse = client.Search<VendorPriceInfo>(s => s
                               .From(0)
                               .Size(10)
                               .Index("vendorpriceinfo")
                               .Query(q => q
                                    .Match(m => m
                                       .Field(f => f.priceID)
                                       .Query("1")
                                    )
                               )
                           );

            var people = searchResponse.Documents;
        }
        public static void Search1()
        {
            var result = client.Search<VendorPriceInfo>(s => s.Index("vendorpriceinfo"));
        }

        public VendorPriceInfo GetCliass()
        {
            VendorPriceInfo vendorPriceInfo = new VendorPriceInfo();
            vendorPriceInfo.priceID = 1;
            vendorPriceInfo.oldID = 11;
            vendorPriceInfo.source = "asdfasfd";
            return vendorPriceInfo;
        }
        [ElasticsearchType(IdProperty = "priceID", Name = "datatype")]
        public class VendorPriceInfo
        {
            public Int64 priceID { get; set; }
            public int oldID { get; set; }
            public string source { get; set; }
        }
    }
}
