using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace SlugEnt.APIInfo.HealthInfo
{
	/// <summary>
	/// Manages all Health Checks 
	/// </summary>
	public class HealthCheckProcessor
	{
		private List<IHealthChecker>          _healthCheckerList;
		//private ILogger<HealthCheckProcessor> _logger;
		private ILogger<HealthCheckProcessor> _logger;
		private ILoggerFactory                _loggerFactory;

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="checkIntervalInSeconds"></param>
		public HealthCheckProcessor(ILogger<HealthCheckProcessor> logger)
		{
			_healthCheckerList = new List<IHealthChecker>();
			_logger = logger;
		}


		/// <summary>
		/// How often the check processor runs
		/// </summary>
		public int CheckInterval { get; set; }


		/// <summary>
		/// Adds a Health Check item
		/// </summary>
		/// <param name="healthChecker"></param>
		public void AddCheckItem (IHealthChecker healthChecker) {
			_healthCheckerList.Add(healthChecker);
			_logger.LogInformation("HealthChecker added: " + healthChecker.Name);
		}
		

		/// <summary>
		/// Performs all Health Checks 
		/// </summary>
		/// <returns></returns>
		public async Task CheckHealth () {
			Console.WriteLine("Starting Check Health cycle");
			foreach (var healthChecker in _healthCheckerList) 
				healthChecker.CheckHealth();
		}



		/// <summary>
		/// Returns the overall status of all the Health Checks.  The status returned will be the the most severe of all of the Health Checks.  So one service degraded will result in overall status of degraded.
		/// </summary>
		public EnumHealthStatus Status {
			get {
				EnumHealthStatus status = EnumHealthStatus.Healthy;
				foreach ( IHealthChecker healthChecker in _healthCheckerList ) {
					if ( healthChecker.Status > status ) status = healthChecker.Status;
				}

				return status;
			}
		}


		/// <summary>
		/// Starts the checking process
		/// </summary>
		public async Task Start () {
			
		}


		public StringBuilder Display () {
			StringBuilder sb = new StringBuilder(2048);
			sb.Append("<html>");
			foreach ( IHealthChecker healthChecker in _healthCheckerList ) {
				string color = "grey";
				if ( healthChecker.Status == EnumHealthStatus.Healthy ) color = "green";
				else if ( healthChecker.Status == EnumHealthStatus.Degraded ) color = "orange";
				else if ( healthChecker.Status == EnumHealthStatus.Failed ) color = "red";

				sb.Append("<H2 style=\"color:" + color + ";\">" + healthChecker.CheckerName + ": " + healthChecker.Name + "  Status: [" + healthChecker.Status.ToString() + "]");
				healthChecker.DisplayHTML(sb);
			}

			sb.Append("</html>");
			return sb;

		}
	}
}
