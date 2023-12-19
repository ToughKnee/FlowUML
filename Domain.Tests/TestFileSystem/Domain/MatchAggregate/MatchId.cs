using CleanArchitectureWorkshop.Domain.Core;

namespace CleanArchitectureWorkshop.Domain.MatchAggregate;
public class MatchId : ValueObject
{
    public string Value { get; }

    private MatchId(string value)
    {
        Value = value;
    }

    public static MatchId CreateUnique()
    {
        return new MatchId(Guid.NewGuid().ToString());
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
