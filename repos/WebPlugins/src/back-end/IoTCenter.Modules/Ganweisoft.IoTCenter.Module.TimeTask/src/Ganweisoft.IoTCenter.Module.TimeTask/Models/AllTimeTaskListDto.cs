// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class AllTimeTaskListDto
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string TableType { get; set; }
        public string Remark { get; set; }
    }

    public class CompareTaskList : IComparer<string>
    {
        public int Compare([AllowNull] string x, [AllowNull] string y)
        {
            if (x == TaskType.T.ToString() && y == TaskType.C.ToString())
            {
                return 1;
            }
            else if (x == TaskType.C.ToString() && y == TaskType.T.ToString())
            {
                return -1;
            }

            return string.Compare(x, y, true);
        }
    }
}
