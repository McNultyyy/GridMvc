﻿using System.Data.Objects;
using System.Linq;

namespace GridMvc.Sample.Models
{
    public abstract class SqlRepository<T> : IRepository<T> where T : class
    {
        private readonly ObjectSet<T> _set;

        protected SqlRepository(ObjectContext context)
        {
            _set = context.CreateObjectSet<T>();
        }

        #region IRepository<T> Members

        public virtual IOrderedQueryable<T> GetAll()
        {
            return _set;
        }

        public abstract T GetById(object id);

        #endregion
    }
}