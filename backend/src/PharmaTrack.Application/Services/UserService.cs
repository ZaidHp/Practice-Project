using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.User;
using PharmaTrack.Application.Interfaces;
using System.Linq.Expressions;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Application.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;

    public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponseDto<UserDto>> CreateUserAsync(CreateUserDto dto, string createdBy)
    {
        var existingUsers = await _userRepository.FindAsync(u =>
            (u.Email == dto.Email || u.Username == dto.Username) && !u.IsDeleted);
       
        if (existingUsers.Any())
        {
            if (existingUsers.Any(u => u.Username == dto.Username))
                return ApiResponseDto<UserDto>.FailureResponse("A user with this username already exists.");
           
            return ApiResponseDto<UserDto>.FailureResponse("A user with this email already exists.");
        }

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role == null)
        {
            return ApiResponseDto<UserDto>.FailureResponse("Invalid Role ID.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var newUser = new User
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            RoleId = dto.RoleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsActive = true
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        var responseDto = new UserDto
        {
            UserId = newUser.UserId,
            FullName = newUser.FullName,
            Username = newUser.Username,
            Email = newUser.Email,
            RoleName = role.RoleName,
            IsActive = newUser.IsActive
        };

        return ApiResponseDto<UserDto>.SuccessResponse(responseDto, "User created successfully.");
    }

    public async Task<ApiResponseDto<PaginatedListDto<UserDto>>> GetAllUserAsync(int page, int pageSize, string? search)
    {
        Expression<Func<User, bool>> predicate = u => !u.IsDeleted;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            predicate = u => !u.IsDeleted && 
                (u.FullName.ToLower().Contains(lowerSearch) || 
                u.Username.ToLower().Contains(lowerSearch) || 
                u.Email.ToLower().Contains(lowerSearch));
        }

        var (items, totalCount) = await _userRepository.GetPagedAsync(predicate, page, pageSize);
        var roles = await _roleRepository.GetAllAsync();

        var dtos = items.Select(u => new UserDto
        {
            UserId = u.UserId,
            FullName = u.FullName,
            Username = u.Username,
            Email = u.Email,
            RoleName = roles.FirstOrDefault(r => r.RoleId == u.RoleId)?.RoleName ?? "Unknown",
            IsActive = u.IsActive
        });

        var pagedResult = new PaginatedListDto<UserDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return ApiResponseDto<PaginatedListDto<UserDto>>.SuccessResponse(pagedResult, "Users retrieved successfully.");
    }


    public async Task<ApiResponseDto<bool>> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return ApiResponseDto<bool>.FailureResponse("User not found.");
        }

        _userRepository.SoftDelete(user);
        await _userRepository.SaveChangesAsync();
        
        return ApiResponseDto<bool>.SuccessResponse(true, "User deleted successfully.");
    }
}