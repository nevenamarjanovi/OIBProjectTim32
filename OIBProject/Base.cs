using PubSubEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIBProject
{
    public class Base
    {
        internal static ConcurrentDictionary<string, Subscriber> subscribers = new ConcurrentDictionary<string, Subscriber>();
    }
}
