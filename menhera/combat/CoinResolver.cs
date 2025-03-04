namespace menhera
{
    public class CoinResolver(CombatActor combatant, ServiceLocator services)
    {
        private readonly CombatActor combatant = combatant;
        private readonly ServiceLocator services = services;

        public bool[] FlipCoins(int coinCount)
        {
            var results = new bool[coinCount];
            for (var i = 0; i < coinCount; ++i)
                results[i] = FlipCoin();
            return results;
        }

        public bool FlipCoin()
        {
            var rng = services.GetService<RandomNumberService>();
            return combatant.sanity + 50 < rng.NextInt(100);
        }
    }
}