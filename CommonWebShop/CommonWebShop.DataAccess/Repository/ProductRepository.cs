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
            //_db.Products.Update(product);
            var productFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Title = product.Title;
                productFromDb.Author = product.Author;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.Price10 = product.Price10;
                productFromDb.CategoryId = product.CategoryId;
                if(productFromDb.ImageUrl != null) 
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
