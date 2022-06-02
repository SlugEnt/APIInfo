using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.APIInfo.HealthInfo
{
	public class IHealthCheckConfig
	{
		/// <summary>
		/// How often the Check should be performed in seconds
		/// </summary>
		public int CheckIntervalSeconds { get; set; }
	}
}
