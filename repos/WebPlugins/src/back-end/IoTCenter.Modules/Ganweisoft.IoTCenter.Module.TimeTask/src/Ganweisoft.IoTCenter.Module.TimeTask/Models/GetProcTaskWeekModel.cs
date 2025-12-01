// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class GetProcTaskWeekModel
    {
        public int TableId { get; set; }

        public string TableName { get; set; }

        public string TableType { get; set; }

        public string Remark { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }
    }

    public class TaskDetailData
    {
        public int TableId { get; set; }

        public string Time { get; set; }

        public string Type { get; set; }
    }
}
