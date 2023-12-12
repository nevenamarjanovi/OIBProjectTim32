using Common;
using AESEncAlg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Publisher
{
    public class WCFPublisher : ChannelFactory<ITest>, IDisposable
	{
		ITest factory;

		public WCFPublisher(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

		public void TestCommunication()
		{
			try
			{
				factory.TestCommunication();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}

		public void SendDataToEngine(string alarm, byte[] sign)
		{
			try
			{
				string key = AESEncAlg.SecretKey.GenerateKey();
				string startupPath = "C:\\Users\\Public\\keyPubEng.txt";
				SecretKey.StoreKey(key, startupPath);

				factory.SendDataToEngine(AESEncAlg.Encryption.EncryptString(alarm, key), sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: {0}", e);
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
