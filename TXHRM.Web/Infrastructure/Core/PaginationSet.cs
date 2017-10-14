﻿using System.Collections.Generic;
using System.Linq;

namespace TXHRM.Web.Infrastructure.Core
{
    public class PaginationSet<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }

        public int Count
        {
            get
            {
                return (Items != null ? Items.Count() : 0);
            }
        }
    }
}