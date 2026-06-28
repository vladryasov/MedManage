using MedManage.Application.DTOs;

namespace MedManage.Application.Interfaces;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDTO>> GetAllAsync();
    Task<OrganizationDTO> GetByIdAsync(Guid id);
    Task<OrganizationDTO> CreateAsync(CreateOrganizationRequest request);
    Task UpdateAsync(OrganizationDTO dto);
    Task DeleteAsync(Guid id);
}
