using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.APIInfo.HealthInfo
{
	/// <summary>
	/// Identifies the state of a given service or function
	/// </summary>
	public enum EnumHealthStatus
	{
		/// <summary>
		/// Service / API is Healthy and completely working
		/// </summary>
		Healthy = 0,

		/// <summary>
		/// No checks have been run, so the status is not yet known.
		/// </summary>
		Unknown = 100,

		/// <summary>
		/// Service is working suboptimally, Generally means it is accessible, but maybe permissions issues or something else is preventing a Healthy status
		/// </summary>
		Degraded = 200,

		/// <summary>
		/// Service is completely failed.
		/// </summary>
		Failed = 254,
	}
}
