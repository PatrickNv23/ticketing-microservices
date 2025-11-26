using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Infrastructure.Repositories;

public class MongoRepository<TDocument>(IMongoClient mongoClient, IOptions<MongoSettings> options) : IMongoRepository<TDocument>
    where TDocument : IDocument
{
    private readonly IMongoCollection<TDocument> _collection = mongoClient
        .GetDatabase(options.Value.Database)
        .GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

    private static string GetCollectionName(Type documentType)
    {
        var name = documentType
            .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
            .FirstOrDefault();
        return name is not null 
            ? ((BsonCollectionAttribute)name).CollectionName 
            : throw new ArgumentException($"Collection {documentType} not found");
    }

    public async Task<IClientSessionHandle> BeginSessionAsync(CancellationToken cancellationToken = default)
    {
        var option = new ClientSessionOptions
        {
            DefaultTransactionOptions = new TransactionOptions()
        };
        return await _collection.Database.Client.StartSessionAsync(option, cancellationToken);
    }

    public void BeginTransaction(IClientSessionHandle clientSessionHandle) => clientSessionHandle.StartTransaction();

    public async Task CommitTransactionAsync(
        IClientSessionHandle clientSessionHandle, 
        CancellationToken cancellationToken = default) 
        => await clientSessionHandle.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(
        IClientSessionHandle clientSessionHandle,
        CancellationToken cancellationToken = default)
        => await clientSessionHandle.AbortTransactionAsync(cancellationToken);

    public void DisposeSession(IClientSessionHandle clientSessionHandle) =>  clientSessionHandle.Dispose();

    public IQueryable<TDocument> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public async Task InsertOneAsync(
        TDocument document, 
        IClientSessionHandle clientSessionHandle,
        CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(clientSessionHandle, document, null, cancellationToken);
    }

    public async Task<IEnumerable<TDocument>> FilterByAsync(
        Expression<Func<TDocument, bool>> filterExpression, 
        CancellationToken cancellationToken = default)
    {
        var result = await _collection.FindAsync(filterExpression, null, cancellationToken);
        var resultList = await result.ToListAsync();
        
        return resultList.Any() ? resultList : Enumerable.Empty<TDocument>();
    }
}