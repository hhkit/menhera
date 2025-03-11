namespace menhera
{
    public struct ActorIdentifier(int id, int team)
    {
        public int Id { get; private set; } = id;
        public int Team { get; private set; } = team;
        public readonly long Flag { get => 1L << Id; }

        public static bool operator ==(ActorIdentifier c1, ActorIdentifier c2)
        {
            return c1.Id == c2.Id && c1.Team == c2.Team;
        }

        public static bool operator !=(ActorIdentifier c1, ActorIdentifier c2)
        {
            return !(c1 == c2);
        }

        public override readonly bool Equals(object? obj) => obj is ActorIdentifier other && Equals(other);

        public readonly bool Equals(ActorIdentifier obj)
        {
            return this == obj;
        }

        public override readonly int GetHashCode()
        {
            return Id >> 7 & Team << 18;
        }
    }


    public class ActorService(TeamManager teamManager) : Service
    {
        readonly Dictionary<CombatActor, ActorIdentifier> lookupTable = [];

        public ActorIdentifier GetId(CombatActor combatant)
        {
            if (lookupTable.TryGetValue(combatant, out var value))
                return value;

            throw new KeyNotFoundException();
        }

        public ActorIdentifier Register(CombatActor combatant, int team)
        {
            var newId = new ActorIdentifier(lookupTable.Count, team);
            teamManager.Join(newId, team);
            lookupTable.Add(combatant, newId);
            return newId;
        }
    }
}