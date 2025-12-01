// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using Ganweisoft.IoTCenter.Module.EquipConfig;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig
{
    public interface IModelConfigService
    {
        OperateResult<PagedResult<AllEquipDataList>> GetAllEquipDataList(GetEquipDataListModel getAllEquipDataListModel);

        Task<OperateResult<IotEquip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel);

        Task<OperateResult> AddEquipData(EquipDataModel equipDataModel);

        Task<OperateResult> EditEquipData(EquipDataModel equipDataModel);

        Task<OperateResult> DelEquipData(DelEquipDataModel delEquipDataModel);

        Task<OperateResult<PagedResult<IotYcp>>> GetYcpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

        Task<OperateResult<IotYcp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

        Task<OperateResult> AddYcpData(YcpDataModel ycpDataModel);

        Task<OperateResult> EditYcpData(YcpDataModel ycpDataModel);

        Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel);

        Task<OperateResult<PagedResult<IotYxp>>> GetYxpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

        Task<OperateResult<IotYxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

        Task<OperateResult> AddYxpData(YxpDataModel yxpDataModel);

        Task<OperateResult> EditYxpData(YxpDataModel yxpDataModel);

        Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel);

        Task<OperateResult<PagedResult<IotSetParm>>> GetSetParmDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

        Task<OperateResult<IotSetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

        Task<OperateResult> AddSetData(SetDataModel setDataModel);

        Task<OperateResult> EditSetData(SetDataModel setDataModel);

        Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel);

        Task<OperateResult> GetYcYxSetNumByEquipNo(int equipNo);

        Task<OperateResult<PageResult<SubsystemTypeEquip>>> GetSubsystemTypeEquipsAsync(QueryRequest request);
    }
}
