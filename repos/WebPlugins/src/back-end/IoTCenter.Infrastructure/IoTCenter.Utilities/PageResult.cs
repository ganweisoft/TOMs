// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenter.Utilities
{
    public class PageResult<T> where T : class
    {
        public int Total { get; set; }
        public IEnumerable<T> Rows { get; set; }

        public static PageResult<T> Create(int totalCount = 0, IEnumerable<T> rows = default)
        {
            return new PageResult<T>()
            {
                Total = totalCount,
                Rows = rows
            };
        }
    }
}
