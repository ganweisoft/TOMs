// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.Event
{
    /// <summary>
    /// 获取设备事件请求参数
    /// </summary>
    public class EquipEventQueryRequest : QueryRequest
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 选中设备号，逗号分隔
        /// </summary>
        public IEnumerable<int> EquipNos { get; set; }

        /// <summary>
        /// 事件类型 Y 遥测事件 X 遥信事件 S 设置事件
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 是否全选
        /// </summary>
        public bool IsAll { get; set; }
    }

    public class DeleteQueryResquest
    {
        //
        // 摘要:
        //     开始时间
        public DateTime beginTime
        {
            get;
            set;
        }

        //
        // 摘要:
        //     结束时间
        public DateTime endTime
        {
            get;
            set;
        }

        //
        // 摘要:
        //     
        public int[] equipNos
        {
            get;
            set;
        }



        //
        // 摘要:
        //     事件类型 YC_Event = 0,YX_Event = 1, Equip_Event = 2,SetParm_Event = 3,System_Event
        //     = 4
        public string eventType
        {
            get;
            set;
        }
    }
}
