namespace menhera.tests
{
    public class AlwaysMaxRandomNumberService : RandomNumberService
    {
        public override int NextInt(int exclusiveMax, int min)
        {
            return exclusiveMax - 1;
        }
    }


    public class AlwaysMinRandomNumberService : RandomNumberService
    {
        public override int NextInt(int exclusiveMax, int min)
        {
            return min;
        }
    }
}