using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Category;

namespace PharmaTrack.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponseDto<PaginatedListDto<CategoryDto>>> GetAllCategoryAsync(int page, int pageSize, string? search);
    Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, string createdBy);
    Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id);
    Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto, string updatedBy);
    Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
}