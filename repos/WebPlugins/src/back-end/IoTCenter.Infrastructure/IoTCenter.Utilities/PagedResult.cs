// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenter.Utilities
{
    public class PagedResult<T> where T : class
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Rows { get; set; }

        public string List { get; set; }

        public static PagedResult<T> Create(int totalCount, IEnumerable<T> rows, string json = "")
        {
            return new PagedResult<T>()
            {
                TotalCount = totalCount,
                Rows = rows,
                List = json
            };
        }

        [Obsolete("该方法已过时,为兼容此前历史遗留问题而有所保留，请使用重载方法以对象返回")]
        public static PagedResult<T> Create(int totalCount = 0, string json = "")
        {
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                List = json
            };
        }
    }
}
