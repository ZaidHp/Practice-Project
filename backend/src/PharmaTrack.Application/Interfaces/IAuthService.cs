using PharmaTrack.Application.DTOs.Auth;
using PharmaTrack.Application.DTOs.Common;

namespace PharmaTrack.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request);
}