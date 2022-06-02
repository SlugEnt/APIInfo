namespace SlugEnt.APIInfo
{
	/// <summary>
	/// Interface for the APIInfoBase object
	/// </summary>
	public interface IAPIInfoBase
	{
		/// <summary>
		/// The root path of all of the endpoints provided by APIInfoBase. Default is /info
		/// </summary>
		public string InfoRootPath { get; set; }

		/// <summary>
		/// Adds the given ConfigMatchCriteria object to the list of criteria to evaluate config keys for hidding or concealing their values
		/// </summary>
		/// <param name="configMatchCriteria"></param>
		public void AddConfigHideCriteria (ConfigMatchCriteria configMatchCriteria);

		/// Adds the given attributes of a ConfigMatchCriteria object to the list of criteria to evaluate config keys for hidding or concealing their values
		public void AddConfigHideCriteria (string keyword, bool isCaseMatch = false, bool isFullMatch = true);


		/// <summary>
		/// Returns True if the config key provided should be shown in output.  False if it should not be shown.
		/// </summary>
		/// <param name="configKey"></param>
		/// <returns></returns>
		public bool ShouldShowConfigKeyWord (string configKey);
	}
}
