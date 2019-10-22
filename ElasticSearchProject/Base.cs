using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchBeta
{
    public class Base
    {
        public static ElasticClient client = new ElasticClient(ConnectionSettings);
        private static Uri Node
        {
            get
            {
                return new Uri(@"http://localhost:9200");
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
