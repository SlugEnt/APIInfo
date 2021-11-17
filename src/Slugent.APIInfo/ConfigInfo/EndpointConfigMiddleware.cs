﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.APIInfo
{
	/// <summary>
	
	/// </summary>
	public class EndpointConfigMiddleware {

		private IConfigurationRoot _configRoot;
		private IAPIInfoBase _apiInfoBase;


		/// <summary>
		/// Initializes a new instance of <see cref="EndpointSimpleMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		public EndpointConfigMiddleware (RequestDelegate next, IConfiguration configuration, IAPIInfoBase apiInfoBase) {
			_configRoot = configuration as ConfigurationRoot;
			_apiInfoBase = apiInfoBase;
		}


		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
		public async Task InvokeAsync(HttpContext httpContext, IConfiguration configuration, IAPIInfoBase apiInfoBase) {
			ConfigurationParser parser = new ConfigurationParser(_configRoot,_apiInfoBase);
			string returnHtml = parser.DisplayConfig();
			await httpContext.Response.WriteAsync(returnHtml);
		}


	}
}
