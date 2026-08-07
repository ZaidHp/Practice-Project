using PharmaTrack.Application.DTOs.Transaction;
using PharmaTrack.Application.DTOs.Common;
using PharmaTrack.Domain.Entities;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.Application.Services;
public class TransactionService : ITransactionService
{
    private readonly IGenericRepository<StockTransaction> _transactionRepository;

    public TransactionService(IGenericRepository<StockTransaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();
        var dtoList = transactions.Select(t => new TransactionDto
        {
            UserId = t.UserId,
            BatchId = t.BatchId,
            TransactionType = t.TransactionType,
            Quantity = t.Quantity,
            TransactionDate = t.TransactionDate,
            Remarks = t.Remarks,
            IsAdminOverride = t.IsAdminOverride
        });

        return dtoList;
    }
}