// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class ModelConfigController : DefaultController
    {
        private readonly IModelConfigService _modelConfigService;
        public ModelConfigController(IModelConfigService modelConfigService)
        {
            _modelConfigService = modelConfigService;
        }

        [HttpPost]
        public OperateResult<PagedResult<AllEquipDataList>> GetAllEquipDataList(GetEquipDataListModel getAllEquipDataListModel)
        {
            return _modelConfigService.GetAllEquipDataList(getAllEquipDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<IotEquip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel)
        {
            return await _modelConfigService.GetEquipDataByEquipNo(getEquipDataByNoModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddEquipData(EquipDataModel equipDataModel)
        {
            return await _modelConfigService.AddEquipData(equipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditEquipData(EquipDataModel equipDataModel)
        {
            return await _modelConfigService.EditEquipData(equipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelEquipData([FromBody] DelEquipDataModel delEquipDataModel)
        {
            return await _modelConfigService.DelEquipData(delEquipDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<IotYcp>>> GetYcpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _modelConfigService.GetYcpDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<IotYcp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return await _modelConfigService.GetYcpDataByEquipYcNo(equipYcYxSetNoModel);
        }

        [HttpGet]
        [Route("/IoT/api/v3/ModelConfig/ycyxset-num")]
        public async Task<OperateResult> GetYcYxSetNumByEquipNo([FromQuery] int equipNo)
        {
            if (equipNo <= 0)
            {
                return OperateResult.Failed("设备号不存在");
            }

            return await _modelConfigService.GetYcYxSetNumByEquipNo(equipNo);
        }

        [HttpPost]
        public async Task<OperateResult> AddYcpData(YcpDataModel ycpDataModel)
        {
            return await _modelConfigService.AddYcpData(ycpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditYcpData(YcpDataModel ycpDataModel)
        {
            return await _modelConfigService.EditYcpData(ycpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _modelConfigService.DelYcpData(delYcYxSetDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<IotYxp>>> GetYxpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _modelConfigService.GetYxpDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public async Task<OperateResult<IotYxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return await _modelConfigService.GetYxpDataByEquipYxNo(equipYcYxSetNoModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddYxpData(YxpDataModel yxpDataModel)
        {
            return await _modelConfigService.AddYxpData(yxpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditYxpData(YxpDataModel yxpDataModel)
        {
            return await _modelConfigService.EditYxpData(yxpDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _modelConfigService.DelYxpData(delYcYxSetDataModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<IotSetParm>>> GetSetParmDataList(GetYcYxSetDataListModel getYcYxSetDataListModel)
        {
            return await _modelConfigService.GetSetParmDataList(getYcYxSetDataListModel);
        }

        [HttpPost]
        public Task<OperateResult<IotSetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel)
        {
            return _modelConfigService.GetSetParmDataByEquipSetNo(equipYcYxSetNoModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddSetData(SetDataModel setDataModel)
        {
            return await _modelConfigService.AddSetData(setDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditSetData(SetDataModel setDataModel)
        {
            return await _modelConfigService.EditSetData(setDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel)
        {
            return await _modelConfigService.DelSetData(delYcYxSetDataModel);
        }
    }
}
