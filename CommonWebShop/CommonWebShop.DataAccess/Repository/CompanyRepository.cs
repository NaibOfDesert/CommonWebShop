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
    internal class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
