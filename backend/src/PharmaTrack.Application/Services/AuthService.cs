using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmaTrack.Application.DTOs.Auth;
using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Application.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var users = await _userRepository.FindAsync(u => u.Email == request.Email && !u.IsDeleted && u.IsActive);
        var user = users.FirstOrDefault();

        if (user == null)
            return ApiResponseDto<AuthResponseDto>.FailureResponse("Invalid email or password.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            return ApiResponseDto<AuthResponseDto>.FailureResponse("Invalid email or password.");

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.RoleName ?? "User";

        var token = GenerateJwtToken(user, roleName);

        var response = new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            FullName = user.FullName,
            Role = roleName,
            ExpiresInMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpiryInMinutes"])
        };

        return ApiResponseDto<AuthResponseDto>.SuccessResponse(response, "Login successful.");
    }

    private string GenerateJwtToken(User user, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(secretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}