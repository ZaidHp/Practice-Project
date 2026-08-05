using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Role;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Application.Services;

public class RoleService : IRoleService
{
    private readonly IGenericRepository<Role> _roleRepository;

    public RoleService(IGenericRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponseDto<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        var dtoList = roles.Select(r => new RoleDto
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            Description = r.Description ?? "Unknown",
            IsActive = r.IsActive
        });

        return ApiResponseDto<IEnumerable<RoleDto>>.SuccessResponse(dtoList);
    }
}