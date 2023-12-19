using CleanArchitectureWorkshop.Domain.Core;

namespace CleanArchitectureWorkshop.Application.Repositories;

public interface IRepository<TAggregateRoot, TId>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : ValueObject
{
}
