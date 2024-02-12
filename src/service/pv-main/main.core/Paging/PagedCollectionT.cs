using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Paging
{
	// TODO: This should implement ICollection, not compose it.
	public class PagedCollection<T> : List<T>
	{
        public PagedCollection()
        {
        }

        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }

        public PagedCollection(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedCollection<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedCollection<T>(items, count, pageNumber, pageSize);
        }

        public static PagedCollection<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedCollection<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Create a new pagedcollection of type T using a previously created collection
        /// </summary>
        /// <param name="source">The previously created paged collection</param>
        /// <param name="pageNumber">The current page number</param>
        /// <param name="pageSize">Number of records per page</param>
        /// <param name="totalCount">The total number of records in the original collection before it was paged</param>
        /// <returns></returns>
        public static PagedCollection<T> Create(PagedCollection<T> source, int pageNumber, int pageSize, int totalCount)
        {
            var count = totalCount;
            return new PagedCollection<T>(source, count, pageNumber, pageSize);
        }
    }
}