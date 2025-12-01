// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public interface ISystemConfigService
{
    OperateResult<PageResult<GetEquipDataDataModel>> GetEquipDataList(GetEquipDataListModel getAllEquipDataListModel);

    Task<OperateResult<Equip>> GetEquipDataByEquipNo(GetEquipDataByNoModel getEquipDataByNoModel);

    OperateResult<List<List<string>>> GetEquipColumnData();

    Task<OperateResult> AddEquipData(EquipDataModel equipDataModel, int groupId);

    Task<OperateResult> EditEquipData(EquipDataModel equipDataModel);

    Task<OperateResult> DelEquipData(DelEquipDataModel delEquipDataModel);

    Task<OperateResult<PagedResult<YcpResponesModel>>> GetYcpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

    Task<OperateResult<Ycp>> GetYcpDataByEquipYcNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

    OperateResult GetYcpColumnData();

    Task<OperateResult> AddYcpData(YcpDataModel ycpDataModel);

    Task<OperateResult> EditYcpData(YcpDataModel ycpDataModel);

    Task<OperateResult> DelYcpData(DelYcYxSetDataModel delYcYxSetDataModel);

    OperateResult<IEnumerable<string>> GetCommunicationDrv();

    Task<OperateResult<PagedResult<Yxp>>> GetYxpDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

    Task<OperateResult<Yxp>> GetYxpDataByEquipYxNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

    OperateResult<List<List<string>>> GetYxpColumnData();

    Task<OperateResult> AddYxpData(YxpDataModel yxpDataModel);

    Task<OperateResult> EditYxpData(YxpDataModel yxpDataModel);

    Task<OperateResult> DelYxpData(DelYcYxSetDataModel delYcYxSetDataModel);

    Task<OperateResult<PagedResult<SetParmResponesModel>>> GetSetParmDataList(GetYcYxSetDataListModel getYcYxSetDataListModel);

    Task<OperateResult<SetParm>> GetSetParmDataByEquipSetNo(EquipYcYxSetNoModel equipYcYxSetNoModel);

    OperateResult<List<List<string>>> GetSetParmColumnData();

    Task<OperateResult> AddSetData(SetDataModel setDataModel);

    Task<OperateResult> EditSetData(SetDataModel setDataModel);

    Task<OperateResult> DelSetData(DelYcYxSetDataModel delYcYxSetDataModel);

    Task<OperateResult<AddEquipFromModelRequest[]>> AddEquipFromModelAsync(EquipSetModel equipSetModel);

    Task<OperateResult> SetEquipToModel(long equipNo);

    Task<OperateResult> BatchModifyEquipParam(BatchOperateEquipModel model);

    Task<OperateResult> BatchModifyYcp(BatchOperateEquipModel model);

    Task<OperateResult> BatchModifyYxp(BatchOperateEquipModel model);

    Task<OperateResult> BatchModifyEquipSetting(BatchOperateEquipModel model);

    Task<OperateResult> BatchDeleteEquip(BaseBatchOperateEquipModel model);

    OperateResult BatchImportEquipOrTemplate(List<BatchEquipModel> equips,
        List<BatchYcpModel> ycps, List<BatchYxpModel> yxps,
        List<BatchSetModel> sets, string[] tableNames, int? groupId = null);

    OperateResult<byte[]> BatchExportEquip(List<int> ids, bool exportEquip = true);

    OperateResult<IWorkbook> BatchXmlEquip(List<int> ids, bool exportEquip = true);
}
