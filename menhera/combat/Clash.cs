namespace menhera
{    // this stores the data for a single combatant during a clash
    public struct ClashingCombatant(CombatActor combatant, SkillData skill)
    {
        public CombatActor Combatant { get; private set; } = combatant;
        public SkillData Skill { get; private set; } = skill;
        public int brokenCoins = 0;
        public int coinsLeft { get => Skill.CoinCount - brokenCoins; }
    }

    public class Clash(CombatActor player, SkillData playerSkill, CombatActor enemy, SkillData enemySkill)
    {
        public ClashingCombatant player = new(player, playerSkill);
        public ClashingCombatant enemy = new(enemy, enemySkill);
    }
}