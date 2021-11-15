using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using SlugEnt.APIInfo;




namespace Test_Retrievers
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void SimpleInfoHost() {
			SimpleRetrieverHostInfo hostInfo = new SimpleRetrieverHostInfo();
			hostInfo.ProvideDictionary();
			Assert.AreEqual(2,hostInfo.Results.Count, "A10: Dictionary has incorrect number of items");
			

			Assert.Pass();
		}
	}
}