using NUnit.Framework;
using SlugEnt.APIInfo;



namespace Test_Retrievers
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }


        [Test]
        public void SimpleInfoHost()
        {
            SimpleRetrieverHostInfo hostInfo = new();
            hostInfo.ProvideDictionary();
            Assert.That(hostInfo.Results.Count, Is.GreaterThanOrEqualTo(2), "A10: Dictionary has incorrect number of items");


            Assert.Pass();
        }
    }
}