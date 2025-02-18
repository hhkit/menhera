namespace menhera
{
    public class OnCoinFlip(bool[] coinResults) : IEvent
    {
        public bool[] coinRes = coinResults;
    }

    public class OnHeadsHit : IEvent
    {
    }

    public class OnTailsHit : IEvent
    {
    }

    public class OnClashWin : IEvent
    {
        public ClashingCombatant winner;
    }

    public class OnClashLose : IEvent
    {
        public ClashingCombatant winner;
    }
}