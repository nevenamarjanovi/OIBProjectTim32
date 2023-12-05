using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class Subscriber
    {
        List<AlarmEnum> alarms;
        ClientProxy proxy;
        string subscriberName;

        public Subscriber(List<AlarmEnum> alarms, ClientProxy proxy, string subscriberName)
        {
            this.Alarms = alarms;
            this.proxy = proxy;
            this.subscriberName = subscriberName;
        }

        public List<AlarmEnum> Alarms { get => alarms; set => alarms = value; }
        public ClientProxy Proxy { get => proxy; set => proxy = value; }
        public string SubscriberName { get => subscriberName; set => subscriberName = value; }
    }
}
