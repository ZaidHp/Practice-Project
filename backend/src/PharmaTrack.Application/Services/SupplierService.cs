using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Supplier;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;
using System.Linq.Expressions;

namespace PharmaTrack.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IGenericRepository<Supplier> _supplierRepository;

    public SupplierService(IGenericRepository<Supplier> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<ApiResponseDto<PaginatedListDto<SupplierDto>>> GetAllSuppliersAsync(int page, int pageSize, string? search)
    {
        Expression<Func<Supplier, bool>> predicate = s => !s.IsDeleted;
    
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            predicate = s => !s.IsDeleted && 
                (s.SupplierName.ToLower().Contains(lowerSearch) || 
                (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(lowerSearch)) || 
                (s.Email != null && s.Email.ToLower().Contains(lowerSearch)));
        }

        var (items, totalCount) = await _supplierRepository.GetPagedAsync(predicate, page, pageSize);

        var dtos = items.Select(s => new SupplierDto
        {
            SupplierId = s.SupplierId,
            SupplierName = s.SupplierName,
            ContactPerson = s.ContactPerson,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            IsActive = s.IsActive
        });

        var pagedResult = new PaginatedListDto<SupplierDto> 
        { 
            Items = dtos, 
            TotalCount = totalCount, 
            CurrentPage = page, 
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) 
        };
    
        return ApiResponseDto<PaginatedListDto<SupplierDto>>.SuccessResponse(pagedResult, "Suppliers retrieved.");
    }

    public async Task<ApiResponseDto<SupplierDto>> GetSupplierById(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
            return ApiResponseDto<SupplierDto>.FailureResponse("Supplier not found");
        
        var dtoList = new SupplierDto
        {
           SupplierId = supplier.SupplierId,
           SupplierName = supplier.SupplierName,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            IsActive = supplier.IsActive 
        };

        return ApiResponseDto<SupplierDto>.SuccessResponse(dtoList);
    }

    public async Task<ApiResponseDto<SupplierDto>> CreateSupplierAsync(CreateSupplierDto dto, string createdBy)
    {
        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            !s.IsDeleted &&
            s.SupplierName.ToLower() == dto.SupplierName.ToLower())).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this name already exists.");
        }

        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            !s.IsDeleted &&
            s.Phone == dto.Phone)).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this phone number already exists.");
        }

        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            !s.IsDeleted &&
            s.Email != null &&
            dto.Email != null &&
            s.Email.ToLower() == dto.Email.ToLower()
            )).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this email already exists.");
        }

        var newSupplier = new Supplier
        {
          SupplierName = dto.SupplierName,
          ContactPerson = dto.ContactPerson,
          Phone = dto.Phone,
          Email = dto.Email,
          Address = dto.Address,
          CreatedAt = DateTime.UtcNow,
          CreatedBy = createdBy,
          IsActive = true
        };

        await _supplierRepository.AddAsync(newSupplier);
        await _supplierRepository.SaveChangesAsync();

        var result = new SupplierDto
        {
            SupplierId = newSupplier.SupplierId,
            SupplierName = newSupplier.SupplierName,
            ContactPerson = newSupplier.ContactPerson,
            Phone = newSupplier.Phone,
            Email = newSupplier.Email,
            Address = newSupplier.Address,
            IsActive = newSupplier.IsActive
        };

        return ApiResponseDto<SupplierDto>.SuccessResponse(result, "Supplier created successfully.");
    }

    public async Task<ApiResponseDto<SupplierDto>> UpdateSupplierAsync(int id, UpdateSupplierDto dto, string updatedBy)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        
        if (supplier == null || supplier.IsDeleted)
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("Supplier not found.");
        }

        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            s.SupplierId != id &&
            !s.IsDeleted &&
            s.SupplierName.ToLower() == dto.SupplierName.ToLower())).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this name already exists.");
        }

        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            s.SupplierId != id &&
            !s.IsDeleted &&
            s.Phone == dto.Phone)).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this phone number already exists.");
        }

        if ((await _supplierRepository.FindIgnoreQueryFiltersAsync(s =>
            s.SupplierId != id &&
            !s.IsDeleted &&
            s.Email != null &&
            dto.Email != null &&
            s.Email.ToLower() == dto.Email.ToLower()
            )).Any())
        {
            return ApiResponseDto<SupplierDto>.FailureResponse("A supplier with this email already exists.");
        }

        supplier.SupplierName = dto.SupplierName;
        supplier.ContactPerson = dto.ContactPerson;
        supplier.Phone = dto.Phone;
        supplier.Email = dto.Email;
        supplier.Address = dto.Address;
        supplier.IsActive = dto.IsActive;
        
        supplier.UpdatedAt = DateTime.UtcNow;
        supplier.UpdatedBy = updatedBy;

        _supplierRepository.Update(supplier);
        await _supplierRepository.SaveChangesAsync();

        var responseDto = new SupplierDto
        {
            SupplierId = supplier.SupplierId,
            SupplierName = supplier.SupplierName,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            IsActive = supplier.IsActive
        };

        return ApiResponseDto<SupplierDto>.SuccessResponse(responseDto, "Supplier updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteSupplierAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        
        if (supplier == null || supplier.IsDeleted)
        {
            return ApiResponseDto<bool>.FailureResponse("Supplier not found.");
        }

        _supplierRepository.SoftDelete(supplier);
        await _supplierRepository.SaveChangesAsync();

        return ApiResponseDto<bool>.SuccessResponse(true, "Supplier deleted successfully.");
    }

    public async Task<ApiResponseDto<IEnumerable<SupplierDto>>> GetSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();

        var dtos = suppliers.Select(s => new SupplierDto
        {
            SupplierId = s.SupplierId,
            SupplierName = s.SupplierName,
            ContactPerson = s.ContactPerson,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            IsActive = s.IsActive
        });

        return ApiResponseDto<IEnumerable<SupplierDto>>.SuccessResponse(dtos, "Suppliers retrieved.");
    }
}