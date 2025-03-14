namespace menhera.tests
{
    [TestClass]
    public sealed class TestCoinResolver
    {
        [TestMethod]
        public void TestFlipCoin()
        {
            {
                ServiceLocator services = new(new AlwaysMaxRandomNumberService());
                CombatActor combatActor = new()
                {
                    sanity = -45
                };

                CoinResolver resolver = new(combatActor, services);
                Assert.IsTrue(resolver.FlipCoin(), "High roll should always return heads");
            }

            {
                ServiceLocator services = new(new AlwaysMinRandomNumberService());
                CombatActor combatActor = new()
                {
                    sanity = +45
                };

                CoinResolver resolver = new(combatActor, services);
                Assert.IsFalse(resolver.FlipCoin(), "Low roll should always return tails");
            }
        }

        [TestMethod]
        public void TestFlipCoins()
        {
            {
                ServiceLocator services = new(new AlwaysMaxRandomNumberService());
                CombatActor combatActor = new()
                {
                    sanity = -45
                };

                CoinResolver resolver = new(combatActor, services);
                Assert.IsTrue(resolver.FlipCoins(100).All(x => x), "High roll should always return heads");
            }

            {
                ServiceLocator services = new(new AlwaysMinRandomNumberService());
                CombatActor combatActor = new()
                {
                    sanity = +45
                };

                CoinResolver resolver = new(combatActor, services);
                Assert.IsTrue(resolver.FlipCoins(100).All(x => !x), "Low roll should always return tails");
            }
        }
    }
}