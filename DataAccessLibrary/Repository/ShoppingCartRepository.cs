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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.ShoppingsCarts.Update(shoppingCart);
        }
    }
}
