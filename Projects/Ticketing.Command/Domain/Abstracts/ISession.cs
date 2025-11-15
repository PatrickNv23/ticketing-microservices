using MongoDB.Driver;

namespace Ticketing.Command.Domain.Abstracts;

public interface ISession
{
    Task<IClientSessionHandle> BeginSessionAsync(CancellationToken cancellationToken = default);
    void BeginTransaction(IClientSessionHandle clientSessionHandle);
    Task CommitTransactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken = default);
    void DisposeSession(IClientSessionHandle clientSessionHandle);
}