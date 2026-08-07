using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Category;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;
using System.Linq.Expressions;

namespace PharmaTrack.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponseDto<PaginatedListDto<CategoryDto>>> GetAllCategoryAsync(int page, int pageSize, string? search)
    {
        Expression<Func<Category, bool>> predicate = c => !c.IsDeleted;
    
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            predicate = c => !c.IsDeleted && 
                (c.CategoryName.ToLower().Contains(lowerSearch) || 
                (c.Description != null && c.Description.ToLower().Contains(lowerSearch)));
        }

        var (items, totalCount) = await _categoryRepository.GetPagedAsync(predicate, page, pageSize);

        var dtos = items.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive
        });

        var pagedResult = new PaginatedListDto<CategoryDto> 
        { 
            Items = dtos, 
            TotalCount = totalCount, 
            CurrentPage = page, 
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) 
        };
    
        return ApiResponseDto<PaginatedListDto<CategoryDto>>.SuccessResponse(pagedResult, "Categories retrieved.");
    }

    public async Task<ApiResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, string createdBy)
    {
        var existingCategory = await _categoryRepository.FindIgnoreQueryFiltersAsync(c => 
            c.CategoryName.ToLower() == dto.CategoryName.ToLower());

        if (existingCategory.Any())
        {
            return ApiResponseDto<CategoryDto>.FailureResponse("A category with this name already exists.");
        }
        var category = new Category
        {
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsActive = true
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        var result = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        };

        return ApiResponseDto<CategoryDto>.SuccessResponse(result, "Category created successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponseDto<bool>.FailureResponse("Category not found.");
        }

        _categoryRepository.SoftDelete(category);
        await _categoryRepository.SaveChangesAsync();

        return ApiResponseDto<bool>.SuccessResponse(true, "Category deleted successfully.");
    }

    public async Task<ApiResponseDto<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto, string updatedBy)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponseDto<CategoryDto>.FailureResponse("Category not found.");
        }

        var existingCategory = await _categoryRepository.FindIgnoreQueryFiltersAsync(c => 
            c.CategoryName.ToLower() == dto.CategoryName.ToLower() && 
            c.CategoryId != id);

        if (existingCategory.Any())
        {
            return ApiResponseDto<CategoryDto>.FailureResponse("A category with this name already exists.");
        }

        category.CategoryName = dto.CategoryName;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = updatedBy;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();

        var responseDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive

        };

        return ApiResponseDto<CategoryDto>.SuccessResponse(responseDto, "Category updated successfully");
    }

    public async Task<ApiResponseDto<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return ApiResponseDto<CategoryDto>.FailureResponse("Category not found.");
        }

        var responseDto = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive
        };

        return ApiResponseDto<CategoryDto>.SuccessResponse(responseDto);
    }

    public async Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        var dtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            IsActive = c.IsActive
        });

        return ApiResponseDto<IEnumerable<CategoryDto>>.SuccessResponse(dtos, "Categories retrieved successfully.");
    }
}