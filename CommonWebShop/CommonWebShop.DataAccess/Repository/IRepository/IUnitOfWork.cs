using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWebShop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        // to collects all IRepositories
        ICategoryRepository category { get; }
        IProductRepository product { get; } 
        void Save();

    }
}
