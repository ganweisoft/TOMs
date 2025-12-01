// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ModelBatchConfigController : DefaultController
    {
        private readonly IModelConfigService _modelConfigService;
        public ModelBatchConfigController(IModelConfigService modelConfigService)
        {
            _modelConfigService = modelConfigService;
        }

        [HttpGet]
        public async Task<OperateResult<PageResult<SubsystemTypeEquip>>> GetSubsystemTypeEquipsAsync(
            [FromQuery] QueryRequest request)
        {
            return await _modelConfigService.GetSubsystemTypeEquipsAsync(request);
        }
    }
}