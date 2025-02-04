namespace menhera
{

    public class RandomNumberService : Service
    {
        Random rng;
        public RandomNumberService()
        {
            rng = new();
        }
        public RandomNumberService(int seed)
        {
            rng = new(seed);
        }
        public int NextInt(int exclusiveMax, int min = 0)
        {
            var diff = exclusiveMax - min;
            return (int)rng.NextInt64(diff) + min;
        }
    }
}