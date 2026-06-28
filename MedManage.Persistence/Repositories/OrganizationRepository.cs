using MedManage.Domain.Entities;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly IAppDbContext _context;

    public OrganizationRepository(IAppDbContext context)
    {
        _context = context;
    }

    [Cache("AllOrganizations", ExpirationSeconds = 600)]
    public async Task<IEnumerable<Organization>> GetAllAsync()
    {
        return await _context.Organizations.ToListAsync();
    }

    [Cache("OrganizationById:{id}", ExpirationSeconds = 1800)]
    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _context.Organizations.FindAsync(id);
    }

    public async Task<Organization?> FindByNameAsync(string name)
    {
        return await _context.Organizations.FirstOrDefaultAsync(o => o.Name == name);
    }

    [Transactional]
    [CacheInvalidate("AllOrganizations")]
    public async Task<Organization> AddAsync(Organization organization)
    {
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
        return organization;
    }

    [Transactional]
    [CacheInvalidate("AllOrganizations", "OrganizationById:*")]
    public async Task UpdateAsync(Organization organization)
    {
        _context.Organizations.Update(organization);
        await _context.SaveChangesAsync();
    }

    [Transactional]
    [CacheInvalidate("AllOrganizations", "OrganizationById:*")]
    public async Task DeleteAsync(Organization organization)
    {
        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();
    }
}
