using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unirest_net;

namespace kawaii_animedb
{
    class API
    {
        public void GetAPIData(string key, string RequestUrl)
        {
            unirest_net.http.HttpResponse<string> request = unirest_net.http.Unirest.get(RequestUrl).header("X-Mashape-Key", key).asString();
            Console.WriteLine(request.Body);

        }
    }
}
