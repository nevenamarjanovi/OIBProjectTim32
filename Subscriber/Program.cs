using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Description;
using AESEncAlg;
using System.Diagnostics;

namespace Subscriber
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            NetTcpBinding binding = new NetTcpBinding();
            
            ServiceHost host = new ServiceHost(typeof(SubscriberEngine)); ///////////////
            //string address = host.BaseAddresses.First().ToString();
            string address = "net.tcp://localhost:4000/PubSubEngineServer";

            host.AddServiceEndpoint(typeof(ISubscriberEngine), binding, address);

            
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();
            string srvCertCN = "PubSub";

            NetTcpBinding bind = new NetTcpBinding();

            bind.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress addr = new EndpointAddress(new Uri("net.tcp://localhost:4000/PubSubEngineServer"),
                                      new X509CertificateEndpointIdentity(srvCert));


            WCFSubscriber sub = new WCFSubscriber(bind, addr);

            while (true)
            {
                try
                {

                    List<AlarmEnum> alarmTypes = new List<AlarmEnum>();
                    int alarmType;

                    Console.WriteLine("Izaberite alarme na koje zelite da se pretplatite.");
                    Console.WriteLine("Kada izaberete sve alarme na koje zelite da se pretplatite unesite 0.\n");
                    do
                    {
                        Console.WriteLine("Unesite tip alarma:\n 1. NO_ALARM\n 2. FALSE_ALARM\n 3. INFO\n 4. WARNING\n 5. ERROR\n");
                        if (!Int32.TryParse(Console.ReadLine(), out alarmType))
                        {
                            Console.WriteLine("Pogresan unos.");
                            continue;
                        }

                        if (alarmType == 0) break;


                        if (alarmType < 1 || alarmType > 5)
                        {
                            Console.WriteLine("Pogresan unos.");
                            continue;
                        }

                        if (alarmTypes.Contains((AlarmEnum)alarmType - 1))
                        {
                            Console.WriteLine("Vec ste odabrali taj alarm.");
                            continue;
                        }

                        alarmTypes.Add((AlarmEnum)alarmType - 1);

                    } while (true);



                    string alarmTypess = "";
                    foreach (AlarmEnum at in alarmTypes)
                    {
                        alarmTypess = alarmTypess + at + " ";
                    }

                    string key = AESEncAlg.SecretKey.GenerateKey();

                    string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
                    SecretKey.StoreKey(key, startupPath);


                    sub.Subscribe(AESEncAlg.Encryption.EncryptString(alarmTypess, key),
                        AESEncAlg.Encryption.EncryptString(address, key));



                    Console.WriteLine("Pritisnite x za gasenje:");

                    if (Console.ReadLine() == "x")
                    {
                        break;
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] {0}", e.Message);
                    Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                }

            }

            sub.Unsubscribe(address);

        }
    }
}
