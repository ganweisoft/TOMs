//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction
{
    public class QueryLogRequest
    {
        private int? pageNo;
        private int? pageSize;

        public string LogFileName { get; set; } = string.Empty;

        public int? PageNo
        {
            get
            {
                return pageNo;
            }
            set
            {
                if (pageNo <= 0)
                {
                    pageNo = 1;
                }
                else
                {
                    if (value == null)
                    {
                        pageNo = 1;
                    }
                    else
                    {
                        pageNo = value;
                    }

                }
            }
        }
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (pageSize <= 0)
                {
                    pageSize = 10;
                }
                else
                {
                    if (value == null || !int.TryParse(value.ToString(), out var pg) || pg <= 0)
                    {
                        pageSize = 10;
                    }
                    else
                    {
                        pageSize = value;
                    }

                }
            }
        }
    }
}
