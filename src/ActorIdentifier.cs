namespace menhera;

public class ActorIdentifier
{
    public int id { get; private set; }
    public int team { get; private set; }
    public long flag { get => 1L << id; }

    public ActorIdentifier(int id, int team)
    {
        this.id = id;
        this.team = team;
    }
}