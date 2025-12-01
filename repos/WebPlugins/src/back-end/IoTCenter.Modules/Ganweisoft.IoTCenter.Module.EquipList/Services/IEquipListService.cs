// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Proxy;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public interface IEquipListService
{
    Task<OperateResult> GetRealEquipListByPage(GetEquipListModel getEquipListModel);

    OperateResult<Dictionary<int, GrpcEquipState>> GetEquipListStateByPage(GetEquipListStateModel getEquipListStateModel);

    OperateResult<Page> GetEquipItemStateByPage(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult GetEquipItemStateByIds(GetEquipItemStateByIds getEquipItemStateByIds);

    OperateResult GetYcpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult<Page> GetYcpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult GetYxpItemValueAndState(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult<Page> GetYxpItemStateByPage(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult<Page> GetSetItemStateByPage(GetEquipItemStateModel getEquipItemStateModel);

    OperateResult<object> GetEquipYcpState(GetEquipYcYxModel getEquipYcYxModel);

    OperateResult<object> GetEquipYxpState(GetEquipYcYxModel getEquipYcYxModel);

    Task<OperateResult> SetCommandBySetNo(CommandBySetNoModel commandBySetNoModel);

    OperateResult SetCommandByParameter(CommandByParameterModel commandByParameterModel);


    OperateResult SetCommandWithRespone(CommandWithResponeModel commandByParameterModel);


    OperateResult SyncCommand(CommandWithResponeModel commandByParameterModel);

    OperateResult<Page> GetYcpHistroyByTimeAsync(GetYcpHistroyModel getYcpHistroyModel);

    OperateResult<List<MyCurveCommonData>> GetYcpHistroyChartByTimeAsync(GetYcpHistroyModel getYcpHistroyModel);

    OperateResult<PagedResult<GetEquipNoAndNameResponse>> GetEquipNoAndName(GetEquipListStateModel getEquipListStateModel);

    OperateResult<PagedResult<string>> GetSetEquip(GetEquipListStateModel getEquipListStateModel);

    OperateResult<PagedResult<string>> GetEquipAndSet(GetEquipListStateModel getEquipListStateModel);

    Task<OperateResult<PageResult<YcpList>>> GetYcpByEquipNo(GetEquipItemStateModel getEquipItemStateModel);

    Task<OperateResult<PageResult<YxpList>>> GetYxpByEquipNo(GetEquipItemStateModel getEquipItemStateModel);

    Task<OperateResult<PageResult<EquipSetParmQuery>>> GetSetParmByEquipNo(GetEquipItemStateModel getEquipItemStateModel);

    Task<OperateResult<PageResult<GetSetParmNewByEquipNoModel>>> NewGetSetParmByEquipNo(GetEquipItemStateModel getEquipItemStateModel);


    Task<OperateResult<IEnumerable<SetParmByEquipNosResponse>>> GetSetParmByEquipNos(List<int> equipList);

    OperateResult<PagedResult<string>> GetEquipSetParmList(CommonSearchPageModel commonSearchPageModel);

    OperateResult<PagedResult<EquipSetParmResonse>> GetEquipSetParmTreeList(CommonSearchPageModel commonSearchPageModel);

    Task<OperateResult<PagedResult<RealEquipSetParmListModel>>> GetRealEquipSetParmList(CommonSearchPageModel commonSearchPageModel);


    Task<OperateResult<byte[]>> ExportAbnormalRecord(int deviceStatus);

    Task<OperateResult<byte[]>> ExportEquipHistroyCurves(ExportHistoryCuresModelInternal model);

    Task<OperateResult<byte[]>> ExportAllEquipList();

    Task<OperateResult<byte[]>> DownloadEquipTemplateFile();

    Task<OperateResult> ImportEquipList(IFormFile excelFile);

    Task<OperateResult<GetExportEquipYcYxpsRespones>> GetExportEquipYcYxps(GetExportEquipYcYxpsRequest request);

    Task<OperateResult> GetEquipListByPageNoState(GetEquipListModel equipListModel);
}
