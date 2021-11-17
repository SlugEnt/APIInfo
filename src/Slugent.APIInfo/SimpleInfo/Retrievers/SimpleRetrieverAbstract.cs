using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;

[assembly: InternalsVisibleTo("Test.APIInfo")]

namespace SlugEnt.APIInfo
{
	public abstract class SimpleRetrieverAbstract
	{
		protected Dictionary<string, string> _results = new();

		public Dictionary<string,string> Results { get { return _results; } }


		/// <summary>
		/// If true the retriever can provide full html result of its data.  
		/// </summary>
		public bool SupportsHTML { get; set; }


		/// <summary>
		///  Title to be displayed on the info page.
		/// </summary>
		public string Title { get; set; }


		/// <summary>
		/// The order that this SimpleInfo object should be displayed in.  If ties, on number, then it goes by Title.
		/// </summary>
		public int SortedOrderValue { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="title"></param>
		public SimpleRetrieverAbstract (string title, short sortValue) { 
			Title = title;
			SortedOrderValue = sortValue;
		}



		/// <summary>
		/// Returns the Style element that needs to be placed into the header so the table is formatted correctly.
		/// </summary>
		/// <returns></returns>
		public static string GetHTMLStyle () {
			string styleHTML = @"
<style>
table {
border-spacing: 25px;
}
</style>
";
			return styleHTML;
		}



		/// <summary>
		/// Returns the data as an HTML string.  This method must be override if the derived class wishes to provide customized HTML
		/// </summary>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public StringBuilder ProvideHTML()
		{
			string className = this.GetType().FullName;

			GatherData();

			// Now format as HTML
			StringBuilder html = ToHtmlTable(_results);
			return html;
		}


		/// <summary>
		/// Returns a dictionary of all the key host elements
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> ProvideDictionary()
		{
			GatherData();
			return _results;
		}


		/// <summary>
		/// Takes an enumerable and converts it to an HTML Table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enums"></param>
		/// <returns></returns>
		protected StringBuilder ToHtmlTable<T>(IEnumerable<T> enums)
		{
			var type = typeof(T);
			var props = type.GetProperties();


			// Set CSS
			StringBuilder html = new StringBuilder();
			
			//html.Append(header);

			html.Append("<h1>" + Title + "</h1>");

			html.Append("<table>");
			

			//Header
			html.Append("<thead><tr>");
			if ( props.Length == 2 ) 
				html.Append("<th>Item</th><th>Value</th>");
			else if ( props.Length == 1 ) html.Append("<th>Item</th>");
			/* Do not delete - good example for converting lists, etc to tables
						foreach (var p in props)
							html.Append("<th>" + p.Name + "</th>");
						html.Append("</tr></thead>");
			*/
			html.Append("</tr></thead>");

			//Body
			html.Append("<tbody>");
			foreach (var e in enums)
			{
				html.Append("<tr>");
				props.Select(s => s.GetValue(e)).ToList().ForEach(p => {
					html.Append("<td>" + p + "</td>");
				});
				html.Append("</tr>");

			}

			html.Append("</tbody></table>");
			//html.Append(ending);
			return html;
		}

		protected abstract void GatherData ();
	}

}
