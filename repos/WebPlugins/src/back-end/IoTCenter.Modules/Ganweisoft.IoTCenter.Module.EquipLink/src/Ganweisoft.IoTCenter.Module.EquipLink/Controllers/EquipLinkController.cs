// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Models;
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiExplorerSettings(GroupName = SwaggerApiGroup.EquipLink)]
    public class EquipLinkController : DefaultController
    {
        private readonly IEquipLinkService _equipLinkService;
        private readonly IStringLocalizer<EquipLinkController> _stringLocalizer;
        public EquipLinkController(
            IEquipLinkService equipLinkService,
            IStringLocalizer<EquipLinkController> stringLocalizer)
        {
            _equipLinkService = equipLinkService;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost]
        public async Task<OperateResult<PageResult<EquipLinkListResponse>>> GetEquipLinkListByPage(GetEquipLinkModel equipLinkModel)
        {
            return await _equipLinkService.GetEquipLinkListByPage(equipLinkModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddEquipLinkData(AddEquipLinkModel addEquipLinkModel)
        {
            if (addEquipLinkModel.Delay.ToString().Length >= 11)
            {
                return OperateResult.Failed(_stringLocalizer["延时最大值不能超过10位数"]);
            }
            return await _equipLinkService.AddEquipLinkData(addEquipLinkModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditEquipLinkData(EditEquipLinkModel editEquipLinkModel)
        {
            if (editEquipLinkModel.Delay.ToString().Length >= 11)
            {
                return OperateResult.Failed(_stringLocalizer["延时最大值不能超过10位数"]);
            }

            return await _equipLinkService.EditEquipLinkData(editEquipLinkModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelEquipLinkData(int id)
        {
            return await _equipLinkService.DelEquipLinkData(id);
        }

        [HttpPost]
        public async Task<OperateResult<TEquipAndOEquiepListResponse>> GetIEquipAndOEquiepList()
        {
            return await _equipLinkService.GetIEquipAndOEquiepList();
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<EqulpLinkSence>>> GetSceneListByPage(CommonSearchPageModel commonSearchPageModel)
        {
            if (commonSearchPageModel == null)
            {
                commonSearchPageModel = new CommonSearchPageModel();
            }

            return await _equipLinkService.GetSceneListByPage(commonSearchPageModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddSceneLinkData(AddSceneModel addSceneModel)
        {
            return await _equipLinkService.AddSceneLinkData(addSceneModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditSceneLinkData(UpdateSceneModel updateSceneModel)
        {
            return await _equipLinkService.EditSceneLinkData(updateSceneModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelSceneLinkData(DelSceneModel delSceneModel)
        {
            return await _equipLinkService.DelSceneLinkData(delSceneModel);
        }
    }
}
