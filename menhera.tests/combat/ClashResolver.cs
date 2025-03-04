namespace menhera.tests
{
    [TestClass]
    public sealed class TestClashResolver
    {
        [TestMethod]
        public void TestClash()
        {
            ServiceLocator services = new();
            var actorService = services.GetService<ActorService>();
            var player = actorService.Register(new CombatActor()
            {
                sanity = 0,
            }, 0);

            var enemy = actorService.Register(new CombatActor()
            {
                sanity = 0,
            }, 1);
        }
    }
}