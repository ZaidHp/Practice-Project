using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Supplier;

namespace PharmaTrack.Application.Interfaces;

public interface ISupplierService
{
    Task<ApiResponseDto<IEnumerable<SupplierDto>>> GetAllSuppliersAsync();
    Task<ApiResponseDto<SupplierDto>> GetSupplierById(int id);
    Task<ApiResponseDto<SupplierDto>> CreateSupplierAsync(CreateSupplierDto dto, string createdBy);
    Task<ApiResponseDto<SupplierDto>> UpdateSupplierAsync(int id, UpdateSupplierDto dto, string updatedBy);
    Task<ApiResponseDto<bool>> DeleteSupplierAsync(int id);
}