using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Category;

namespace PharmaTrack.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllCategoryAsync();
    Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, string createdBy);
    Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id);
    Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto, string updatedBy);
}