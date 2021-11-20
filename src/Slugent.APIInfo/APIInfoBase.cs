using System;
using System.Collections.Generic;

namespace SlugEnt.APIInfo
{
	/// <summary>
	/// Adds endpoints to the API that allow it to report on certain information about the API.
	/// </summary>
	public class APIInfoBase : IAPIInfoBase {
		private const string _ROOT_PATH = "info";

		private static string _rootPath = _ROOT_PATH;


		/// <summary>
		/// Dictionary containing all configuration match criteria
		/// </summary>
		private Dictionary<string, ConfigMatchCriteria> _configMatches = new();


		/// <summary>
		/// The rootpath that the APIInfo endpoints start with.  By default this is /info.
		/// </summary>
		public string InfoRootPath {
			get => _rootPath;
			set {
				if ( _rootPath != _ROOT_PATH ) throw new ArgumentException("APIInfo Root path can only be set once during running of the application");
				_rootPath = value;
			}
		}


		/// <summary>
		/// Constructor accepting a string that allows you to define what the rootpath of the APIInfo endpoints is.  By default it is /info
		/// </summary>
		/// <param name="rootPath"></param>
		public APIInfoBase (string rootPath = "") {
			if ( rootPath != string.Empty ) _rootPath = rootPath;
		}



		/// <summary>
		/// Add the given ConfigMatchCriteria to the list of Match Criteria that is used when displaying config values
		/// </summary>
		/// <param name="configMatchCriteria">A single ConfigMatchCriteria object</param>
		public void AddConfigMatchCriteria (ConfigMatchCriteria configMatchCriteria) {
			_configMatches.Add(configMatchCriteria.Keyword,configMatchCriteria);
		}


		/// <summary>
		/// Add the given ConfigMatchCriteria to the list of Match Criteria that is used when displaying config values
		/// </summary>
		/// <param name="keyword">The text to match against</param>
		/// <param name="isCaseMatch">If true then the case of the keyword and the case of the compared text must be exact</param>
		/// <param name="isFullMatch">If true then the entire keyword must match the entire compared text.</param>
		public void AddConfigMatchCriteria (string keyword, bool isCaseMatch = false, bool isFullMatch = true) {
		ConfigMatchCriteria configMatchCriteria = new (keyword, isCaseMatch, isFullMatch);
			AddConfigMatchCriteria(configMatchCriteria);
		}


		/// <summary>
		/// Determines if the given keyword exists in the ConfigMatchCriteria list.
		/// </summary>
		/// <param name="configKey">the string Configuration values we want to match against</param>
		/// <returns></returns>
		public bool DoesConfigEntryMatch (string configKey) {
			foreach ( KeyValuePair<string, ConfigMatchCriteria> matchCriteria in _configMatches ) {
				if ( matchCriteria.Value.IsMatch(configKey) ) return true;
			}

			return false;
		}
	}
}
