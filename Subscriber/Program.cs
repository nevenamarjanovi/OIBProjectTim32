using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = "PubSub";

            //string address = "net.tcp://localhost:4000/ITest";
            NetTcpBinding binding = new NetTcpBinding();

            binding.Security.Mode = SecurityMode.None;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4000/ITest"),
                                      new X509CertificateEndpointIdentity(srvCert));


            using (WCFSubscriber proxy = new WCFSubscriber(binding, address))
            {
                proxy.TestCommunication();
            }

        }
    }
}
