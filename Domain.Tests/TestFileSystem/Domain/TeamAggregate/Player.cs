using CleanArchitectureWorkshop.Domain.Core;

namespace CleanArchitectureWorkshop.Domain.TeamAggregate;
public class Player : Entity<UserName>
{
    public Team? Team { get; private set; } = null;

    public Player(UserName userName) : base(userName)
    {
        Team = null;
    }
    private Player()
    {
    }

    public void AssignTeam(Team team)
    {
        // Busines rule enforcemnent
        if (Team is not null)
        {
            throw new InvalidOperationException("Cannot assign player to another team if player already has team");
        }
        Team = team;
    }
    public void UnassignTeam()
    {
        Team = null;
    }
}
