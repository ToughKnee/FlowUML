using CleanArchitectureWorkshop.Application.Repositories;
using CleanArchitectureWorkshop.Domain.TeamAggregate;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureWorkshop.Infrastructure.Repositories;

internal class TeamsRepository : ITeamsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TeamsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateTeamAsync(Team team)
    {
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Team>> GetAllTeamsAsync()
    {
        return await _dbContext.Teams
            .Include(t => t.Players)
            .ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(TeamName teamName)
    {
        return await _dbContext.Teams
            .Include(t => t.Players)
            .FirstOrDefaultAsync(t => t.Id == teamName);
    }

    public async Task<List<Team>> GetTeamsByNameAsync(string searchTerm)
    {
        return await _dbContext.Teams
            .Include(t => t.Players)
            .Where(t => ((string)t.Id).Contains(searchTerm))
            .ToListAsync();
    }

    public async Task UpdateTeamAsync(Team team)
    {
        _dbContext.Teams.Update(team);
        await _dbContext.SaveChangesAsync();
    }
}
