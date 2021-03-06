using System.Data;
using System.Data.Common;
using Infrastructure.Interfaces;
using Z.BulkOperations;

namespace Infrastructure;

public static class RepositoryExtensions
{
    public static BulkOperation<T> GetBulkOperation<T>(this IDbConnection connection, string tableName, IUnitOfWork unitOfWork)
        where T : BaseEntity
    {
        var bulkOperation = new BulkOperation<T>((DbConnection)connection);
        bulkOperation.DestinationTableName = tableName;
        bulkOperation.MatchNamesWithUnderscores = true;
        bulkOperation.CaseSensitive = CaseSensitiveType.Insensitive;
        bulkOperation.Transaction = (DbTransaction)unitOfWork.Transaction;

        return bulkOperation;
    }
}
