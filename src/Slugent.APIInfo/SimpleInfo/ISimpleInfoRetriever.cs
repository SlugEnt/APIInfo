using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slugent.APIInfo.SimpleInfo
{
	public interface ISimpleInfoRetriever {
		/// <summary>
		/// If the class is able to provide formatted html of its data.
		/// </summary>
		public bool SupportsHTML { get; set; }


		/// <summary>
		/// The order this item should be sorted in
		/// </summary>
		public int SortedOrderValue { get; set; }

		/// <summary>
		/// Title of this Retreiver
		/// </summary>
		public string Title { get; set; }


		/// <summary>
		/// Provider called to retrieve the HTML Data.
		/// </summary>
		/// <returns></returns>
		public string ProvideHTML ();


		/// <summary>
		/// Provider called to retrieve a dictionary of the keys and data
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> ProvideDictionary ();
	}
}
