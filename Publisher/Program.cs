using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string address = "net.tcp://localhost:4000/ITest";
            NetTcpBinding binding = new NetTcpBinding();

            ChannelFactory<ITest> channel = new ChannelFactory<ITest>(binding, address);
            ITest proxy = channel.CreateChannel();

            string nesto = "123";
            proxy.Ispisi("nesto");
        }
    }
}
