using DocxSignature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace DocxSignatureSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            WebConfig();
        }

        private static void WebConfig()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Routes.MapHttpRoute(
            "API Default", "api/{controller}/{id}", 
            new { id = RouteParameter.Optional
            });
            //config.Services.Add(typeof(ISignature), new Signature());

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
    }
}
