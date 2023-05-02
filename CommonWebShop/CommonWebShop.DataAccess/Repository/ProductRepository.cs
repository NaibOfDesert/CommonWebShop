using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWebShop.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        public ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;   
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
