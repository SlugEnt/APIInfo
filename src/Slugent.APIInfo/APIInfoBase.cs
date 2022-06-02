using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace SlugEnt.APIInfo
{
	/// <summary>
	/// Adds an endpoint to the API that allow it to report on certain information about the API.
	/// Endpoint default is /info
	/// </summary>
	public class APIInfoBase : IAPIInfoBase {
		private const string _ROOT_PATH = "info";

		private static string _rootPath = _ROOT_PATH;


		/// <summary>
		/// Dictionary containing all configuration match criteria that results in the config value not being displayed.
		/// </summary>
		private Dictionary<string, ConfigMatchCriteria> _hiddenConfigEntries = new();

		/// <summary>
		/// Config entries - exact string matches that override a hidden value
		/// </summary>
		private Dictionary<string, string> _hiddenConfigOverrides = new Dictionary<string, string>();


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
		public void AddConfigHideCriteria (ConfigMatchCriteria configMatchCriteria) {
			_hiddenConfigEntries.Add(configMatchCriteria.Keyword,configMatchCriteria);
		}


		/// <summary>
		/// Add the given ConfigMatchCriteria to the list of Match Criteria that is used when displaying config values
		/// </summary>
		/// <param name="keyword">The text to match against</param>
		/// <param name="isCaseMatch">If true then the case of the keyword and the case of the compared text must be exact</param>
		/// <param name="isFullMatch">If true then the entire keyword must match the entire compared text.</param>
		public void AddConfigHideCriteria (string keyword, bool isCaseMatch = false, bool isFullMatch = true) {
		ConfigMatchCriteria configMatchCriteria = new (keyword, isCaseMatch, isFullMatch);
			AddConfigHideCriteria(configMatchCriteria);
		}


		/// <summary>
		/// Adds the given keyword to the dictionary of config overrides.  Any Config entry that exactly matches the keyword will be displayed.
		/// </summary>
		/// <param name="keyword"></param>
		public void AddConfigOverrideString (string keyword) {
			_hiddenConfigOverrides.Add(keyword,keyword);
		}


		/// <summary>
		/// Determines if the given keyword exists in the ConfigMatchCriteria Hidden list and returns true or false
		/// </summary>
		/// <param name="configKey">the string Configuration values we want to match against</param>
		/// <returns></returns>
		protected bool DoesConfigEntryMatchHidden (string configKey) {
			foreach ( KeyValuePair<string, ConfigMatchCriteria> matchCriteria in _hiddenConfigEntries ) {
				if ( matchCriteria.Value.IsMatch(configKey) ) return true;
			}

			return false;
		}


		/// <summary>
		/// Returns true if the specified configKey should be displayed.  Returns false if it should not be displayed based upon Config Criteria previously entered.
		/// </summary>
		/// <param name="configKey"></param>
		/// <returns></returns>
		public bool ShouldShowConfigKeyWord (string configKey) {
			// 1st determine if it is set to be hidden.
			bool isHidden = DoesConfigEntryMatchHidden(configKey);
			if ( isHidden ) {
				// See if it has an exact match override
				if ( _hiddenConfigOverrides.ContainsKey(configKey) ) return true;

				return false;
			}

			// It's not specifically hidden so show it.
			return true;
		}
	}
}
