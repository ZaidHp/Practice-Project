using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Medicine;

namespace PharmaTrack.Application.Interfaces;

public interface IMedicineService
{
    Task<ApiResponseDto<IEnumerable<MedicineDto>>> GetAllMedicineAsync();
    Task<ApiResponseDto<MedicineDto>> CreateMedicineAsync(CreateMedicineDto dto, string createdBy);
    Task<ApiResponseDto<MedicineDto>> GetMedicineByIdAsync(int id);
    Task<ApiResponseDto<MedicineDto>> UpdateMedicineAsync(int id, UpdateMedicineDto dto, string updatedBy);
    Task<ApiResponseDto<bool>> DeleteMedicineAsync(int id);
}