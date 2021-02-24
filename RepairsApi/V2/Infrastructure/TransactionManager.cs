using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.V2.Infrastructure
{
    public interface ITransactionManager
    {
        Task<ITransaction> Start();
    }

    public class TransactionManager : ITransactionManager
    {
        private readonly RepairsContext _repairsContext;

        public TransactionManager(RepairsContext repairsContext)
        {
            _repairsContext = repairsContext;
        }

        public async Task<ITransaction> Start()
        {
            return new Transaction(await _repairsContext.Database.BeginTransactionAsync());
        }
    }

    public interface ITransaction : IAsyncDisposable
    {
        Task Commit();
    }

    public class Transaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;
        private bool _commited;

        public Transaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
            _commited = false;
        }

        public Task Commit()
        {
            _commited = true;
            return _transaction.CommitAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (!_commited) await _transaction.RollbackAsync();
        }
    }
}
