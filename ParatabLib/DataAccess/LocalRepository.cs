using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace ParatabLib.DataAccess
{
    /* This class is inferface realization from IGenericRepository<TEntity> interface
     * purpose of this class is use as generic class and to handle database object for each model class.
     */ 
    public class LocalRepository<TEntity>:IGenericRepository<TEntity> where TEntity:class
    {
        private DbSet<TEntity> db;
        private LibraryContext context;

        public LocalRepository(LibraryContext context)
        {
            this.context = context;
            db = context.Set<TEntity>();
        }

        //This method use to receive all TEntity type object in database as list.
         public virtual List<TEntity> List()
        {
            return db.AsNoTracking<TEntity>().ToList();
        }

         //This method use to receive related TEntity type object in database base on condition parameter as list.
        public virtual List<TEntity> ListWhere(Func<TEntity, bool> condition)
        {
            return db.AsNoTracking().ToList().Where(condition).ToList();
        }

        //This method use to receive one TEntity type object in database base on id parameter.
        public virtual TEntity Find(int id)
        {
            return db.Find(id);
        }

        /* This method use to add new record(e.g. item parameter) for TEntity type 
         * into database set object(db properties in this class).
         */ 
        public virtual void Add(TEntity item)
        {
            db.Add(item);
        }

        /* This method use to update desired record(e.g. item parameter) for TEntity type 
         * into database set object(db properties in this class).
         */ 
        public virtual void Update(TEntity item)
        {
            db.Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }

        /* This method use to remove individual object in database(e.g. item parameter) for TEntity type 
         * from database set object(db properties in this class).
         */ 
        public virtual void Remove(TEntity item)
        {
            db.Attach(item);
            db.Remove(item);
        }

        /* This method use to remove range of object in database(e.g. removeList parameter) for TEntity type 
         * from database set object(db properties in this class).This method will use foreach all call remove method that
         * remove individual object until it reach last object in removelist.
         */ 
        public virtual void Remove(List<TEntity> removeList)
        {
            foreach (var item in removeList)
                Remove(item);
        }
    }
}