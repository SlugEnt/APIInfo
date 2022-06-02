using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.APIInfo.HealthInfo
{
	public interface IHealthChecker {
		/// <summary>
		/// Name of this health checker
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of Health Check this is
		/// </summary>
		public EnumHealthCheckerType HealthCheckerType { get; set; }

		/// <summary>
		/// The configuration for this Health Checker
		/// </summary>
		public IHealthCheckConfig Config { get; set; }

		public EnumHealthStatus Status { get; }

		public DateTimeOffset LastStatusCheck { get; }

		public DateTimeOffset NextStatusCheck { get; }

		public List<HealthEntryRecord> HealthEntries { get;  }

		/// <summary>
		/// Returns true if the Health Checker is currently running
		/// </summary>
		public bool IsRunning { get; }


		public string CheckerName { get; set; }


		/// <summary>
		/// Performs the health check specific to the given HealthChecker.  
		/// </summary>
		/// <returns></returns>
		public void CheckHealth (bool force = false);

		public void DisplayHTML (StringBuilder sb);
	}
}
