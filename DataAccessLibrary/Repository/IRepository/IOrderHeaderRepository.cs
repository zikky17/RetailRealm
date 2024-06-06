using ModelsLibrary.Models;

namespace DataAccessLibrary.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader order);
    }
}
