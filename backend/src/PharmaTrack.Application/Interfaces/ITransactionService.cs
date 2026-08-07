using PharmaTrack.Application.DTOs.Transaction;
using PharmaTrack.Application.DTOs.Common;

namespace PharmaTrack.Application.Interfaces;
public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
}