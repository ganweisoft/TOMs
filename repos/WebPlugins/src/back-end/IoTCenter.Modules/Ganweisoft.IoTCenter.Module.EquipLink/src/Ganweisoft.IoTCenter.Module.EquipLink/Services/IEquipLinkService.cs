// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.EquipLink.Models;
using IoTCenter.Utilities;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public interface IEquipLinkService
    {
        Task<OperateResult<PageResult<EquipLinkListResponse>>> GetEquipLinkListByPage(GetEquipLinkModel equipLinkModel);

        Task<OperateResult> AddEquipLinkData(AddEquipLinkModel addEquipLinkModel);

        Task<OperateResult> EditEquipLinkData(EditEquipLinkModel editEquipLinkModel);

        Task<OperateResult> DelEquipLinkData(int id);

        Task<OperateResult<TEquipAndOEquiepListResponse>> GetIEquipAndOEquiepList();

        Task<OperateResult<PagedResult<EqulpLinkSence>>> GetSceneListByPage(CommonSearchPageModel commonSearchPageModel);

        Task<OperateResult> AddSceneLinkData(AddSceneModel addSceneModel);

        Task<OperateResult> EditSceneLinkData(UpdateSceneModel updateSceneModel);

        Task<OperateResult> DelSceneLinkData(DelSceneModel delSceneModel);
    }
}
