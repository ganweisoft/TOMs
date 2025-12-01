// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterCore.ExcelHelper;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Event.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class EventController : DefaultController
    {
        private readonly IExportManager _exportManager;
        private readonly IEventService _eventService;

        public EventController(IEventService eventService,
            IExportManager exportManager)
        {
            _eventService = eventService;
            _exportManager = exportManager;
        }

        [HttpPost]
        public OperateResult<PagedResult<EquipEventResponse>> GetEquipEvtByPage(EquipEvtModel equipEvtModel)
        {
            return _eventService.GetEquipEvtByPage(equipEvtModel);
        }

        [HttpGet]
        public async Task<OperateResult<PagedResult<SysEventResonse>>> GetSysEvtByPage([FromQuery] SysEvtModel sysEvtModel)
        {
            return await _eventService.GetSysEvtByPage(sysEvtModel);
        }

        [HttpGet]
        public async Task<OperateResult> GetSysEvtByType(DateType dateType)
        {
            return await _eventService.GetSysEvtCollection(SysEvtType.All, dateType);
        }
    }
}
