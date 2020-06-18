using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using emri_service.Middleware;
using DddsUtils.Logging.NetStandard;
using System.Configuration;
using System.Net.Http;
using emri_service.Auth;
using emri_service.RabbitMQSetup;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.ConstrainedExecution;
using System;
using MassTransit;
using emri_repository;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using emri_service.Managers;
using DddsUtils.Data.Context;

namespace emri_service
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddDbContext<TodoContext>(opt =>
			//	opt.UseInMemoryDatabase("TodoList"));
			services.AddControllers();
			services.AddHttpClient();
			var model = Configuration.GetSection("RabbitMQModel");
			var primary = new RabbitMQModel { URI = Configuration["RabbitMQModel:URI"], CertificateName = Configuration["RabbitMQModel:CertificateName"] };
			var cert = LoadClientCertificate(primary);

			services.ConfigureRabbitMQBus(primary.URI, cert);
			// Dependency injection
			services.AddMassTransitHostedService();
			services.AddScoped<ILogFactory, LogFactory>();
			services.AddScoped<IMRRTokenManager, MRRTokenManager>();
			services.AddScoped<IDbConnection, SqlConnection>();
			services.AddScoped<IDapperContext, DapperContext>();
			services.AddScoped<ICDIAlertManager, CDIAlertMananger>();
			services.AddScoped<IEMRTrackingRepository, EMRTrackingRepository>();
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "emri-service", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UsePathBase(new PathString("/emr/[client]/[lob]/v1"));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseEnableRequestBuffering();
			//app.UseAuditLog();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("v1/swagger.json", "emri-service V1");
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private X509Certificate2 LoadClientCertificate(RabbitMQModel model)
		{
			var certificate = Get509Certificate(model.CertificateName);
			return new X509Certificate2(certificate.Export(X509ContentType.Cert));
		}

		private X509Certificate Get509Certificate(string name)
		{
			var certificateStore = new X509Store(StoreLocation.LocalMachine);
			certificateStore.Open(OpenFlags.OpenExistingOnly);
			var certificateCollection = certificateStore.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, name, true);
			if (certificateCollection != null && certificateCollection.Count == 0)
			{
				certificateCollection = certificateStore.Certificates.Find(X509FindType.FindBySubjectName, name, true);
			}
			var certificate = certificateCollection[0];
			return certificate;
		}
	}
}
