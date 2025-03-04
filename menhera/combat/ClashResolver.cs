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
            var coinResults = coinResolver.FlipCoins(config.Skill.CoinCount - config.BrokenCoins);
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
            var playerClashPower = ResolveClashPower(clash.Player);
            var enemyClashPower = ResolveClashPower(clash.Enemy);

            if (playerClashPower == enemyClashPower)
                return ClashResult.Tie;

            return playerClashPower > enemyClashPower ? ClashResult.PlayerWin : ClashResult.EnemyWin;
        }

        public ClashResult ResolveClash()
        {
            var messagingService = services.GetService<MessagingService>();
            var actorService = services.GetService<ActorService>();
            var playerId = actorService.GetId(clash.Player.Combatant);
            var enemyId = actorService.GetId(clash.Enemy.Combatant);

            int clashStepCount = 0;
            while (clash.Player.CoinsLeft > 0 && clash.Enemy.CoinsLeft > 0)
            {
                clashStepCount++;
                Debug.Assert(clashStepCount < 1000, $"exceeded clash limit of 1000, playerCoins {clash.Player.CoinsLeft}, enemyCoins {clash.Enemy.CoinsLeft}"); // limit the number of iterations

                var clashStepResult = ResolveClashStep();
                switch (clashStepResult)
                {
                    case ClashResult.PlayerWin:
                        clash.Enemy.BrokenCoins++;
                        break;
                    case ClashResult.EnemyWin:
                        clash.Player.BrokenCoins++;
                        break;
                }
            }

            var winner = clash.Player.CoinsLeft > 0 ? ClashResult.PlayerWin : ClashResult.EnemyWin;
            switch (winner)
            {
                case ClashResult.PlayerWin:
                    messagingService?.BroadcastEvent(playerId, new OnClashWin(clash.Player, clash.Enemy, clashStepCount));
                    messagingService?.BroadcastEvent(enemyId, new OnClashLose(clash.Enemy, clash.Player, clashStepCount));
                    break;
                case ClashResult.EnemyWin:
                    messagingService?.BroadcastEvent(enemyId, new OnClashWin(clash.Enemy, clash.Player, clashStepCount));
                    messagingService?.BroadcastEvent(playerId, new OnClashLose(clash.Player, clash.Enemy, clashStepCount));
                    break;
                case ClashResult.Tie:
                    Debug.Assert(false);
                    break;
            }
            return winner;
        }
    }
}