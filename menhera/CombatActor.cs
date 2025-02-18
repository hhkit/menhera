namespace menhera
{
    // This class stores the battle instance data of the combatant
    public class CombatActor
    {
        public CharacterData data;
        public int hp;
        public int maxHp;
        public int sanity;
        public List<Status> statuses = [];
    }
}