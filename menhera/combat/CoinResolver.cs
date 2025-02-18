namespace menhera
{
    public class CoinResolver(CombatActor combatant, ServiceLocator services)
    {
        private readonly CombatActor combatant = combatant;
        private readonly ServiceLocator services = services;

        public CoinResolver(CombatActor combatant)
        : this(combatant, ServiceLocator.Main)
        {
        }


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
            return rng.NextInt(100) < combatant.sanity + 50;
        }
    }
}