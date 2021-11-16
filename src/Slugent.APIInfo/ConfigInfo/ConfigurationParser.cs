using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.APIInfo
{
	public class ConfigurationParser
	{
		private IConfigurationRoot _configRoot;
		private StringBuilder _htmlStringBuilder = new StringBuilder(4096);
		private short _indentLevel = -1;

		internal string DisplayConfig(IConfiguration config)
		{
			_configRoot = config as IConfigurationRoot;
			RecurseChildren(_configRoot.GetChildren());

			StringBuilder finalHtml = new StringBuilder(_htmlStringBuilder.Length + 300);
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


		internal void RecurseChildren(IEnumerable<IConfigurationSection> children) {
			_indentLevel++;

			IEnumerable<IConfigurationSection> sortedChildren = children.OrderByDescending(c => c.Value);
			foreach (IConfigurationSection child in sortedChildren)
			{
				// Get the child as well as all providers for whom the child key was overridden
				Stack<(string Value, IConfigurationProvider Provider)> childStack = GetChildrenAndOverwrites(_configRoot, child.Path);

				// Is a child section
				if (childStack.Count == 0)
				{
					int spaceCount = (_indentLevel+1) * 2;
					string indentString = "";
					if (spaceCount>0) indentString = new string(' ', spaceCount);
					string rowString = indentString + "|--> " + child.Key + ":";
					_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");
				}

				// Is a child value
				else
				{
					var childValue = childStack.Pop();

					// Write out this entry which is the current value for the key.
					int spaceCount = (_indentLevel+1) * 2;
					string indentString = "";
					if (spaceCount > 0) indentString = new string(' ', spaceCount);

					// If there are overridden values then we list ALL the values on separate lines
					string rowString = indentString + "|--: " + child.Key + ":  ";
					if ( childStack.Count == 0 ) {
						rowString += childValue.Value + "  [ " + childValue.Provider + " ]";
						_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");
					}
					else {
						// Write the config Key out on line, then write all the subsequent values
						_htmlStringBuilder.Append("<pre>" + rowString + "</pre>");

						// Write current value
						int newIndent = _indentLevel + 2;
						indentString = new string(' ', newIndent*2);
						rowString = "<pre>" + indentString + "  ** " + childValue.Value + "  [ " + childValue.Provider + " ] </pre>";
						_htmlStringBuilder.Append( rowString);

						// Loop thru all the over-ridden values of this key.
						foreach (var overriddenValue in childStack)
						{
							int childSpaceCount = (_indentLevel + 1) * 2;
							rowString = "<pre>" + indentString + "  -- " + overriddenValue.Value + "  [ " + overriddenValue.Provider + " ] </pre>";
							_htmlStringBuilder.Append(rowString);
						}
					}
				}
				RecurseChildren(child.GetChildren());
				_indentLevel--;
			}
			
		}

		internal static Stack<(string Value, IConfigurationProvider Provider)> GetChildrenAndOverwrites(
			IConfigurationRoot root,
			string key)
		{
			var stack = new Stack<(string, IConfigurationProvider)>();
			foreach (IConfigurationProvider provider in root.Providers)
			{
				if (provider.TryGet(key, out string value))
				{
					stack.Push((value, provider));
				}
			}

			return stack;
		}

	}


}
