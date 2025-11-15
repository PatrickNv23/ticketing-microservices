using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ticketing.Command.Domain.Common;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    // representa el identificador único de un documento en mongoDB
    ObjectId Id { get; set; }
}