using CleanArchitectureWorkshop.Application.UseCases;
using CleanArchitectureWorkshop.Presentation.Api.Teams.Dtos;
using CleanArchitectureWorkshop.Presentation.Api.Teams.Requests;
using CleanArchitectureWorkshop.Presentation.Api.Teams.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureWorkshop.Presentation.Api.Teams;

public static class TeamsEndpoints
{
    public static async Task<PostTeamResponse> PostTeamHandler(
        [FromRoute] string teamName,
        [FromServices] ITeamsUseCase teamsUseCase)
    {
        var team = await teamsUseCase.CreateTeamAsync(teamName);
        return new PostTeamResponse(TeamDto.FromTeam(team));
    }

    public static async Task<AddPlayerToTeamResponse> AddPlayerToTeamHandler(
        [FromRoute] string teamName,
        [FromRoute] string playerName,
        [FromServices] ITeamsUseCase teamsUseCase)
    {
        var team = await teamsUseCase.AddPlayerToTeamAsync(teamName, playerName);

        return new AddPlayerToTeamResponse(
            TeamDto.FromTeam(team));
    }

    public static async Task<Results<NotFound, Ok<GetTeamByIdResponse>>> GetTeamByIdHandler(
        [FromRoute] string teamName,
        [FromServices] ITeamsUseCase teamsUseCase)
    {
        var team = await teamsUseCase.GetTeamByIdAsync(teamName);
        if (team is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(
            new GetTeamByIdResponse(
            TeamDto.FromTeam(team)));
    }

    public static async Task<GetAllTeamsResponse> GetAllTeamsHandler(
        [FromServices] ITeamsUseCase teamsUseCase)
    {
        var teams = await teamsUseCase.GetAllTeamsAsync();

        return new GetAllTeamsResponse(
            teams.Select(t => TeamDto.FromTeam(t))
                 .ToList());
    }

    public static async Task<GetAllTeamsResponse> SearchTeamsByNameHandler(
        [FromQuery] string teamName,
        [FromServices] ITeamsUseCase teamsUseCase)
    {
        var teams = await teamsUseCase.GetTeamsByNameAsync(teamName);

        return new GetAllTeamsResponse(
            teams.Select(t => TeamDto.FromTeam(t))
                 .ToList());
    }

    public static Ok PostReceiveRandomData(
        [FromBody] RandomDataRequest request)
    {
        Console.WriteLine($"Received the following random request: {request}. Don't know what to do with it.");
        return TypedResults.Ok();
    }

    public static IEndpointRouteBuilder RegisterTeamsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/teams/{teamName}", TeamsEndpoints.PostTeamHandler)
            .WithName("PostTeam")
            .WithOpenApi();

        builder
            .MapPost("/teams/{teamName}/add-player/{playerName}", TeamsEndpoints.AddPlayerToTeamHandler)
            .WithName("AddPlayer")
            .WithOpenApi();

        builder
            .MapGet("/teams/{teamName}", TeamsEndpoints.GetTeamByIdHandler)
            .WithName("GetTeamById")
            .WithOpenApi();

        builder
            .MapGet("/teams", TeamsEndpoints.GetAllTeamsHandler)
            .WithName("GetAllTeams")
            .WithOpenApi();

        builder
            .MapGet("/teams/search/{teamName}", TeamsEndpoints.SearchTeamsByNameHandler)
            .WithName("FindTeamsByName")
            .WithOpenApi();

        builder
            .MapPost("/random-data", TeamsEndpoints.PostReceiveRandomData)
            .WithName("PostRandomData")
            .WithOpenApi();

        return builder;
    }
}
