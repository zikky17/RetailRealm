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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader order)
        {
            _db.OrderHeaders.Update(order);
        }
    }
}
