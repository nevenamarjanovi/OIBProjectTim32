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

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4000/ITest"),
                                      new X509CertificateEndpointIdentity(srvCert));


            ChannelFactory<ITest> channel = new ChannelFactory<ITest>(binding, address);
            ITest proxy = channel.CreateChannel();

            proxy.TestCommunication();
        }
    }
}
