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

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
          var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderId == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
            }
            if(!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.OrderId == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
