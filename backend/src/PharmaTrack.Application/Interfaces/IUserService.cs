using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.User;

namespace PharmaTrack.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponseDto<UserDto>> CreateUserAsync(CreateUserDto dto, string createdBy);
    Task<ApiResponseDto<IEnumerable<UserDto>>> GetAllUserAsync();
    Task<ApiResponseDto<bool>> DeleteUserAsync(int id);
}