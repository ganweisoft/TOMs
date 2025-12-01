// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Event;

public interface IEventService
{
    Task<OperateResult> RecordLoginEvent();
    OperateResult<PagedResult<EquipEventResponse>> GetEquipEvtByPage(EquipEvtModel equipEvtModel);

    Task<OperateResult<PagedResult<SysEventResonse>>> GetSysEvtByPage(SysEvtModel sysEvtModel);

    Task<OperateResult> GetSysEvtCollection(SysEvtType sysEvtType, DateType dateType);
}
