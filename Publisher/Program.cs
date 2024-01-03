using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;

namespace Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

                string srvCertCN = "PubSub";

                //string address = "net.tcp://localhost:4000/ITest";
                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                binding.Security.Mode = SecurityMode.Transport;


                /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
                X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, srvCertCN);
                EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4000/ITest"),
                                          new X509CertificateEndpointIdentity(srvCert));

                using(WCFPublisher publisher = new WCFPublisher(binding, address))
                {
                    Random r = new Random();

                    X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                           StoreLocation.LocalMachine, "wcfpublisher");

                    string alarmMessageBase = AlarmMessage.GetAlarmMessage;

                    while (true)
                    {
                        int risk = r.Next(1, 100);

                        AlarmEnum alarmType = GetAlarmTypeForRisk(risk);

                        string alarm = String.Format(alarmMessageBase, DateTime.Now, alarmType, risk);

                        byte[] signature = DigitalSignature.Create(alarm, HashAlgorithm.SHA1, certificateSign);


                        publisher.SendDataToEngine(alarm, signature);

                        Thread.Sleep(5000);
                    }
                    }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        public static AlarmEnum GetAlarmTypeForRisk(int risk)
        {
            if (risk >= 0 && risk <= 20)
            {
                return AlarmEnum.NO_ALARM;
            }
            else if (risk >= 21 && risk <= 40)
            {
                return AlarmEnum.FALSE_ALARM;
            }
            else if (risk >= 41 && risk <= 60)
            {
                return AlarmEnum.INFO;
            }
            else if (risk >= 61 && risk <= 80)
            {
                return AlarmEnum.WARNING;
            }
            else
            {
                return AlarmEnum.ERROR;
            }
        }
    }
}
