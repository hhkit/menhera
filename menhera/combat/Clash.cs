namespace menhera
{
    // this stores the data for a single combatant during a clash
    public class ClashingCombatant(CombatActor combatant, SkillData? skill)
    {
        public CombatActor Combatant { get; private set; } = combatant;
        public SkillData? Skill { get; private set; } = skill;
        public int BrokenCoins { get; set; } = 0;
        public int CoinsLeft { get => Skill.HasValue ? Skill.Value.CoinCount - BrokenCoins : 0; }
        public bool CanFight { get => Skill.HasValue && CoinsLeft > 0; }
    }

    public class Clash(CombatActor player, SkillData playerSkill, CombatActor enemy, SkillData enemySkill)
    {
        public ClashingCombatant Player { get; private set; } = new(player, playerSkill);
        public ClashingCombatant Enemy { get; private set; } = new(enemy, enemySkill);
    }
}