namespace SlugEnt.APIInfo
{
	/// <summary>
	/// Represents a set of text to be matched against and indicates criteria for the match to be compared against, included exact text and matching case.
	/// </summary>
	public class ConfigMatchCriteria
	{
		/// <summary>
		/// The text to match against
		/// </summary>
		public string Keyword { get; set; }

		/// <summary>
		/// If true then the case of the keyword and the case of the compared text must be exact
		/// </summary>
		public bool IsCaseMatch { get; set; }


		/// <summary>
		/// If true then the entire keyword must match the entire compared text.
		/// </summary>
		public bool IsFullMatch { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="keyword">The text to match against</param>
		/// <param name="isCaseMatch">If true then the case of the keyword and the case of the compared text must be exact</param>
		/// <param name="isFullMatch">If true then the entire keyword must match the entire compared text.</param>
		public ConfigMatchCriteria (string keyword, bool isCaseMatch = true, bool isFullMatch = true) {
			Keyword = keyword;
			IsCaseMatch = isCaseMatch;	
			IsFullMatch = isFullMatch;
		}


		/// <summary>
		/// Compares the provided key text with the Keyword of this object.  If they match based upon the settings of Case and FullMatch  then returns true, otherwise false
		/// </summary>
		/// <param name="otherKey">The text to compare the Keyword to based upon matching criteria.</param>
		/// <returns></returns>
		public bool IsMatch (string otherKey = "") {
			// Convert to lower case if case does not matter
			if ( !IsCaseMatch ) otherKey = otherKey.ToLower();

			if ( IsFullMatch ) return Keyword == otherKey;

			return (otherKey.Contains(Keyword));
		}
	}
}
