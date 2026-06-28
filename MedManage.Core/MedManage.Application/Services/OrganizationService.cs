using System.Security.Claims;
using AutoMapper;
using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MedManage.Application.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrganizationService(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<OrganizationDTO>> GetAllAsync()
    {
        var organizations = await _organizationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);
    }

    public async Task<OrganizationDTO> GetByIdAsync(Guid id)
    {
        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)
            throw new KeyNotFoundException("Организация не найдена");
        return _mapper.Map<OrganizationDTO>(organization);
    }

    public async Task<OrganizationDTO> CreateAsync(CreateOrganizationRequest request)
    {
        await EnsureAdminAsync();

        var existing = await _organizationRepository.FindByNameAsync(request.Name);
        if (existing != null)
            throw new InvalidOperationException("Организация с таким названием уже существует");

        var organization = new Organization(
            request.Name,
            request.Address,
            request.PhoneNumber,
            request.Email);

        await _organizationRepository.AddAsync(organization);
        return _mapper.Map<OrganizationDTO>(organization);
    }

    public async Task UpdateAsync(OrganizationDTO dto)
    {
        await EnsureAdminAsync();

        var organization = await _organizationRepository.GetByIdAsync(dto.OrganizationId);
        if (organization == null)
            throw new KeyNotFoundException("Организация не найдена");

        var existing = await _organizationRepository.FindByNameAsync(dto.Name);
        if (existing != null && existing.OrganizationId != dto.OrganizationId)
            throw new InvalidOperationException("Организация с таким названием уже существует");

        _mapper.Map(dto, organization);
        await _organizationRepository.UpdateAsync(organization);
    }

    public async Task DeleteAsync(Guid id)
    {
        await EnsureAdminAsync();

        var organization = await _organizationRepository.GetByIdAsync(id);
        if (organization == null)
            throw new KeyNotFoundException("Организация не найдена");

        await _organizationRepository.DeleteAsync(organization);
    }

    private async Task EnsureAdminAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Пользователь не авторизован");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Role < UserRole.Admin)
            throw new UnauthorizedAccessException("Недостаточно прав");
    }
}
