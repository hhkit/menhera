using System.Diagnostics;

namespace menhera
{


    public class ClashResolver(Clash clash)
    {
        public enum ClashResult
        {
            PlayerWin,
            Tie,
            EnemyWin
        }

        public readonly Clash clash = clash;
        private readonly ServiceLocator services = ServiceLocator.Main;

        private bool[] ResolveCoins(ClashingCombatant config)
        {
            var messagingService = services.GetService<MessagingService>();
            var actorService = services.GetService<ActorService>();
            var id = actorService.GetId(config.Combatant);

            var coinResolver = new CoinResolver(config.Combatant, services);
            var coinResults = coinResolver.FlipCoins(config.Skill.CoinCount - config.brokenCoins);
            messagingService?.BroadcastEvent(id, new OnCoinFlip(coinResults));

            foreach (var coinRes in coinResults)
                messagingService?.BroadcastEvent(id, coinRes ? new OnHeadsHit() : new OnTailsHit());

            return coinResults;
        }

        private int CalculateClashPower(SkillData skillData, bool[] coins)
        {
            return skillData.BasePower + coins.Select(x => x).Count() * skillData.CoinPower;
        }

        private int ResolveClashPower(ClashingCombatant combatant)
        {
            var coins = ResolveCoins(combatant);
            return CalculateClashPower(combatant.Skill, coins);
        }

        private ClashResult ResolveClashStep()
        {
            var playerClashPower = ResolveClashPower(clash.player);
            var enemyClashPower = ResolveClashPower(clash.enemy);

            if (playerClashPower == enemyClashPower)
                return ClashResult.Tie;

            return playerClashPower > enemyClashPower ? ClashResult.PlayerWin : ClashResult.EnemyWin;
        }

        public ClashResult ResolveClash()
        {
            var messagingService = services.GetService<MessagingService>();
            var actorService = services.GetService<ActorService>();
            var playerId = actorService.GetId(clash.player.Combatant);
            var enemyId = actorService.GetId(clash.enemy.Combatant);

            int clashStepCount = 0;
            while (clash.player.coinsLeft > 0 && clash.enemy.coinsLeft > 0)
            {
                clashStepCount++;
                Debug.Assert(clashStepCount < 1000); // limit the number of iterations

                switch (ResolveClashStep())
                {
                    case ClashResult.PlayerWin:
                        clash.enemy.brokenCoins++;
                        break;
                    case ClashResult.EnemyWin:
                        clash.player.brokenCoins++;
                        break;
                }
            }

            var winner = clash.player.coinsLeft > 0 ? ClashResult.PlayerWin : ClashResult.EnemyWin;
            switch (winner)
            {
                case ClashResult.PlayerWin:
                    messagingService?.BroadcastEvent(playerId, new OnClashWin());
                    messagingService?.BroadcastEvent(enemyId, new OnClashLose());
                    break;
                case ClashResult.EnemyWin:
                    messagingService?.BroadcastEvent(enemyId, new OnClashWin());
                    messagingService?.BroadcastEvent(playerId, new OnClashLose());
                    break;
                case ClashResult.Tie:
                    Debug.Assert(false);
                    break;
            }
            return winner;
        }
    }
}