using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.APIInfo.ConfigInfo
{
	/// <summary>
	/// Some of this code is from an Andrew Lock article:  https://andrewlock.net/viewing-overriden-configuration-values-in-aspnetcore/
	/// </summary>
	public class EndpointConfigMiddleware
	{
		/// <summary>
		/// Initializes a new instance of <see cref="EndpointSimpleMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		public EndpointConfigMiddleware(RequestDelegate next) { }


		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
		public async Task InvokeAsync(HttpContext httpContext)
		{

			await httpContext.Response.WriteAsync("Not Implemented Yet");
		}


		internal void DisplayConfig (IConfiguration config) {
			IConfiguration rootConfig = config;

			
		}


		internal void RecurseChildren (IEnumerable<IConfigurationSection> children) {

		}

		internal static Stack<(string Value, IConfigurationProvider Provider)> GetValueAndProviders(
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
