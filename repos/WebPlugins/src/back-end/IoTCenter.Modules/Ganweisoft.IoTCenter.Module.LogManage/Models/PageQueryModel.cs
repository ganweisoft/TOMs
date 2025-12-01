// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenter.Utilities
{
    public class PageQueryModel
    {
        private int pageNo = 1;
        private int pageSize = 20;

        public int PageNo
        {
            get
            {
                return pageNo;
            }
            set
            {
                if (value <= 0)
                {
                    pageNo = 1;
                }
                else
                {
                    pageNo = value;
                }
            }
        }
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value <= 0)
                {
                    pageSize = 20;
                }
                else if (value > 100)
                {
                    pageSize = 100;
                }
                else
                {
                    pageSize = value;
                }
            }
        }

        public void SetMaxPageSize(int maxPageSize)
        {
            if (maxPageSize <= 0)
            {
                return;
            }
            if (maxPageSize <= 100)
            {
                return;
            }
            this.pageSize = maxPageSize;
        }
    }
}
