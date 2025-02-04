namespace menhera
{
    public class CoinResolver(Combatant combatant)
    {
        private Combatant combatant  = combatant;
        private ServiceLocator services  = ServiceLocator.Main;

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