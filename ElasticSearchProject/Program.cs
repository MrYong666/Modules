using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ElasticSearchBeta
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Search
            ESSearch.CreateIndex();
            ESSearch.search2();
            #endregion


            ESProvider es = new ESProvider();
            List<MeetupEvents> pbs = new List<MeetupEvents>();
            for (int i = 0; i < 10; i++)
            {
                MeetupEvents pb = new MeetupEvents();
                pb.eventid = i;
                pb.orignalid = $"{i}orignalId888";
                pb.eventname = $"{i}eventname";
                pb.description = $"{i}description";

                pbs.Add(pb);
            }
            //es.BulkPopulateIndex(pbs);
        }
    }
    public class ESProvider
    {
        public static ElasticClient client = new ElasticClient(Setting.ConnectionSettings);
        public static string strIndexName = @"meetup".ToLower();
        public static string strDocType = "events".ToLower();

        //public bool PopulateIndex(MeetupEvents meetupevent)
        //{
        //    var index = client.Index(meetupevent, i => i.Index(strIndexName).Type(strDocType).Id(meetupevent.eventid));
        //    return index.IsValid;
        //}

        //public bool BulkPopulateIndex(List<MeetupEvents> posts)
        //{
        //    var bulkRequest = new BulkRequest(strIndexName, strDocType) { Operations = new List<IBulkOperation>() };
        //    var idxops = posts.Select(o => new BulkIndexOperation<MeetupEvents>(o) { Id = o.eventid }).Cast<IBulkOperation>().ToList();
        //    bulkRequest.Operations = idxops;
        //    var response = client.Bulk(bulkRequest);
        //    return response.IsValid;
        //}
    }
    public class MeetupEvents
    {
        public int eventid { get; set; }
        public string orignalid { get; set; }
        public string eventname { get; set; }
        public string description { get; set; }
    }
    public static class Setting
    {
        public static string strConnectionString = @"http://localhost:9200";
        public static Uri Node
        {
            get
            {
                return new Uri(strConnectionString);
            }
        }
        public static ConnectionSettings ConnectionSettings
        {
            get
            {
                return new ConnectionSettings(Node).DefaultIndex("default");
            }
        }
    }
}
