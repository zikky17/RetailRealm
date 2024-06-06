using DataAccessLibrary.Repository.IRepository;
using ModelsLibrary.Models;
using RetailRealm.DataAccessLibrary.Data;
using RetailRealm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail order)
        {
            _db.OrderDetails.Update(order);
        }
    }
}
