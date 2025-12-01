// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class BAController : DefaultController
    {
        private readonly Session _session;
        private readonly IBAService _BAService;
        private readonly GWDbContext _context;
        private readonly ILoggingService _apiLog;

        public BAController(IBAService bAServicelmpl,
            GWDbContext context,
            ILoggingService apiLog,
            Session session)
        {
            _session = session;
            _BAService = bAServicelmpl;
            _context = context;
            _apiLog = apiLog;
        }

        #region 分组管理

        [HttpPost]
        public async Task<OperateResult<EquipGroupResponse>> GroupList([FromBody] GroupQueryModel model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            var result =
                await _BAService.GetGroupListAsync(model.PageNo, model.PageSize, model.SearchName, model.SystemName);

            return OperateResult.Successed(new EquipGroupResponse
            {
                Count = result.Count,
                TotalEquipCount = result.TotalEquipCount,
                PageNo = model.PageNo,
                PageSize = model.PageSize,
                EquipList = result.ListNos.ToArray(),
                Groups = result.List
            });
        }

        [HttpPost]
        public async Task<OperateResult<EquipGroupResponse>> XCGroupList([FromBody] GroupQueryModel model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            var result = await _BAService.GetGroupListAsync(model.PageNo, model.PageSize, model.SearchName, model.SystemName);

            return OperateResult.Successed(new EquipGroupResponse
            {
                Count = result.Count,
                TotalEquipCount = result.TotalEquipCount,
                PageNo = model.PageNo,
                PageSize = model.PageSize,
                EquipList = result.ListNos.ToArray(),
                Groups = result.List
            });
        }

        void RecursiveGroupList(List<EquipListModelEx> list)
        {
            List<EquipListModelEx> listTemp = new List<EquipListModelEx>();
            foreach (var item in list)
            {
                if (item.Children.Count > 0)
                {
                    RecursiveGroupList(item.Children);
                }

                if (item.EquipLists.Count == 0 && item.Children.Count == 0)
                {
                    listTemp.Add(item);
                }
            }
            foreach (var item in listTemp)
            {
                list.Remove(item);
            }
        }


        [HttpPost]
        public async Task<OperateResult<EquipGroupResponse>> GroupListNoState([FromBody] GroupQueryModel model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            var result =
                await _BAService.GetEquipTreeList(model.PageNo, model.PageSize, model.SearchName, model.SystemName);

            return OperateResult.Successed(new EquipGroupResponse
            {
                Count = result.Count,
                PageNo = model.PageNo,
                TotalEquipCount = result.TotalEquipCount,
                PageSize = model.PageSize,
                EquipList = result.ListNos.ToArray(),
                Groups = result.List
            });
        }

        [HttpPost]
        public async Task<OperateResult<EquipListModelEx>> GetOneGroup([FromBody] OneGroupRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipListModelEx>("请求参数为空");
            }

            return OperateResult.Successed(await _BAService.GetOneGroupAsync(model));
        }


        [HttpPost]
        public async Task<OperateResult> AddGroup([FromBody] AddGroupRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            var result = await _BAService.AddGroupAsync(model.Name, model.ParentId, model.EquipNos);

            if (!result.IsSuccess)
            {
                return OperateResult.Failed<EquipGroupResponse>(result.Message);
            }

            return OperateResult.Successed(new
            {
                result.GroupId,
                result.Message
            });
        }

        [HttpPost]
        public async Task<OperateResult> DeleteGroup([FromBody] DeleGroupRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            if (model.GroupId == 0)
            {
                return OperateResult.Failed("请求数据为空或此分组不能被删除");
            }

            if (!model.DeleteEquip)
            {
                var moveResult = await _BAService.MoveEquipsToParentGroup(model.GroupId);
                if (!moveResult)
                {
                    return OperateResult.Failed("移动分组的设备到父分组中失败");
                }
            }

            var deleteResult = await _BAService.DeleteGroupAsync(model.GroupId, model.DeleteEquip);

            if (!deleteResult)
            {
                return OperateResult.Failed("删除失败");
            }

            return OperateResult.Success;
        }

        [HttpPost]
        public async Task<OperateResult> BatchMoveEquip(BatchMoveEquipDto request)
        {
            if (request == null || !request.EquipNoList.Any())
            {
                return OperateResult.Failed();
            }

            if (!(await _context.EGroup.AnyAsync(x => x.GroupId == request.newGroupId)))
            {
                return OperateResult.Failed("分组不存在");
            }

            if (await _context.Equip.CountAsync(x => request.EquipNoList.Contains(x.EquipNo)) !=
                request.EquipNoList.Length)
            {
                return OperateResult.Failed("设备不存在");
            }

            var equipGroup =
                await _context.EGroupList.Where(x => request.EquipNoList.Contains(x.EquipNo)).ToListAsync();

            if (!equipGroup.Any())
            {
                return OperateResult.Failed("未查询到相关设备分组信息");
            }

            equipGroup.ForEach(d => d.GroupId = request.newGroupId);
            if (await _context.SaveChangesAsync() < 0)
            {
                return OperateResult.Failed("移除分组失败");
            }

            _apiLog.Info(
                $"用户\"[{_session.UserName}({_session.IpAddress})]\"-批量移动设备:[{string.Join(",", request.EquipNoList)}]-至分组[id:{request.newGroupId}]-成功");
            return OperateResult.Success;
        }

        [HttpGet]
        public async Task<OperateResult> ReNameGroup(int groupId, string newName)
        {
            if (groupId == 0)
            {
                return OperateResult.Failed("请求数据为空或此分组不能被修改");
            }

            var result = await _BAService.ReNameGroupAsync(groupId, newName);

            if (!result)
            {
                return OperateResult.Failed("重命名失败，可能已经存在相同名称的分组");
            }

            return OperateResult.Success;
        }

        #endregion


        #region 设备管理

        [HttpPost]
        public async Task<OperateResult> GroupInsertEquip([FromBody] GroupInsertEquipRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipGroupResponse>("请求参数为空");
            }

            var result = await _BAService.GroupInsertEquipAsync(model.GroupId, model.EquipNos);

            if (!result.IsSuccess)
            {
                return OperateResult.Failed(result.Message);
            }

            _apiLog.Info(
                $"用户\"[{_session.UserName}({_session.IpAddress})]\"-分组[id：{model.GroupId}]中移入设备:[{string.Join(",", model.EquipNos)}]-成功");

            return OperateResult.Success;
        }

        [HttpGet]
        public async Task<OperateResult> GroupDeleteEquip(int groupId, int equipNo)
        {
            var result = await _BAService.GroupDeleteEquipAsync(groupId, equipNo);

            if (!result)
            {
                return OperateResult.Failed("删除失败");
            }

            return OperateResult.Success;
        }

        #endregion


        #region 获取设备数据

        [HttpPost]
        public async Task<OperateResult<PagedResult<EquipDetailResponse>>> EquipList([FromBody] GroupQueryModel model)
        {
            if (model == null)
            {
                return OperateResult.Failed<PagedResult<EquipDetailResponse>>("请求参数为空");
            }

            var result = await _BAService.GetEquipDetail(model);

            return OperateResult.Successed(PagedResult<EquipDetailResponse>.Create(result.Count, result.Result));
        }

        [HttpPost]
        public async Task<OperateResult<EquipYcYxList<EquipYc>>> EquipYcList([FromBody] EquipycyxListRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipYcYxList<EquipYc>>("请求参数为空");
            }

            model.PageNo ??= 1;

            var result = await _BAService.EquipYcListAsync(
                model.StaN,
                model.EquipNo,
                model.PageNo.Value,
                model.PageSize.Value,
                model.SearchName
            );

            return OperateResult.Successed<EquipYcYxList<EquipYc>>(result);
        }

        [HttpPost]
        public async Task<OperateResult<EquipYcYxList<EquipYx>>> EquipYxList([FromBody] EquipycyxListRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipYcYxList<EquipYx>>("请求参数为空");
            }

            model.PageNo ??= 1;

            var result = await _BAService.EquipYxListAsync(
                model.StaN,
                model.EquipNo,
                model.PageNo.Value,
                model.PageSize.Value,
                model.SearchName
            );

            return OperateResult.Successed<EquipYcYxList<EquipYx>>(result);
        }


        [HttpPost]
        public async Task<OperateResult> EquipSetparmList([FromBody] EquipycyxListRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipYcYxList<Yxp>>("请求参数为空");
            }

            model.PageNo ??= 1;
            model.PageSize = model.PageSize < 20 || model.PageSize > 100 ? 20 : model.PageSize;

            var result = await _BAService.EquipSetparmListAsync(
                model.StaN,
                model.EquipNo,
                model.PageNo.Value,
                model.PageSize.Value,
                model.SearchName
            );

            return OperateResult.Successed<EquipYcYxList<SetParm>>(result ?? new EquipYcYxList<SetParm>());
        }

        #endregion

        #region 获取实时数据
        [HttpPost]
        public OperateResult<IEnumerable<EquipValueResponse<GrpcEquipState>>> EquipStatus([FromBody] int[] nos)
        {
            if (nos == null || nos.Length == 0)
            {
                return OperateResult.Failed<IEnumerable<EquipValueResponse<GrpcEquipState>>>("请求数据不能为空");
            }

            var result = _BAService.GetEquipStatus(nos);

            return OperateResult.Successed<IEnumerable<EquipValueResponse<GrpcEquipState>>>(result);
        }

        [HttpPost]
        [DoNotRefreshSlidingExpiration]
        public OperateResult<EquipValueResponse<IEnumerable<EquipValueResponse<object>>>> EquipYcState(
            [FromBody] EquipYcYxRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipValueResponse<IEnumerable<EquipValueResponse<object>>>>("请求参数为空");
            }

            var result = _BAService.GetYcStatus(model.EquipNo, model.YcYxNos);

            return OperateResult.Successed(result);
        }

        [HttpPost]
        [DoNotRefreshSlidingExpiration]
        public OperateResult<EquipValueResponse<IEnumerable<EquipValueResponse<object>>>> EquipYxState(
            [FromBody] EquipYcYxRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EquipValueResponse<IEnumerable<EquipValueResponse<object>>>>("请求参数为空");
            }

            var result = _BAService.GetYxStatus(model.EquipNo, model.YcYxNos);

            return OperateResult.Successed(result);
        }

        #endregion

        #region 分组

        [HttpPost]
        public async Task<OperateResult<IEnumerable<CurDataResponse>>> EquipCurData([FromBody] CurDataRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<IEnumerable<CurDataResponse>>("请求参数为空");
            }

            var result = await _BAService.GetCurAsyncAsync(model);

            return OperateResult.Successed(result);
        }

        #endregion

        #region 分组重构

        [HttpGet]
        public async Task<OperateResult<IEnumerable<GroupEquipModel>>> GroupListNew([FromQuery] GroupQueryRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<IEnumerable<GroupEquipModel>>("请求参数为空");
            }

            var result =
                await _BAService.GetGroupEquipAsync(model.PageNo, model.PageSize, model.SearchName, model.GroupId);

            return OperateResult.Successed(result);
        }
        
        
        [HttpGet]
        public async Task<OperateResult<int>> GroupListCount()
        {
            var result = await _BAService.GroupListCount();

            return OperateResult.Successed(result);
        }

        [HttpGet]
        public async Task<OperateResult<IEnumerable<GroupEquipModel>>> GetGroupEquipAsync([FromQuery] string searchWorld)
        {
            var result = await _BAService.GetGroupEquipAsync(searchWorld);
            
            return OperateResult.Successed(result);
        }
        
        #endregion
    }
}
