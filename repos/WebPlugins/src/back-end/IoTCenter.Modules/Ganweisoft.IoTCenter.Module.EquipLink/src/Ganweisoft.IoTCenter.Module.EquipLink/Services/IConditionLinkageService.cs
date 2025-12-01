// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Models;
using Ganweisoft.IoTCenter.Module.EquipLink.Models.Condition;
using IoTCenter.Utilities;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public interface IConditionLinkageService
    {
        Task<OperateResult<PageResult<EquipLinkListResponseEx>>> GetConditionLinkListByPage(GetEquipLinkModel equipLinkModel);

        Task<OperateResult> AddConditionLinkData(AddConditionModel addSceneModel);

        Task<OperateResult> EditConditionLinkData(EditConditionModel updateModel);

        Task<OperateResult> DelConditionLinkData(DelConditionModel delSceneModel);

        Task<OperateResult<GetConditionResponse>> GetConditionLinkByAutoProcId(GetConditionModel getConditionModel);

        Task<OperateResult<GetEquipYcYxpResponse>> GetEquipYcYxps(GetEquipYcYxpModel model);
    }
}