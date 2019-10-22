using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchBeta
{
    [ElasticsearchType(RelationName = "datatype")]
    public class VendorPriceInfo
    {
        public Int64 priceID { get; set; }
        public int oldID { get; set; }
        public int source { get; set; }
    }
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class ESSearch : Base
    {
        /// <summary>
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
            //  client.Create("createindextest", p => p.InitializeUsing(indexState));
        }
        public static void CreateType()
        {
            var vendorPriceInfo = new VendorPriceInfo
            {
                priceID = 1,
                oldID = 2,
                source = 3
            };
            // client.Index(vendorPriceInfo, o => o.VersionType<VendorPriceInfo>());//使用 2）标记的类型
        }
        public static void Search5()
        {
            var searchResponse = client.Search<MeetupEvents1>(s => s
                                 .From(0)
                                 .Size(10)
                                 .Index("meetup")
                                 .Query(q => q
                                      .Match(m => m
                                         .Field(f => f.eventid)
                                         .Query("1")
                                      )
                                 )
                             );

            var people = searchResponse.Documents;


            var response = client.Search<MeetupEvents1>(s => s
                            .Index("meetup")
                            .Query(q => q
                                .MatchAll()
                            )
                        );
            var peo1ple = searchResponse.Documents;

        }
        public static void search2()
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



            var person = new Person
            {
                Id = 1,
                FirstName = "Martijn",
                LastName = "Laarman"
            };
            //IndexRequest indexRequest = new IndexRequest("people");
            //indexRequest.so
            //peoples.Add(person);
            var index = client.Index(person, i => i.Index("person").Routing("TT").Id(person.Id));
        }
        public static void search1()
        {
            var list = new List<string>();
            var connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var settings = new ConnectionSettings(connectionPool, new InMemoryConnection()) // <1> Here we use `InMemoryConnection` but in a real application, you'd use an `IConnection` that _actually_ sends the request, such as `HttpConnection`
                .DefaultIndex("meetup")
                .DisableDirectStreaming() // <2> Disable direct streaming so we can capture the request and response bytes
                .OnRequestCompleted(apiCallDetails => // <3> Perform some action when a request completes. Here, we're just adding to a list, but in your application you may be logging to a file.
                {
                    // log out the request and the request body, if one exists for the type of request
                    if (apiCallDetails.RequestBodyInBytes != null)
                    {
                        list.Add(
                            $"{apiCallDetails.HttpMethod} {apiCallDetails.Uri} " +
                            $"{Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes)}");
                    }
                    else
                    {
                        list.Add($"{apiCallDetails.HttpMethod} {apiCallDetails.Uri}");
                    }

                    // log out the response and the response body, if one exists for the type of response
                    if (apiCallDetails.ResponseBodyInBytes != null)
                    {
                        list.Add($"Status: {apiCallDetails.HttpStatusCode}" +
                                 $"{Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes)}");
                    }
                    else
                    {
                        list.Add($"Status: {apiCallDetails.HttpStatusCode}");
                    }
                });

            var client = new ElasticClient(settings);


            var searchResponse1 = client.Search<MeetupEvents1>(s => s
                                .From(0)
                                .Size(10)
                                .Query(q => q
                                     .Match(m => m
                                        .Field(f => f.eventid)
                                        .Query("1")
                                     )
                                )
                            );

            var people = searchResponse1.Documents;


            var searchResponse = client.Search<MeetupEvents1>(s => s
                        .StoredFields(sf => sf
                            .Fields(
                                f => f.eventname
                            )
                        )
                        .Query(q => q
                            .MatchAll()
                        )
                    );

            var response = client.Search<MeetupEvents1>(s => s
                                .Query(q => q
                                    .MatchAll()
                                )
                            );

            var firstSearchResponse = client.Search<MeetupEvents1>(s => s
               .Query(q => q
                   .Term(p => p.eventid, 1)
               )
           );
        }

        public static void Search()
        {
            var request = new SearchRequest
            {
                From = 0,
                Size = 10,
                Query = new TermQuery { Field = "eventname", Value = "eventname" }
            };
            var firstSearchResponse = client.Search<MeetupEvents1>(s => s
                  .Query(q => !q
                      .Term(p => p.eventname, "3eventname")
                  )
              );

            SearchRequest sr = new SearchRequest();
            //"meetup", "events"
            TermQuery tq = new TermQuery();
            tq.Field = "eventname";
            tq.Value = "azu.*";
            sr.Query = tq;

            //windows
            sr.From = 0;
            sr.Size = 100;

            //source filter
            sr.Source = new SourceFilter()
            {
                Includes = new string[] { "eventid", "eventname" },
                Excludes = new string[] { "roginalid", "description" }
            };
            var index = client.Search<MeetupEvents1>(sr);
        }

    }
    public class MeetupEvents1
    {
        public static string TypeName = "events";
        public string Type => TypeName;
        public int eventid { get; set; }
        public string orignalid { get; set; }
        public string eventname { get; set; }
        public string description { get; set; }
    }
}
