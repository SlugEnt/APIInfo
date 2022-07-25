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
			SimpleRetrieverHostInfo hostInfo = new();
			hostInfo.ProvideDictionary();
			Assert.GreaterOrEqual(hostInfo.Results.Count, 2,"A10: Dictionary has incorrect number of items");
			

			Assert.Pass();
		}
	}
}