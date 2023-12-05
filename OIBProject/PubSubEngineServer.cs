using System;
using AES;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Manager;
using PubSubEngine;
using System.ServiceModel;

namespace OIBProject
{
    public class PubSubEngineServer : ITest
    {
        public void TestCommunication()
        {
            Console.WriteLine("test neki");
        }

        public void SendDataToEngine(string alarm, byte[] sign)
        {
            string publisherName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keyPubEng.txt");
            string decrypedAlarm = AES.Decryption.DecryptString(alarm, AES.SecretKey.LoadKey(startupPath));

            Console.WriteLine(decrypedAlarm);

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] publisherNameBytes = encoding.GetBytes(publisherName);
            string startupPathh = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");

            foreach (Subscriber s in Base.subscribers.Values)
            {
                X509Certificate2 subscriberCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, s.SubscriberName);
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)subscriberCert.PublicKey.Key;

                foreach (AlarmEnum alarmType in s.Alarms)
                {
                    if (alarmType.Equals(alarmType))
                    {
                        s.Proxy.SendDataToSubscriber(AES.Encryption.EncryptString(decrypedAlarm, AES.SecretKey.LoadKey(startupPathh)),
                                                    sign, csp.Encrypt(publisherNameBytes, false));
                    }
                }
            }

        }

        public void Subscribe(string alarmTypes, string clientAddress)
        {
            string subscriberName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");

            string decryptedAddress = AES.Decryption.DecryptString(clientAddress, AES.SecretKey.LoadKey(startupPath));

            string decryptedAlarmTypes = AES.Decryption.DecryptString(alarmTypes, AES.SecretKey.LoadKey(startupPath));

            string[] parts = decryptedAlarmTypes.Split(' ');

            List<AlarmEnum> alarmTypess = new List<AlarmEnum>();

            foreach (string at in parts)
            {
                if (at == "")
                {
                    break;
                }
                alarmTypess.Add((AlarmEnum)Enum.Parse(typeof(AlarmEnum), at));
            }


            NetTcpBinding binding = new NetTcpBinding();
            ClientProxy pr = new ClientProxy(binding, decryptedAddress);

            Subscriber s = new Subscriber(alarmTypess, pr, subscriberName);

            Base.subscribers.TryAdd(decryptedAddress, s);

            Console.WriteLine("New subscriber!");
        }

        public void Unsubscribe(string clientAddress)
        {
            Subscriber ret;
            Base.subscribers.TryRemove(clientAddress, out ret);

            Console.WriteLine("New unsubscriber!");
        }
    }
}
