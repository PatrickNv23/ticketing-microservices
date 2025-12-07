using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketing.Query.Domain.Addresses;

// value object
[ComplexType]
public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}