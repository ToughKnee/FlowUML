using CleanArchitectureWorkshop.Presentation.Api.Teams.Dtos;

namespace CleanArchitectureWorkshop.Presentation.Api.Teams.Responses;

public record GetAllTeamsResponse(List<TeamDto> Teams);
