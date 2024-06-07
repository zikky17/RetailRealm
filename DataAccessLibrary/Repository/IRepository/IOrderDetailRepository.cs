using ModelsLibrary.Models;

namespace DataAccessLibrary.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail order);
    }
}
