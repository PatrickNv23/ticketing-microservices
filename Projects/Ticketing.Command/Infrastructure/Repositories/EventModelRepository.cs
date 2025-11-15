using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Repositories;

public class EventModelRepository(
    IMongoClient mongoClient, 
    IOptions<MongoSettings> options) 
    : MongoRepository<EventModel>(mongoClient, options), IEventModelRepository
{ }