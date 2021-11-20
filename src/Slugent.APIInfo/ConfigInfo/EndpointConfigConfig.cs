namespace SlugEnt.APIInfo
{
	/// <summary>
	/// The configuration settings for the /config endpoint
	/// </summary>
	public class EndpointConfigConfig
	{
		/// <summary>
		/// Determines if this endpoint should be shown.
		/// </summary>
		public bool Enabled { get; set; } = true;
	}
}
