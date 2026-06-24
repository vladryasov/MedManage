using MedManage.Application.DTOs;
using MedManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedManage.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizations = await _organizationService.GetAllAsync();
        return Ok(organizations);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var organization = await _organizationService.GetByIdAsync(id);
            return Ok(organization);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request)
    {
        try
        {
            var organization = await _organizationService.CreateAsync(request);
            return Ok(organization);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] OrganizationDTO dto)
    {
        try
        {
            await _organizationService.UpdateAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _organizationService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
