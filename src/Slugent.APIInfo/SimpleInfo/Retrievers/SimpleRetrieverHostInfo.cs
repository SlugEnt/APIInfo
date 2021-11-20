using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;



namespace SlugEnt.APIInfo
{
	

	/// <summary>
	/// Retrieves basic host Information 
	/// </summary>
	public class SimpleRetrieverHostInfo : SimpleRetrieverAbstract,ISimpleInfoRetriever {
		/// <summary>
		/// Title for this SimpleRetriever
		/// </summary>
		public const string TITLE_HOST_INFO = "Host Information";

		/// <summary>
		/// The relative sort order for this SimpleRetriever
		/// </summary>
		public const short SORT_VALUE = 1000;
		


		/// <summary>
		/// Constructs a Host Info retriever
		/// </summary>
		public SimpleRetrieverHostInfo () : base (TITLE_HOST_INFO, SORT_VALUE) { }



		/// <summary>
		/// Retrieves all the data we provide.
		/// </summary>
		protected override void GatherData () {
			_results.Add("Hostname",GetHostName());

			List<IPAddress> ipAddresses = GetIPAddresses();
			int counter = 1;
			foreach ( IPAddress ipAddress in ipAddresses ) {
				_results.Add("IP Adddress #" + counter++ , ipAddress.ToString());
			}
		}


		/// <summary>
		/// Retrieves FQDN of the host running on.
		/// </summary>
		/// <returns></returns>
		internal string GetHostName () {
			string domainName = "." + IPGlobalProperties.GetIPGlobalProperties().DomainName;
			string host = Dns.GetHostName();

			if ( !host.EndsWith(domainName) ) host += domainName;
			return host;
		}


		internal List<IPAddress> GetIPAddresses () {
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			return host.AddressList.ToList();
		}
	}
}
