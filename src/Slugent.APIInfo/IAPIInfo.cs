using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.APIInfo
{
	public interface IAPIInfoBase
	{
		public string InfoRootPath { get; set; }

		public void AddConfigMatchCriteria (ConfigMatchCriteria configMatchCriteria);

		public void AddConfigMatchCriteria (string keyword, bool isCaseMatch = false, bool isFullMatch = true);

		public bool DoesConfigEntryMatch (string configKey);
	}
}
