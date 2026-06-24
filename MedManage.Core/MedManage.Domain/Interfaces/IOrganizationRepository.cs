using MedManage.Domain.Entities;

namespace MedManage.Domain.Interfaces;

public interface IOrganizationRepository
{
    Task<IEnumerable<Organization>> GetAllAsync();
    Task<Organization?> GetByIdAsync(Guid id);
    Task<Organization?> FindByNameAsync(string name);
    Task<Organization> AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}
