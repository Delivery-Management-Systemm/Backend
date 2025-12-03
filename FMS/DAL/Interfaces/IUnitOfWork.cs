using UITour.DAL.Interfaces.Repositories;

namespace FMS.DAL.Interfaces
{
    //gom het tat ca repository vao 1 transaction duy nhat de tranh viec save changes nhieu lan
    public interface IUnitOfWork : IAsyncDisposable
    {

        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

