﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GridMvc.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridMvc.Tests.Sorting
{
    /// <summary>
    /// Summary description for SortTests
    /// </summary>
    [TestClass]
    public class SortTests
    {
        private TestGrid _grid;
        private TestRepository _repo;

        [TestInitialize]
        public void Init()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter()));

            _repo = new TestRepository();
            _grid = new TestGrid(_repo.GetAll());
        }

        [TestMethod]
        public void TestSortingStringDescending()
        {
            _grid.Columns.Add(x => x.Title).Sortable(true);
            if (
                !ValidateSorting<string, string, object>(_grid, x => x.Title, x => x.Title, "Title",
                                                         GridSortDirection.Descending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingStringAscending()
        {
            _grid.Columns.Add(x => x.Title).Sortable(true);
            if (
                !ValidateSorting<string, string, object>(_grid, x => x.Title, x => x.Title, "Title",
                                                         GridSortDirection.Ascending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntAscending()
        {
            _grid.Columns.Add(x => x.Id).Sortable(true);
            if (
                !ValidateSorting<int, int, object>(_grid, x => x.Id, x => x.Id, "Id", GridSortDirection.Ascending, null,
                                                   null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingIntDescending()
        {
            _grid.Columns.Add(x => x.Id).Sortable(true);
            if (
                !ValidateSorting<int, int, object>(_grid, x => x.Id, x => x.Id, "Id", GridSortDirection.Descending, null,
                                                   null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle).Sortable(true);
            if (
                !ValidateSorting<string, string, object>(_grid, x => x.Child.ChildTitle, x => x.Child.ChildTitle,
                                                         "Child__ChildTitle", GridSortDirection.Ascending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildStringDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildTitle).Sortable(true);
            if (
                !ValidateSorting<string, string, object>(_grid, x => x.Child.ChildTitle, x => x.Child.ChildTitle,
                                                         "Child__ChildTitle", GridSortDirection.Descending, null, null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true);
            if (
                !ValidateSorting<DateTime, DateTime, object>(_grid, x => x.Child.ChildCreated, x => x.Child.ChildCreated,
                                                             "Child__ChildCreated", GridSortDirection.Descending, null,
                                                             null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingChildDateTimeAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true);
            if (
                !ValidateSorting<DateTime, DateTime, object>(_grid, x => x.Child.ChildCreated, x => x.Child.ChildCreated,
                                                             "Child__ChildCreated", GridSortDirection.Ascending, null,
                                                             null))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByAscending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortBy(x => x.Title);
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated, x => x.Title, "Child__ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Ascending))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        [TestMethod]
        public void TestSortingThenByDescending()
        {
            _grid.Columns.Add(x => x.Child.ChildCreated).Sortable(true).ThenSortByDescending(x => x.Title);
            if (
                !ValidateSorting(_grid, x => x.Child.ChildCreated, x => x.Title, "Child__ChildCreated",
                                 GridSortDirection.Ascending, x => x.Title, GridSortDirection.Descending))
            {
                Assert.Fail("Sort works incorrect");
            }
        }

        private bool ValidateSorting<T, TSelect, TNext>(TestGrid grid, Func<TestModel, T> orderExpression,
                                                        Func<TestModel, TSelect> selectExpression, string columnName,
                                                        GridSortDirection direction,
                                                        Func<TestModel, TNext> thenByExpression,
                                                        GridSortDirection? thenByDirection)
        {
            grid.Sorting = new TestSortProvider(columnName, direction);

            IEnumerable<TSelect> resultCollection = _grid.ItemsToDisplay.OfType<TestModel>().Select(selectExpression);
            IOrderedEnumerable<TestModel> etalonCollection;
            switch (direction)
            {
                case GridSortDirection.Ascending:
                    etalonCollection = _repo.GetAll().OrderBy(orderExpression);
                    break;
                case GridSortDirection.Descending:
                    etalonCollection = _repo.GetAll().OrderByDescending(orderExpression);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
            if (thenByExpression != null)
            {
                switch (thenByDirection)
                {
                    case GridSortDirection.Ascending:
                        etalonCollection = etalonCollection.ThenBy(thenByExpression);
                        break;
                    case GridSortDirection.Descending:
                        etalonCollection = etalonCollection.ThenByDescending(thenByExpression);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("thenByDirection");
                }
            }

            if (!ValidateCollectionsTheSame(resultCollection, etalonCollection.Select(selectExpression)))
            {
                return false;
            }
            return true;
        }

        //TODO thenby sort testing...

        private bool ValidateCollectionsTheSame<T>(IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            for (int i = 0; i < collection1.Count(); i++)
            {
                if (!collection1.ElementAt(i).Equals(collection2.ElementAt(i)))
                    return false;
            }
            return true;
        }
    }
}