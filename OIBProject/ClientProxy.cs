using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class ClientProxy : ChannelFactory<ISubscriberEngine>, ISubscriberEngine, IDisposable
    {
        ISubscriberEngine factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            
            factory = this.CreateChannel();

        }

        public void SendDataToSubscriber(string alarm, byte[] sign, byte[] publisherName)
        {
            try
            {
                factory.SendDataToSubscriber(alarm, sign, publisherName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }
    }
}
