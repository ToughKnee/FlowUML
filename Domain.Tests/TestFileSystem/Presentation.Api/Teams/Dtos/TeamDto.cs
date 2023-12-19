using CleanArchitectureWorkshop.Domain.TeamAggregate;

namespace CleanArchitectureWorkshop.Presentation.Api.Teams.Dtos;

public record TeamDto(string TeamName, List<PlayerDto> Players)
{
    public static TeamDto FromTeam(Team team)
    {
        return new TeamDto(
            team.Id.Value,
            team.Players
                .Select(p => PlayerDto.FromPlayer(p))
                .ToList());
    }
}
