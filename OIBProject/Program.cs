using Common;
using OIBProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace PubSubEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(PubSubEngineServer)))
            {

                string srvCertCN = Manager.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
  

                string address = "net.tcp://localhost:4000/ITest";
                NetTcpBinding binding = new NetTcpBinding();

                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                binding.Security.Mode = SecurityMode.None;

                host.AddServiceEndpoint(typeof(ITest), binding, address);

                ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

                ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
                host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut");
                Console.ReadKey();
            }
        }
    }
}
