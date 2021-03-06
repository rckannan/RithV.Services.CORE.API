using RithV.Services.CORE.API.Domain;
namespace RithV.Services.CORE.API.Domain
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
