using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SlugEnt.APIInfo;

namespace Test_Retrievers
{

	public class ConfigMiddlewareTests
	{
		[SetUp]
		public void Setup()
		{

		}


		public void SetupConfiguration (ConfigurationBuilder config) {
			// We hard code some configuration data:
			//  - Change Management Actuator endpoint to Info
			//  - Remove sensitive environment variables from the display.
			Dictionary<string, string> customConfig = new Dictionary<string, string>()
			{
				{"AppA:ChildAA:valueA", "abcdef" },
				{"AppA:ChildAA:valueB","xyz222"},
				{"AppA:ChildAB:valueC","abc3"},
				{"AppA:ChildAB:ChildABC:valueD","valD"},
				{"AppB:ChildBA:ChildBAA:ChildBAAA:ValueE","valueEEEE"},
				{"AppC:ChildCA:valueCAA","caa"},
				{"AppC:ChildCA:ChildCAB:valuecaba","caba"},
				{"AppD:child1:grandchild11:somethin","somre"},
			};
			config.AddInMemoryCollection(customConfig).Build();
		}


		public void OverrideConfiguration(ConfigurationBuilder config) {
			// We hard code some configuration data:
			//  - Change Management Actuator endpoint to Info
			//  - Remove sensitive environment variables from the display.
			Dictionary<string, string> customConfig = new Dictionary<string, string>()
			{
				{"AppA:ChildAB:valueC","xyz6"},
				{"AppB:ChildBA:ChildBAA:ChildBAAA:ValueE","Ooops - just one E"},
				{"AppD:child1:grandchild11:somethin","my favorite grand kid"},
			};

			config.AddInMemoryCollection(customConfig);
		}


		[Test]
		public void NoOverrides() {
			ConfigurationBuilder builder = new ConfigurationBuilder();

			SetupConfiguration(builder);
			OverrideConfiguration(builder);

			IConfiguration config = builder.Build();
			
			APIInfoBase apiInfoBase = new();
			ConfigurationParser parser = new(config,apiInfoBase);

			string returnHtml = parser.DisplayConfig();
			SimpleRetrieverHostInfo hostInfo = new SimpleRetrieverHostInfo();


			hostInfo.ProvideDictionary();
			Assert.AreEqual(2, hostInfo.Results.Count, "A10: Dictionary has incorrect number of items");


			Assert.Pass();
		}
	}
}

