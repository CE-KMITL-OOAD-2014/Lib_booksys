using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace ParatabLib.DataAccess
{
    public class LocalRepository<TEntity>:IGenericRepository<TEntity> where TEntity:class
    {
        private DbSet<TEntity> db;
        private LibraryContext context;

        public LocalRepository(LibraryContext context)
        {
            this.context = context;
            db = context.Set<TEntity>();
        }
        public virtual List<TEntity> List()
        {
            return db.AsNoTracking<TEntity>().ToList();
        }

        public virtual List<TEntity> ListWhere(Func<TEntity, bool> condition)
        {
            return db.AsNoTracking().ToList().Where(condition).ToList();
        }

        public virtual TEntity Find(int id)
        {
            return db.Find(id);
        }

        public virtual void Add(TEntity item)
        {
            db.Add(item);
        }

        public virtual void Update(TEntity item)
        {
            db.Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }

        public virtual void Remove(TEntity item)
        {
            db.Attach(item);
            db.Remove(item);
        }

        public virtual void Remove(List<TEntity> removeList)
        {
            foreach (var item in removeList)
                Remove(item);
        }
    }
}