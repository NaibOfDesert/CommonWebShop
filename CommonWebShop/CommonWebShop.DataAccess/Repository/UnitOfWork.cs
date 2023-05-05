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
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext _db;
        public ICategoryRepository category { get; private set; }

        public IProductRepository product { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            category = new CategoryRepository(db);  
            product = new ProductRepository(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
    