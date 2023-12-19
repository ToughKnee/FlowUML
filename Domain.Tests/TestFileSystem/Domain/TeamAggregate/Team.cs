using CleanArchitectureWorkshop.Domain.Core;
using System.Collections.Generic;

namespace CleanArchitectureWorkshop.Domain.TeamAggregate;
public class Team : AggregateRoot<TeamName>
{
    private readonly List<Player> _players = new();
    public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

    public Team(TeamName name) : base(name)
    {
    }

    // For EFCore do not use.
    protected Team()
    {
    }

    public void AddPLayer(Player player)
    {
        if(_players.Exists(p => p == player))
        {
            throw new InvalidOperationException("Player is already on the team");
        }
        _players.Add(player);
        player.AssignTeam(this);
    }

    public void RemovePlayer(UserName player)
    {
        var playerToRemove = _players.FirstOrDefault(p => p.Id == player);
        if (playerToRemove is null)
        {
            throw new InvalidOperationException("Player is not on the team");
        }

        _players.Remove(playerToRemove);
        playerToRemove.UnassignTeam();
    }
}