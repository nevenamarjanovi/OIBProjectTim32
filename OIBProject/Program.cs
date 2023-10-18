using Common;
using OIBProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(PubSubEngineServer)))
            {
                string address = "net.tcp://localhost:4000/ITest";
                NetTcpBinding binding = new NetTcpBinding();

                host.AddServiceEndpoint(typeof(ITest), binding, address);

                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut");
                Console.ReadKey();
            }
        }
    }
}
