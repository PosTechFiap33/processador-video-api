using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task Commit();
}

public interface IRepository<T> : IDisposable where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
