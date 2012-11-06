﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace GridMvc.Columns
{
    /// <summary>
    /// Collection of collumns
    /// </summary>
    public class GridColumnCollection<T> : KeyedCollection<string, IGridColumn>, IGridColumnCollection<T>
    {
        private readonly IColumnBuilder<T> _columnBuilder;

        public GridColumnCollection(IColumnBuilder<T> columnBuilder)
        {
            _columnBuilder = columnBuilder;
        }

        public bool DefaultSortEnabled { get; set; }
        public bool DefaultFilteringEnabled { get; set; }

        #region IGridColumnCollection<T> Members

        public IGridColumn<T> Add()
        {
            return Add(false);
        }

        public IGridColumn<T> Add(bool hidden)
        {
            return Add((Expression<Func<T, string>>) null, hidden);
        }

        public IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint)
        {
            return Add(constraint, false);
        }

        public IGridColumn<T> Add<TKey>(Expression<Func<T, TKey>> constraint, bool hidden)
        {
            IGridColumn<T> newColumn = _columnBuilder.CreateColumn(constraint, hidden);
            return Add(newColumn);
        }

        public IGridColumn<T> Add(PropertyInfo pi)
        {
            IGridColumn<T> newColumn = _columnBuilder.CreateColumn(pi);
            if (newColumn == null) return null;
            return Add(newColumn);
        }

        public IGridColumn<T> Add(IGridColumn<T> column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            column.Sortable(DefaultSortEnabled);
            column.Filterable(DefaultFilteringEnabled);

            try
            {
                base.Add(column);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(string.Format("Column '{0}' already exist in the grid", column.Name));
            }
            //ProcessColumn();
            return column;
        }

        public IGridColumn<T> Insert(int position, IGridColumn<T> column)
        {
            column.Sortable(DefaultSortEnabled);
            column.Filterable(DefaultFilteringEnabled);
            base.Insert(position, column);
            //ProcessColumn();
            return column;
        }

        public IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint)
        {
            return Insert(position, constraint, false);
        }

        public IGridColumn<T> Insert<TKey>(int position, Expression<Func<T, TKey>> constraint, bool hidden)
        {
            IGridColumn<T> newColumn = _columnBuilder.CreateColumn(constraint, hidden);
            return Insert(position, newColumn);
        }

        public new IEnumerator<IGridColumn> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion

        protected override string GetKeyForItem(IGridColumn item)
        {
            return item.Name;
        }
    }
}