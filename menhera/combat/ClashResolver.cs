namespace menhera
{
    // this stores the data for a single combatant during a clash
    public struct ClashingCombatant(Combatant combatant, SkillData skill)
    {
        public Combatant Combatant { get; private set; } = combatant;
        public SkillData Skill { get; private set; } = skill;
        public int brokenCoins = 0;
        public int coinsLeft { get => Skill.coinCount - brokenCoins; }
    }

    public class ClashResolver
    {
        public enum ClashResult
        {
            PlayerWin,
            Tie,
            EnemyWin
        }

        public ClashingCombatant player, enemy;
        private ServiceLocator services = ServiceLocator.Main;

        private bool[] ResolveCoins(ClashingCombatant config)
        {
            var messagingService = services.GetService<MessagingService>();

            var coinResolver = new CoinResolver(config.Combatant, services);
            var coinResults = coinResolver.FlipCoins(config.Skill.coinCount - config.brokenCoins);
            messagingService?.BroadcastEvent(config.Combatant.id, new OnCoinFlip(coinResults));

            foreach (var coinRes in coinResults)
                messagingService?.BroadcastEvent(config.Combatant.id, coinRes ? new OnHeadsHit() : new OnTailsHit());

            return coinResults;
        }

        private int CalculateClashPower(SkillData skillData, bool[] coins)
        {
            return skillData.basePower + coins.Select(x => x).Count() * skillData.coinPower;
        }

        private int ResolveClashPower(ClashingCombatant combatant)
        {
            var coins = ResolveCoins(combatant);
            return CalculateClashPower(combatant.Skill, coins);
        }

        private ClashResult ResolveClashStep()
        {
            var playerClashPower = ResolveClashPower(player);
            var enemyClashPower = ResolveClashPower(enemy);

            if (playerClashPower == enemyClashPower)
                return ClashResult.Tie;

            return playerClashPower > enemyClashPower ? ClashResult.PlayerWin : ClashResult.EnemyWin;
        }

        public ClashResult ResolveClash()
        {
            var messagingService = services.GetService<MessagingService>();

            int clashStepCount = 0;
            while (player.coinsLeft > 0 && enemy.coinsLeft > 0)
            {
                clashStepCount++;
                switch (ResolveClashStep())
                {
                    case ClashResult.PlayerWin:
                        enemy.brokenCoins++;
                        break;
                    case ClashResult.EnemyWin:
                        player.brokenCoins++;
                        break;
                }
            }

            var winner = player.coinsLeft > 0 ? ClashResult.PlayerWin : ClashResult.EnemyWin;
            switch (winner)
            {
                case ClashResult.PlayerWin:
                    messagingService?.BroadcastEvent(player.Combatant.id, new OnClashWin());
                    messagingService?.BroadcastEvent(enemy.Combatant.id, new OnClashLose());
                    break;
                case ClashResult.EnemyWin:
                    messagingService?.BroadcastEvent(enemy.Combatant.id, new OnClashWin());
                    messagingService?.BroadcastEvent(player.Combatant.id, new OnClashLose());
                    break;
                case ClashResult.Tie:
                    break;

            }
            return winner;
        }
    }
}