namespace Ticketing.Query.Domain.Abstractions;

public class Entity(Guid id)
{
    protected Entity() : this(default) // para instanciarse cualquier tipo de clase heredada
    {}

    public Guid Id { get; set; } = id;
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedBy { get; set; }
}