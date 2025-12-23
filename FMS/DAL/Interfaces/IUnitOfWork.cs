namespace FMS.DAL.Interfaces
{
    //gom het tat ca repository vao 1 transaction duy nhat de tranh viec save changes nhieu lan
    public interface IUnitOfWork : IAsyncDisposable
    {

        IUserRepository Users { get; }
        IDriverRepository Drivers { get; }
        IVehicleRepository Vehicles { get; }
        ITripRepository Trips { get; }

        IDriverLicenseRepository DriverLicenses { get; }

        IExtraExpenseRepository ExtraExpenses { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

