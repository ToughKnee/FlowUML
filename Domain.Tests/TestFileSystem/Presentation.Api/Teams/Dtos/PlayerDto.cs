using CleanArchitectureWorkshop.Domain.TeamAggregate;

namespace CleanArchitectureWorkshop.Presentation.Api.Teams.Dtos;

public record PlayerDto(string UserName)
{
    public static PlayerDto FromPlayer(Player player)
    {
        return new PlayerDto(player.Id.Value);
    }
}
