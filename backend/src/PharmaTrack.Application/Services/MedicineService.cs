using System.Xml.XPath;
using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Medicine;
using PharmaTrack.Application.Interfaces;
using System.Linq.Expressions;
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

    public async Task<ApiResponseDto<PaginatedListDto<MedicineDto>>> GetAllMedicineAsync(int page, int pageSize, string? search)
    {
       Expression<Func<Medicine, bool>> predicate = m => !m.IsDeleted;
    
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            predicate = m => !m.IsDeleted && 
                (m.MedicineName.ToLower().Contains(lowerSearch) || 
                m.MedicineCode.ToLower().Contains(lowerSearch) || 
                (m.GenericName != null && m.GenericName.ToLower().Contains(lowerSearch)));
        }

        var (items, totalCount) = await _medicineRepository.GetPagedAsync(predicate, page, pageSize);
        var category = await _categoryRepository.GetAllAsync();


        var dtos = items.Select(m => new MedicineDto
        {
            MedicineId = m.MedicineId,
            CategoryId = m.CategoryId,
            CategoryName = category.FirstOrDefault(c => c.CategoryId == m.CategoryId)?.CategoryName ?? "Unknown",
            MedicineCode = m.MedicineCode,
            MedicineName = m.MedicineName,
            GenericName = m.GenericName,
            ReorderLevel = m.ReorderLevel,
            UnitOfMeasure = m.UnitOfMeasure,
            IsActive = m.IsActive
        });

        var pagedResult = new PaginatedListDto<MedicineDto> 
        { 
            Items = dtos, 
            TotalCount = totalCount, 
            CurrentPage = page, 
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) 
        };
    
        return ApiResponseDto<PaginatedListDto<MedicineDto>>.SuccessResponse(pagedResult, "Medicines retrieved.");
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