// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.TimeTask.Models;
using IoTCenter.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterWebApi.Service
{
    public interface ITimeTaskService
    {
        Task<OperateResult<PagedResult<AllTimeTaskListDto>>> GetAllTaskDataList(CommonSearchPageModel commonSearchPageModel);

        Task<OperateResult<CommonTaskData>> GetCommonTaskData(int? tableId);

        Task<OperateResult<CycleTaskData>> GetCycleTaskData(int? tableId);


        Task<OperateResult> CreateCommonTask(AddCommonTaskModel addCommonTaskModel);

        Task<OperateResult> CreateCycleTask(AddCycleTaskModel addCycleTaskModel);

        Task<OperateResult> EditCommonTaskData(EditCommonTaskModel editCommonTaskModel);

        Task<OperateResult> EditCycleyTaskData(EditCycleTaskModel editCycleTaskModel);

        Task<OperateResult> DelCommonTaskData(int? tableId);

        Task<OperateResult> DelCycleTaskData(int? tableId);

        Task<OperateResult<EditProcTaskWeekModel>> GetProcTaskWeekDataList(string searchName);

        Task<OperateResult> EditProcTaskWeekData(SaveProcTaskWeekModel saveProcTaskWeekModel);

        Task<OperateResult<PagedResult<SpecTimeListDto>>> GetProcTaskSpecDataList(GetProcTaskSpecDataModel getProcTaskSpecDataModel);

        Task<OperateResult> AddProcTaskSpecData(AddProcTaskSpecModel addProcTaskSpecModel);

        Task<OperateResult> EditProcTaskSpecData(EditProcTaskSpecModel editProcTaskSpecModel);

        Task<OperateResult> DelProcTaskSpecData(long Id);

        Task<OperateResult<IEnumerable<SpecMonthDataDto>>> GetProcTaskSpecMonthData(string beginTime, string endTime);

        Task<OperateResult<IEnumerable<GetExProcCmdDataResponse>>> GetExProcCmdData();
    }
}
