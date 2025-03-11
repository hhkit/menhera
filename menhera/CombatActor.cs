namespace menhera
{
    // This class stores the battle instance data of the combatant
    public class CombatActor
    {
        public CharacterData characterData;
        public int hp;
        public int sanity;
        public List<Status> statuses = [];

        public bool IsAlive { get => hp > 0; }
        public bool IsDead { get => !IsAlive; }
    }
}