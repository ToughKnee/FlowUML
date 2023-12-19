using CleanArchitectureWorkshop.Domain.Core;

namespace CleanArchitectureWorkshop.Domain.TeamAggregate;
public class UserName : ValueObject
{
    public string Value { get; }

    private UserName(string value)
    {
        Value = value;
    }

    // Data Validation, use the private constructor to process through another method the attributes of the class to make impossible invalida data
    public static UserName Create(string value)
    {
        // We enforce invariants - Apply the data restrictions according to business logic
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("User name cant be empty or whitespace", nameof(value));
        }
        if (value.Contains('/'))
        {
            throw new ArgumentException("User name cant contain '/'", nameof(value));
        }

        return new UserName(value);
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
