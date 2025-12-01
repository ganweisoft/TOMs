// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Models
{
    public class EditProcTaskWeekModel
    {
        public WeekDataInfo Mon { get; set; }

        public WeekDataInfo Tues { get; set; }

        public WeekDataInfo Wed { get; set; }

        public WeekDataInfo Thurs { get; set; }

        public WeekDataInfo Fri { get; set; }

        public WeekDataInfo Sat { get; set; }

        public WeekDataInfo Sun { get; set; }
    }

    public class WeekDataInfo
    {
        public List<GetProcTaskWeekModel> tProcTaskLists { get; set; }
        public List<GetProcTaskWeekModel> cProcTaskLists { get; set; }
    }
}
