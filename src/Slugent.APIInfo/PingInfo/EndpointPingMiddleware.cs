using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace SlugEnt.APIInfo
{
	/// <summary>
	/// The actual code executed when the PingEndpoint is called.
	/// </summary>
	public class EndpointPingMiddleware
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EndpointPingMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		public EndpointPingMiddleware(RequestDelegate next) { }

		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
		public async Task InvokeAsync(HttpContext httpContext)
		{
			await httpContext.Response.WriteAsync("pong");
		}
	}
}
