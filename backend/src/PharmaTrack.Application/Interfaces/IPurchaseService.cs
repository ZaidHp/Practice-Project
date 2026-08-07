using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Purchase;

namespace PharmaTrack.Application.Interfaces;

public interface IPurchaseService
{
    Task<ApiResponseDto<bool>> AddPurchasesAsync(CreatePurchaseRequestDto dto, int id, string createdBy);
    Task<ApiResponseDto<IEnumerable<PurchaseDto>>> GetAllPurchasesAsync();
}