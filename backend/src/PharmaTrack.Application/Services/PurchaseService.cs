using System.Transactions;
using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Application.DTOs.Purchase;
using PharmaTrack.Application.Interfaces;
using PharmaTrack.Domain.Entities;
using PharmaTrack.Domain.Enums;
using System.Linq.Expressions;

namespace PharmaTrack.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IGenericRepository<Purchase> _purchaseRepository;
        private readonly IGenericRepository<Batch> _batchRepository;
        private readonly IGenericRepository<StockTransaction> _stockTransactionRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;

        public PurchaseService(
            IGenericRepository<Purchase> purchaseRepository,
            IGenericRepository<Batch> batchRepository,
            IGenericRepository<StockTransaction> stockTransactionRepository,
            IGenericRepository<Supplier> supplierRepository)
        {
            _purchaseRepository = purchaseRepository;
            _batchRepository = batchRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<ApiResponseDto<bool>> AddPurchasesAsync(CreatePurchaseRequestDto dto, int id, string createdBy)
        {
            if (dto.Medicines == null || dto.Medicines.Count == 0)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "No medicines provided in the purchase."
                };
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var purchase = new Purchase
                    {
                        SupplierId = dto.SupplierId,
                        InvoiceNumber = dto.InvoiceNumber,
                        PurchaseDate = dto.PurchaseDate,
                        TotalAmount = dto.TotalAmount,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy
                    };
                    
                    await _purchaseRepository.AddAsync(purchase);
                    await _purchaseRepository.SaveChangesAsync();

                    foreach (var medicine in dto.Medicines)
                    {
                        var batch = new Batch
                        {
                            MedicineId = medicine.MedicineId,
                            PurchaseId = purchase.PurchaseId,
                            BatchNumber = medicine.BatchNumber,
                            ManufactureDate = medicine.ManufactureDate,
                            ExpiryDate = medicine.ExpiryDate,
                            QuantityReceived = medicine.QuantityReceived,
                            CurrentStock = medicine.QuantityReceived,
                            UnitPrice = medicine.UnitPrice,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = createdBy
                        };
                        
                        await _batchRepository.AddAsync(batch);
                        await _batchRepository.SaveChangesAsync();

                        var transaction = new StockTransaction
                        {
                            BatchId = batch.BatchId,
                            UserId = id,
                            TransactionType = TransactionType.IN,
                            Quantity = medicine.QuantityReceived,
                            TransactionDate = dto.PurchaseDate,
                            Remarks = $"Stock received from Supplier ID: {dto.SupplierId} on Invoice: {dto.InvoiceNumber}"
                        };
                        
                        await _stockTransactionRepository.AddAsync(transaction);
                        await _stockTransactionRepository.SaveChangesAsync();
                    }
                    
                    scope.Complete();

                    return new ApiResponseDto<bool>
                    {
                        Success = true,
                        Message = "Invoice processed: Purchases, Batches, and Transactions recorded successfully.",
                        Data = true
                    };
                }
                catch (Exception ex)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = $"Failed to process purchase: {ex.Message}",
                        Data = false
                    };
                }
            }
        }

        public async Task<ApiResponseDto<PaginatedListDto<PurchaseDto>>> GetAllPurchasesAsync(int page, int pageSize, string? search)
        {
            Expression<Func<Purchase, bool>> predicate = p => !p.IsDeleted;
            var suppliers = await _supplierRepository.GetAllAsync();
    
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                predicate = p => !p.IsDeleted && 
                (p.InvoiceNumber.ToLower().Contains(lowerSearch) || 
                (p.Supplier != null && p.Supplier.SupplierName.ToLower().Contains(lowerSearch))); 
            }

            var (purchases, totalCount) = await _purchaseRepository.GetPagedAsync(predicate, page, pageSize);
    
            var purchaseDtos = purchases.Select(p => new PurchaseDto
            {
                PurchaseId = p.PurchaseId,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier.SupplierName ?? "Unknown",
                InvoiceNumber = p.InvoiceNumber,
                PurchaseDate = p.PurchaseDate,
                TotalAmount = p.TotalAmount
            }).ToList();

            var pagedResult = new PaginatedListDto<PurchaseDto> 
            { 
                Items = purchaseDtos, 
                TotalCount = totalCount, 
                CurrentPage = page, 
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) 
            };

            return ApiResponseDto<PaginatedListDto<PurchaseDto>>.SuccessResponse(pagedResult, "Purchases retrieved successfully.");
        }
    }
}