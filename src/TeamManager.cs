

namespace menhera;

public enum Scope
{
    Self,
    Ally,
    AllyButSelf,
    Enemy,
    All,
}

public class TeamManager
{
    private List<ActorIdentifier> members = [];
    private Dictionary<int, List<ActorIdentifier>> teams = new();

    public ActorIdentifier Join(int team)
    {
        var member = new ActorIdentifier(members.Count, team);
        members.Add(member);
        teams.GetValueOrDefault(team, []).Add(member);
        return member;
    }

    public long GetFilterFor(ActorIdentifier member, Scope scope)
    {
        switch (scope)
        {
            case Scope.Self:
                return member.flag;
            case Scope.Ally:
                {
                    var teamFilter = GetTeamFilter(member.team);
                    return teamFilter;
                }
            case Scope.AllyButSelf:
                {
                    var teamFilter = GetTeamFilter(member.team);
                    return teamFilter & ~member.flag;
                }
            case Scope.Enemy:
                var enemyFilter = teams
                    .Where(elem => elem.Key != member.team)
                    .Select(elem => GetTeamFilter(elem.Key))
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
            .Select(inst => inst.flag)
            .Aggregate((lhs, rhs) => lhs | rhs);
    }
}