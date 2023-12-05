using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace Subscriber
{
    internal class WCFSubscriber : ChannelFactory<ITest>, IDisposable
	{
		ITest factory;

		public WCFSubscriber(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Manager.Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

        public void Subscribe(string alarmTypes, string clientAddress)
        {
            try
            {
                factory.Subscribe(alarmTypes, clientAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.ReadLine();
            }
        }

        public void Unsubscribe(string clientAddress)
        {
            try
            {
                factory.Unsubscribe(clientAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.ReadLine();
            }
        }

        public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}
	}
}
