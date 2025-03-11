using System.Diagnostics;

namespace menhera
{
    public class AttackResolver(ClashingCombatant attacker, ClashingCombatant defender, ServiceLocator services)
    {
        public void Resolve()
        {
            Debug.Assert(attacker.Skill.HasValue);
            var attackingCoinResolver = new CoinResolver(attacker.Combatant, services);
            var results = attackingCoinResolver.FlipCoins(attacker.CoinsLeft);

        }
    }
}