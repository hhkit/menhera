namespace menhera
{
    public class ActorIdentifier(int id, int team)
    {
        public int Id { get; private set; } = id;
        public int Team { get; private set; } = team;
        public long Flag { get => 1L << Id; }
    }
}