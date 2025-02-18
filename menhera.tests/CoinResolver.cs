namespace menhera.tests
{
    [TestClass]
    public sealed class TestCoinResolver
    {
        [TestMethod]
        public void TestMethod1()
        {
            var serviceLoc = ServiceLocator.Main;

            // set up character data


            // set up actor data
            var actorService = serviceLoc.GetService<ActorService>();
        }
    }
}