// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Models;
using Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition;
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiExplorerSettings(GroupName = SwaggerApiGroup.EquipLink)]
    public class ConLinkageController : DefaultController
    {
        private readonly IConditionLinkageService _conditionLinkageService;

        public ConLinkageController(IConditionLinkageService conditionLinkageService)
        {
            _conditionLinkageService = conditionLinkageService;
        }

        [HttpGet]
        public async Task<OperateResult<PageResult<EquipLinkListResponseEx>>> GetConditionLinkListByPage(
            [FromQuery]GetEquipLinkModel commonSearchPageModel)
        {
            return await _conditionLinkageService.GetConditionLinkListByPage(commonSearchPageModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddConditionLinkData([FromBody]AddConditionModel addSceneModel)
        {
            return await _conditionLinkageService.AddConditionLinkData(addSceneModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditConditionLinkData([FromBody]EditConditionModel updateSceneModel)
        {
            return await _conditionLinkageService.EditConditionLinkData(updateSceneModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelConditionLinkData([FromBody]DelConditionModel delSceneModel)
        {
            return await _conditionLinkageService.DelConditionLinkData(delSceneModel);
        }

        [HttpGet]
        public async Task<OperateResult<GetConditionResponse>> GetConditionLinkByAutoProcId([FromQuery]GetConditionModel getConditionModel)
        {
            return await _conditionLinkageService.GetConditionLinkByAutoProcId(getConditionModel);
        }

        [HttpPost]
        public async Task<OperateResult<GetEquipYcYxpResponse>> GetEquipYcYxps([FromBody]GetEquipYcYxpModel model)
        {
            return await _conditionLinkageService.GetEquipYcYxps(model);
        }
    }
}