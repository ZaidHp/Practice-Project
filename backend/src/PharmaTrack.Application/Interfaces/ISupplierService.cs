using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Supplier;

namespace PharmaTrack.Application.Interfaces;

public interface ISupplierService
{
    Task<ApiResponseDto<PaginatedListDto<SupplierDto>>> GetAllSuppliersAsync(int page, int pageSize, string? search);
    Task<ApiResponseDto<SupplierDto>> GetSupplierById(int id);
    Task<ApiResponseDto<SupplierDto>> CreateSupplierAsync(CreateSupplierDto dto, string createdBy);
    Task<ApiResponseDto<SupplierDto>> UpdateSupplierAsync(int id, UpdateSupplierDto dto, string updatedBy);
    Task<ApiResponseDto<bool>> DeleteSupplierAsync(int id);
    Task<ApiResponseDto<IEnumerable<SupplierDto>>> GetSuppliersAsync();
}