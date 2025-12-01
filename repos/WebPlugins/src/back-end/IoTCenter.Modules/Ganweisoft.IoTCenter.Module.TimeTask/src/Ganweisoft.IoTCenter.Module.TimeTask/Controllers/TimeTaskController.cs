// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Ganweisoft.IoTCenter.Module.TimeTask.Models;
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.TimeTask.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiExplorerSettings(GroupName = SwaggerApiGroup.TimeTask)]
    public class TimeTaskController : DefaultController
    {
        private readonly ITimeTaskService _timeTaskService;
        private readonly TimeTaskDbContext _context;
        private readonly IStringLocalizer<TimeTaskController> _stringLocalizer;

        public TimeTaskController(ITimeTaskService timeTaskService
            , TimeTaskDbContext context
            , IStringLocalizer<TimeTaskController> stringLocalizer)
        {
            _timeTaskService = timeTaskService;
            _context = context;
            _stringLocalizer = stringLocalizer;
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<AllTimeTaskListDto>>> GetAllTaskDataList(CommonSearchPageModel commonSearchPageModel)
        {
            return await _timeTaskService.GetAllTaskDataList(commonSearchPageModel);
        }

        [HttpGet]
        public async Task<OperateResult> GetCommonTaskData(int? tableId)
        {
            if (tableId == null)
            {

                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (!_context.GwProcTimeTlist.Any(d => d.TableId == tableId))
            {
                return OperateResult.Failed(_stringLocalizer["未查询到任务信息"]);
            }

            return await _timeTaskService.GetCommonTaskData(tableId);
        }

        [HttpGet]
        public async Task<OperateResult> GetCycleTaskData(int? tableId)
        {
            if (tableId == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (!_context.GwProcCycleTlist.Any(d => d.TableId == tableId))
            {
                return OperateResult.Failed(_stringLocalizer["未查询到任务信息"]);
            }

            return await _timeTaskService.GetCycleTaskData(tableId);
        }

        [HttpPost]
        public async Task<OperateResult> CreateCommonTask(AddCommonTaskModel addCommonTaskModel)
        {
            if (addCommonTaskModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }

            if (addCommonTaskModel.TableName.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }
            if (await _context.GwProcTimeTlist.AnyAsync(d => addCommonTaskModel.TableName == d.TableName))
            {
                return OperateResult.Failed(_stringLocalizer["任务名重复"]);
            }

            return await _timeTaskService.CreateCommonTask(addCommonTaskModel);
        }

        [HttpPost]
        public async Task<OperateResult> CreateCycleTask(AddCycleTaskModel addCycleTaskModel)
        {
            if (addCycleTaskModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }
            if (addCycleTaskModel.TableName.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }
            if (await _context.GwProcCycleTlist.AnyAsync(d => addCycleTaskModel.TableName == d.TableName))
            {
                return OperateResult.Failed(_stringLocalizer["任务名重复"]);
            }

            return await _timeTaskService.CreateCycleTask(addCycleTaskModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditCommonTaskData(EditCommonTaskModel editCommonTaskModel)
        {
            if (editCommonTaskModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }
            if (editCommonTaskModel.TableName.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }
            if (await _context.GwProcTimeTlist.AnyAsync(d => editCommonTaskModel.TableId != d.TableId && editCommonTaskModel.TableName == d.TableName))
            {
                return OperateResult.Failed(_stringLocalizer["任务名重复"]);
            }
            return await _timeTaskService.EditCommonTaskData(editCommonTaskModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditCycleyTaskData(EditCycleTaskModel editCycleTaskModel)
        {
            if (editCycleTaskModel == null)
            {
                return OperateResult.Failed(_stringLocalizer["请求参数为空，请检查"]);
            }
            if (editCycleTaskModel.TableName.IsEmpty())
            {
                return OperateResult.Failed(_stringLocalizer["请求参数不完整，请检查"]);
            }
            if (await _context.GwProcCycleTlist.AnyAsync(d => editCycleTaskModel.TableId != d.TableId && editCycleTaskModel.TableName == d.TableName))
            {
                return OperateResult.Failed(_stringLocalizer["任务名重复"]);
            }
            return await _timeTaskService.EditCycleyTaskData(editCycleTaskModel);
        }

        [HttpDelete]
        public async Task<OperateResult> DelCommonData(int? tableId)
        {
            return await _timeTaskService.DelCommonTaskData(tableId);
        }

        [HttpDelete]
        public async Task<OperateResult> DelCycleData(int? tableId)
        {
            return await _timeTaskService.DelCycleTaskData(tableId);

        }

        [HttpPost]
        public async Task<OperateResult<EditProcTaskWeekModel>> GetProcTaskWeekDataList(string searchName)
        {
            return await _timeTaskService.GetProcTaskWeekDataList(searchName);
        }

        [HttpPost]
        public async Task<OperateResult> EditProcTaskWeekData(SaveProcTaskWeekModel saveProcTaskWeekModel)
        {
            return await _timeTaskService.EditProcTaskWeekData(saveProcTaskWeekModel);
        }

        [HttpPost]
        public async Task<OperateResult<PagedResult<SpecTimeListDto>>> GetProcTaskSpecDataList(GetProcTaskSpecDataModel getProcTaskSpecDataModel)
        {
            return await _timeTaskService.GetProcTaskSpecDataList(getProcTaskSpecDataModel);
        }

        [HttpPost]
        public async Task<OperateResult> AddProcTaskSpecData(AddProcTaskSpecModel addProcTaskSpecModel)
        {
            return await _timeTaskService.AddProcTaskSpecData(addProcTaskSpecModel);
        }

        [HttpPost]
        public async Task<OperateResult> EditProcTaskSpecData(EditProcTaskSpecModel editProcTaskSpecModel)
        {
            return await _timeTaskService.EditProcTaskSpecData(editProcTaskSpecModel);
        }

        [HttpPost]
        public async Task<OperateResult> DelProcTaskSpecData(long id)
        {
            return await _timeTaskService.DelProcTaskSpecData(id);
        }

        [HttpGet]
        public async Task<OperateResult<IEnumerable<SpecMonthDataDto>>> GetProcTaskSpecMonthData(string beginTime, string endTime)
        {
            return await _timeTaskService.GetProcTaskSpecMonthData(beginTime, endTime);
        }

        [HttpGet]
        public async Task<OperateResult<IEnumerable<GetExProcCmdDataResponse>>> GetExProcCmdData()
        {
            return await _timeTaskService.GetExProcCmdData();
        }
    }
}
