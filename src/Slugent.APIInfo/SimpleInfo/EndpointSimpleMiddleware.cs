using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace SlugEnt.APIInfo
{
	/// <summary>
	/// The actual code executed when the PingEndpoint is called.
	/// </summary>
	public class EndpointSimpleMiddleware
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EndpointSimpleMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		public EndpointSimpleMiddleware(RequestDelegate next) { }


		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
		/// <param name="simpleRetrievers">List of simple retriever objects that will be used to build the /simple page</param>
		public async Task InvokeAsync(HttpContext httpContext, IEnumerable<ISimpleInfoRetriever> simpleRetrievers)
		{
			
			StringBuilder sb = new(4096);

			// Get the retrievers in sorted order:
			IEnumerable<ISimpleInfoRetriever> sorted = simpleRetrievers.OrderBy(ISimpleInfoRetriever => ISimpleInfoRetriever.SortedOrderValue)
			                                                           .ThenBy(ISimpleInfoRetriever => ISimpleInfoRetriever.Title);

			// Create the HTML Page start and header.
			sb.Append("<html><head>");
			sb.Append(SimpleRetrieverAbstract.GetHTMLStyle());
			sb.Append("</head><body>");


			string ending = @"
</body>
</html>
";
			foreach ( ISimpleInfoRetriever retriever in sorted ) {
				sb.Append(retriever.ProvideHTML());
			}

			sb.Append(ending);
			string html = sb.ToString();
			await httpContext.Response.WriteAsync(html);
		}
	}
}
