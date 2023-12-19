using CleanArchitectureWorkshop.Domain.Core;
using CleanArchitectureWorkshop.Domain.TeamAggregate;

namespace CleanArchitectureWorkshop.Domain.MatchAggregate;
public class Match : AggregateRoot<MatchId>
{
    public TeamName HomeTeamId { get; set; }
    public TeamName AwayTeamId { get; set; }
    public MatchState State { get; set; }

    public Match(TeamName homeTeamId, TeamName awayTeamId) : base(MatchId.CreateUnique())
    {
        HomeTeamId = homeTeamId;
        AwayTeamId = awayTeamId;
        State = MatchState.NotStarted;
    }

}
