using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CommonWebShop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class 
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>(); // return access to T type units in _db
            //_db.Categories == dbSet
            //_db.Products.Include(p => p.Category); //change to call method IncludeProperties
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);    
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            query = IncludeProperties(query, includeProperties);
            return query.FirstOrDefault();
        }

        //Category, others...
        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = IncludeProperties(query, includeProperties);
            return query.ToList();  
        }

        private IQueryable<T> IncludeProperties(IQueryable<T> _query, string? includeProperties = null)
        {
            IQueryable<T> query = _query;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query;
        }
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
