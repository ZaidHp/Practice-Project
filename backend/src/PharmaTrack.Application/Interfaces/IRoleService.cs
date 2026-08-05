using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Role;

namespace PharmaTrack.Application.Interfaces;

public interface IRoleService
{
    Task<ApiResponseDto<IEnumerable<RoleDto>>> GetAllRolesAsync();
}