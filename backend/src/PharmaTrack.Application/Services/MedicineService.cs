using System.Xml.XPath;
using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Medicine;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;

namespace PharmaTrack.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IGenericRepository<Medicine> _medicineRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<Batch> _batchRepository;

    public MedicineService(IGenericRepository<Medicine> medicineRepository, IGenericRepository<Category> categoryRepository, IGenericRepository<Batch> batchRepository)
    {
        _medicineRepository = medicineRepository;
        _categoryRepository = categoryRepository;
        _batchRepository = batchRepository;
    }

    public async Task<ApiResponseDto<IEnumerable<MedicineDto>>> GetAllMedicineAsync()
    {
        var medicines = await _medicineRepository.GetAllAsync();
        var categories = await _categoryRepository.GetAllAsync();

        var dtoList = medicines.Select( m => new MedicineDto
        {
            MedicineId = m.MedicineId,
            MedicineCode = m.MedicineCode,
            MedicineName = m.MedicineName,
            GenericName = m.GenericName,
            CategoryId = m.CategoryId,
            CategoryName = categories
                .FirstOrDefault(c => c.CategoryId == m.CategoryId)?.CategoryName ?? "Unknown",
            ReorderLevel = m.ReorderLevel,
            UnitOfMeasure = m.UnitOfMeasure
        });

        return ApiResponseDto<IEnumerable<MedicineDto>>.SuccessResponse(dtoList);
    }

    public async Task<ApiResponseDto<MedicineDto>> CreateMedicineAsync(CreateMedicineDto dto, string createdBy)
    {
        var existing = await _medicineRepository.FindIgnoreQueryFiltersAsync(
            m => m.MedicineCode.ToLower() == dto.MedicineCode.ToLower());

        if (existing.Any())
            return ApiResponseDto<MedicineDto>.FailureResponse("A medicine with this code already exists.");
        
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

        if (category == null)
            return ApiResponseDto<MedicineDto>.FailureResponse("Invalid Category ID.");

        var newMedicine = new Medicine
        {
            MedicineCode = dto.MedicineCode,
            MedicineName = dto.MedicineName,
            GenericName = dto.GenericName,
            CategoryId = dto.CategoryId,
            ReorderLevel = dto.ReorderLevel,
            UnitOfMeasure = dto.UnitOfMeasure,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            IsActive = true
        };

        await _medicineRepository.AddAsync(newMedicine);
        await _medicineRepository.SaveChangesAsync();

        var result = new MedicineDto
        {
            MedicineId = newMedicine.MedicineId,
            MedicineCode = newMedicine.MedicineCode,
            MedicineName = newMedicine.MedicineName,
            GenericName = newMedicine.GenericName,
            CategoryId = newMedicine.CategoryId,
            CategoryName = category.CategoryName,
            ReorderLevel = newMedicine.ReorderLevel,
            UnitOfMeasure = newMedicine.UnitOfMeasure,
            IsActive = true

        };

        return ApiResponseDto<MedicineDto>.SuccessResponse(result, "Medicine created successfully");
    }

    public async Task<ApiResponseDto<MedicineDto>> GetMedicineByIdAsync(int id)
    {
        var medicine = await _medicineRepository.GetByIdAsync(id);
        var batches = await _batchRepository.FindAsync(b => b.MedicineId == id);

        if (medicine == null)
            return ApiResponseDto<MedicineDto>.FailureResponse("Medicine not found.");

        var category = await _categoryRepository.GetByIdAsync(medicine.CategoryId);
        
        
        var dto = new MedicineDto
        {
            MedicineId = medicine.MedicineId,
            MedicineCode = medicine.MedicineCode,
            MedicineName = medicine.MedicineName,
            GenericName = medicine.GenericName,
            CategoryId = medicine.CategoryId,
            CategoryName = category?.CategoryName ?? "Unknown",
            ReorderLevel = medicine.ReorderLevel,
            UnitOfMeasure = medicine.UnitOfMeasure,
            TotalActiveStock = batches.Where(b => !b.IsDeleted && b.CurrentStock > 0).Sum(b => b.CurrentStock), //b => b.IsActive
            IsActive = medicine.IsActive
        };

        return ApiResponseDto<MedicineDto>.SuccessResponse(dto);

    }

    public async Task<ApiResponseDto<MedicineDto>> UpdateMedicineAsync(int id, UpdateMedicineDto dto, string updatedBy)
    {
        var medicine = await _medicineRepository.GetByIdAsync(id);

        if (medicine == null)
            return ApiResponseDto<MedicineDto>.FailureResponse("Medicine not found.");

        var existing = await _medicineRepository.FindIgnoreQueryFiltersAsync(
            m => m.MedicineCode.ToLower() == dto.MedicineCode.ToLower() && m.MedicineId != id);

        if (existing.Any())
            return ApiResponseDto<MedicineDto>.FailureResponse("A medicine with this code already exists.");

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

        if (category == null)
            return ApiResponseDto<MedicineDto>.FailureResponse("Invalid Category ID.");

        medicine.MedicineCode = dto.MedicineCode;
        medicine.MedicineName = dto.MedicineName;
        medicine.GenericName = dto.GenericName;
        medicine.CategoryId = dto.CategoryId;
        medicine.ReorderLevel = dto.ReorderLevel;
        medicine.UnitOfMeasure = dto.UnitOfMeasure;
        medicine.IsActive = dto.IsActive;
        medicine.UpdatedAt = DateTime.UtcNow;
        medicine.UpdatedBy = updatedBy;

        _medicineRepository.Update(medicine);
        await _medicineRepository.SaveChangesAsync();

        var result = new MedicineDto
        {
            MedicineId = medicine.MedicineId,
            MedicineCode = medicine.MedicineCode,
            MedicineName = medicine.MedicineName,
            GenericName = medicine.GenericName,
            CategoryId = medicine.CategoryId,
            ReorderLevel = medicine.ReorderLevel,
            UnitOfMeasure = medicine.UnitOfMeasure,
            IsActive = medicine.IsActive
        };

        return ApiResponseDto<MedicineDto>.SuccessResponse(result, "Medicine updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteMedicineAsync(int id)
    {
        var medicine = await _medicineRepository.GetByIdAsync(id);

        if (medicine == null)
            return ApiResponseDto<bool>.FailureResponse("Medicine not found.");

        _medicineRepository.SoftDelete(medicine);
        await _medicineRepository.SaveChangesAsync();

        return ApiResponseDto<bool>.SuccessResponse(true, "Medicine deleted successfully.");
    }
}