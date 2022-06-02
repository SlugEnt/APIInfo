using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.APIInfo {
	/// <summary>
	///  This class evaluates all the IConfiguration items from the Microsoft Configuration object and formats it into an HTML output.
	///  <para>
	///   It sorts the configuration objects by name with the exception that childen values that are also parents of other
	///   values are listed first then children that are just values.
	///  </para>
	///  <para>
	///   It will accept a list of keywords that should be hidden from view and never presented in the output list.  This
	///   can be a partial or exact match.
	///  </para>
	///  Some of this code is inspired from an Andrew Lock article:
	///  https://andrewlock.net/viewing-overriden-configuration-values-in-aspnetcore/
	/// </summary>
	public class ConfigurationParser {
		private const    string       _HIDDEN = "**------------- HIDDEN VALUE ------------------------------**";
		private readonly IAPIInfoBase _apiInfoBase;

		private readonly IConfigurationRoot _configRoot;
		private readonly StringBuilder      _htmlStringBuilder = new(4096);
		private          short              _hiddenConfigKeys;

		private short _hiddenConfigSections;
		private short _indentLevel = -1;


		/// <summary>
		///  Constructor
		/// </summary>
		public ConfigurationParser (IConfiguration configuration, IAPIInfoBase apiInfoBase) {
			_configRoot = configuration as ConfigurationRoot;
			_apiInfoBase = apiInfoBase;
		}


		/// <summary>
		///  Method that builds the HTML code that displays the list
		/// </summary>
		/// <returns></returns>
		internal string DisplayConfig () {
			RecurseChildren(_configRoot.GetChildren());

			StringBuilder finalHtml = new(_htmlStringBuilder.Length + 300);
			string header = @"
<html>
<head>
<style>
table {
  
  border-spacing: 25px;
}
</style>
</head>
<body>
";
			string trailer = @"
</body>
</html>";
			finalHtml.Append(header);
			finalHtml.Append(_htmlStringBuilder);
			finalHtml.Append(trailer);
			return finalHtml.ToString();
		}


		/// <summary>
		///  Identifies all occurrences of the given key in all the configuration providers.
		/// </summary>
		/// <param name="key">The key searching for</param>
		/// <returns></returns>
		internal Stack<(string Value, IConfigurationProvider Provider)> GetChildrenAndOverwrites (string key) {
			Stack<(string, IConfigurationProvider)> stack = new Stack<(string, IConfigurationProvider)>();
			foreach ( IConfigurationProvider provider in _configRoot.Providers ) {
				if ( provider.TryGet(key, out string value) ) stack.Push((value, provider));
			}

			return stack;
		}


		/// <summary>
		///  Recurses the given Configuration section listing all children and further recursing down into child Sections.
		/// </summary>
		/// <param name="children">The configuration section to parse into</param>
		/// <param name="hideValue">
		///  If true, then all config values, all config sections and all children keys will have their value hidden
		/// </param>
		internal void RecurseChildren (IEnumerable<IConfigurationSection> children, bool hideValue = false) {
			_indentLevel++;
			bool hideChildrenOfSection = false;

			// We sort the children in descending order by the value.  This way all child values are displayed first then all the sections.
			IEnumerable<IConfigurationSection> sortedChildren = children.OrderByDescending(c => c.Value).ThenBy(c => c.Key);
			foreach ( IConfigurationSection child in sortedChildren ) {
				bool hideChild = false;

				// Get the child as well as all providers for whom the child key was overridden
				Stack<(string Value, IConfigurationProvider Provider)> childStack = GetChildrenAndOverwrites(child.Path);


				// Need to go thru the config criteria we want to hide to see if the child matches.  If it does we skip it - including if it is a section match...
				if ( hideValue || !_apiInfoBase.ShouldShowConfigKeyWord(child.Key) ) {
					if ( childStack.Count == 0 )
						_hiddenConfigSections++;
					else
						_hiddenConfigKeys++;
					hideChild = true;
				}



				// Is a child section
				if ( childStack.Count == 0 ) {
					int spaceCount = (_indentLevel + 1) * 2;
					string indentString = "";
					if ( spaceCount > 0 ) indentString = new string(' ', spaceCount);
					string rowString = indentString + "|--> " + child.Key + ":";
					if ( hideChild ) {
						rowString += "  " + "----- Section Hidden -----";
						hideChildrenOfSection = true;
					}

					_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");
				}

				// Is a child value
				else {
					(string Value, IConfigurationProvider Provider) childValue = childStack.Pop();
					string configValue = hideChild ? _HIDDEN : childValue.Value;

					// Write out this entry which is the current value for the key.
					int spaceCount = (_indentLevel + 1) * 2;
					string indentString = "";
					if ( spaceCount > 0 ) indentString = new string(' ', spaceCount);

					// If there are overridden values then we list ALL the values on separate lines
					string rowString = indentString + "|--: " + child.Key + ":  ";
					if ( childStack.Count == 0 ) {
						rowString += configValue + "  [ " + childValue.Provider + " ]";
						_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");
					}
					else {
						// Write the config Key out on line, then write all the subsequent values
						_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");

						// Write current value
						int newIndent = _indentLevel + 2;
						indentString = new string(' ', newIndent * 2);
						rowString = "<pre>" + indentString + "  ** " + configValue + "  [ " + childValue.Provider + " ] </pre>";
						_htmlStringBuilder.Append(rowString);

						// Loop thru all the over-ridden values of this key.
						foreach ( (string Value, IConfigurationProvider Provider) overriddenValue in childStack ) {
							configValue = hideChild ? _HIDDEN : overriddenValue.Value;
							rowString = "<pre>" + indentString + "  -- " + configValue + "  [ " + overriddenValue.Provider + " ] </pre>";
							_htmlStringBuilder.Append(rowString);
						}
					}
				}

				RecurseChildren(child.GetChildren(), hideChildrenOfSection);
				_indentLevel--;
			}
		}
	}
}