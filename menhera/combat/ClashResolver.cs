using System.Diagnostics;

namespace menhera
{


    public class ClashResolver(Clash clash, ServiceLocator services)
    {
        public enum ClashResult
        {
            PlayerWin,
            Tie,
            EnemyWin
        }

        public readonly Clash clash = clash;
        private readonly ServiceLocator services = services;

        private bool[] ResolveCoins(ClashingCombatant config)
        {
            var messagingService = services.GetService<MessagingService>();
            var actorService = services.GetService<ActorService>();
            var id = actorService.GetId(config.Combatant);

            var coinResolver = new CoinResolver(config.Combatant, services);
            var coinResults = coinResolver.FlipCoins(config.Skill.CoinCount - config.brokenCoins);
            messagingService.BroadcastEvent(id, new OnCoinFlip(coinResults));

            foreach (var coinRes in coinResults)
                messagingService.BroadcastEvent(id, coinRes ? new OnHeadsHit() : new OnTailsHit());

            return coinResults;
        }

        private static int CalculateClashPower(SkillData skillData, bool[] coins)
        {
            return skillData.BasePower + coins.Where(x => x).Count() * skillData.CoinPower;
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
            while (clash.player.CoinsLeft > 0 && clash.enemy.CoinsLeft > 0)
            {
                clashStepCount++;
                Debug.Assert(clashStepCount < 1000, $"exceeded clash limit of 1000, playerCoins {clash.player.CoinsLeft}, enemyCoins {clash.enemy.CoinsLeft}"); // limit the number of iterations

                var clashStepResult = ResolveClashStep();
                switch (clashStepResult)
                {
                    case ClashResult.PlayerWin:
                        clash.enemy.brokenCoins++;
                        break;
                    case ClashResult.EnemyWin:
                        clash.player.brokenCoins++;
                        break;
                }
            }

            var winner = clash.player.CoinsLeft > 0 ? ClashResult.PlayerWin : ClashResult.EnemyWin;
            switch (winner)
            {
                case ClashResult.PlayerWin:
                    messagingService?.BroadcastEvent(playerId, new OnClashWin(clash.player, clash.enemy, clashStepCount));
                    messagingService?.BroadcastEvent(enemyId, new OnClashLose(clash.enemy, clash.player, clashStepCount));
                    break;
                case ClashResult.EnemyWin:
                    messagingService?.BroadcastEvent(enemyId, new OnClashWin(clash.enemy, clash.player, clashStepCount));
                    messagingService?.BroadcastEvent(playerId, new OnClashLose(clash.player, clash.enemy, clashStepCount));
                    break;
                case ClashResult.Tie:
                    Debug.Assert(false);
                    break;
            }
            return winner;
        }
    }
}