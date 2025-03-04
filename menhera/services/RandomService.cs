namespace menhera
{

    public class RandomNumberService : Service
    {
        readonly Random rng;
        public RandomNumberService()
        {
            rng = new();
        }
        public RandomNumberService(int seed)
        {
            rng = new(seed);
        }
        public virtual int NextInt(int exclusiveMax, int min = 0)
        {
            var nextInt = (int)rng.NextInt64(min, exclusiveMax);
            return nextInt;
        }
    }
}