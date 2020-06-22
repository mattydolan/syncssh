using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NLog;

namespace emri_service.RabbitMQSetup
{
    public static class ConfigureRabbitMQBusService
    {
		public static void ConfigureRabbitMQBus(this IServiceCollection services, 
			string uri, X509Certificate2 clientCert)
		{
			services.AddMassTransit(x =>
			{
				x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
				{
					//to do add logging Ilogger
					
					var host = cfg.Host(uri, h => h.UseSsl(ssl =>
					{
						ssl.Certificate = clientCert;
						ssl.UseCertificateAsAuthenticationIdentity = true;
						ssl.Protocol = SslProtocols.Tls12;
						ssl.CertificateValidationCallback = ValidationError;
						ssl.CertificateSelectionCallback = SelectLocalCertificate;
					}
					));
				}));
			});
		}

		private static bool ValidationError(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None;
		}

		private static X509Certificate SelectLocalCertificate(
			object sender,
			string targetHost,
			X509CertificateCollection localCertificates,
			X509Certificate remoteCertificate,
			string[] acceptableIssuers)
		{

			if (acceptableIssuers != null &&
				acceptableIssuers.Length > 0 &&
				localCertificates != null &&
				localCertificates.Count > 0)
			{
				// Use the first certificate that is from an acceptable issuer.
				foreach (X509Certificate certificate in localCertificates)
				{
					string issuer = certificate.Issuer;
					if (Array.IndexOf(acceptableIssuers, issuer) != -1)
						return certificate;
				}
			}
			if (localCertificates != null &&
				localCertificates.Count > 0)
				return localCertificates[0];

			return null;
		}
	}
}
