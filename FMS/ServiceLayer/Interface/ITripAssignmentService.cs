namespace FMS.ServiceLayer.Interface
{
    public interface ITripAssignmentService
    {
        Task AssignVehicleAndDriverAsync(int tripId);
    }
}
