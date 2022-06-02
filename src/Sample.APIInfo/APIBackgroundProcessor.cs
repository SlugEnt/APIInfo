using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.APIInfo.HealthInfo;

namespace Sample.APIInfo
{
	public class APIBackgroundProcessor : BackgroundService {
		private ILogger<APIBackgroundProcessor> _logger;
		private IServiceProvider                _serviceProvider;
		private HealthCheckProcessor            _healthCheckProcessor;

		public APIBackgroundProcessor (ILogger<APIBackgroundProcessor> logger, IServiceProvider serviceProvider) {
			_logger = logger;
			_serviceProvider = serviceProvider;
			_healthCheckProcessor = _serviceProvider.GetService<HealthCheckProcessor>();
		}


		protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
			stoppingToken.Register(StopService);

			TimeSpan sleepTime = TimeSpan.FromSeconds(5);
			while ( !stoppingToken.IsCancellationRequested ) {
				_healthCheckProcessor.CheckHealth();
				await Task.Delay(sleepTime, stoppingToken);
				Console.WriteLine("Overall Health Check Status: " + _healthCheckProcessor.Status.ToString());
			}
			
		}


		private void StopService () {

		}
	}
}
