using ServiceBus.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Diagnostics;
using ServiceBus.Data.ORM.EntityFramework;

namespace ServiceBus.Data.Implementation.DataAccess
{
    public class GenericRepo<T> where T : class
    {
        readonly AiroPayContext _context = new AiroPayContext();

        internal DbSet<T> DbSet;

        public GenericRepo(AiroPayContext context)
        {
            this.DbSet = _context.Set<T>();
            _context.Configuration.LazyLoadingEnabled = true;
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual T GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void SaveEntity(T entity)
        {
            //DbSet.Add(entity);

            using (_context)
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();

            }
        }

        public virtual void SaveEntity_NoError(T entity)
        {
            try
            {

                using (_context)
                {
                    _context.Set<T>().Add(entity);
                    _context.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex.Message}; {ex?.InnerException};{ex?.StackTrace}");
            }

        }

        public virtual void Delete(object id)
        {
            T entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void Update(T entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public IEnumerable<T> QueryObjectGraph(Expression<Func<T, bool>> filter, string children)
        {
            return DbSet.Include(children).Where(filter);
        }


    }
}
