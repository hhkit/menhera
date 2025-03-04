namespace menhera
{
    public class OnCoinFlip(bool[] coinResults) : IEvent
    {
        public bool[] CoinRes { get; private set; } = coinResults;
    }

    public class OnHeadsHit : IEvent
    {
    }

    public class OnTailsHit : IEvent
    {
    }

    public class OnClashWin(ClashingCombatant winner, ClashingCombatant loser, int clashCount) : IEvent
    {
        public ClashingCombatant Winner { get; private set; } = winner;
        public ClashingCombatant Loser { get; private set; } = loser;
        public int ClashCount { get; private set; } = clashCount;
    }

    public class OnClashLose(ClashingCombatant winner, ClashingCombatant loser, int clashCount) : IEvent
    {
        public ClashingCombatant Winner { get; private set; } = winner;
        public ClashingCombatant Loser { get; private set; } = loser;
        public int ClashCount { get; private set; } = clashCount;
    }
}