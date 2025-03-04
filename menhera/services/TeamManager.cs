

namespace menhera
{
    public enum Scope
    {
        Self,
        Ally,
        AllyButSelf,
        Enemy,
        All,
    }

    public class TeamManager : Service
    {
        readonly Dictionary<int, List<ActorIdentifier>> teams = [];

        public void Join(ActorIdentifier actor, int team)
        {
            if (!teams.ContainsKey(team))
                teams.Add(team, []);

            teams[team].Add(actor);
        }

        public long GetFilterFor(ActorIdentifier member, Scope scope)
        {
            switch (scope)
            {
                case Scope.Self:
                    return member.Flag;
                case Scope.Ally:
                    {
                        var teamFilter = GetTeamFilter(member.Team);
                        return teamFilter;
                    }
                case Scope.AllyButSelf:
                    {
                        var teamFilter = GetTeamFilter(member.Team);
                        return teamFilter & ~member.Flag;
                    }
                case Scope.Enemy:
                    var enemyFilter = teams
                        .Select(elem => elem.Key)
                        .Where(elem => elem != member.Team)
                        .Select(GetTeamFilter)
                        .Aggregate((lhs, rhs) => lhs | rhs);
                    return enemyFilter;
                case Scope.All:
                    return -1;
                default:
                    return 0;
            }
        }

        private long GetTeamFilter(int team)
        {
            return teams
                .GetValueOrDefault(team, [])
                .Select(inst => inst.Flag)
                .Aggregate((lhs, rhs) => lhs | rhs);
        }
    }
}