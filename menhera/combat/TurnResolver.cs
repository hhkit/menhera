using System.Collections;
using System.Diagnostics;

namespace menhera
{
    public class TurnResolver(List<Clash> clashes, ServiceLocator services)
    {
        readonly MessagingService messagingService = services.GetService<MessagingService>();

        public IEnumerable Resolve()
        {
            messagingService.BroadcastEvent(new OnCombatStart());

            foreach (var clash in clashes)
            {
                if (clash.Player.Combatant.IsDead || clash.Enemy.Combatant.IsDead)
                    continue;

                AttackResolver attackResolver;
                if (clash.Player.CanFight && clash.Enemy.CanFight)
                {
                    var resolver = new ClashResolver(clash, services);
                    var clashRes = resolver.ResolveClash();
                    Debug.Assert(clashRes != ClashResolver.ClashResult.Tie);
                    yield return null;
                }

                if (clash.Player.Combatant.IsAlive && clash.Enemy.Combatant.IsAlive)
                {
                    if (clash.Player.CanFight)
                    {
                        attackResolver = new AttackResolver(clash.Player, clash.Enemy, services);
                    }
                    else
                    {
                        Debug.Assert(clash.Enemy.CanFight);
                        attackResolver = new AttackResolver(clash.Enemy, clash.Player, services);
                    }

                    attackResolver.Resolve();
                    // check for death
                    yield return null;
                }
            }
            messagingService.BroadcastEvent(new OnCombatEnd());
        }
    }
}