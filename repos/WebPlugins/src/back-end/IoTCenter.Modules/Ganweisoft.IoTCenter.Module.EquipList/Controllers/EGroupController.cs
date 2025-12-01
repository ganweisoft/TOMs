// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    public class EGroupController : DefaultController
    {
        private readonly IEGroupService _eGroupService;
        public EGroupController(IEGroupService eGroupService)
        {
            _eGroupService = eGroupService;
        }

        [HttpPost]
        public OperateResult<PagedResult<EGroupStructResponse>> Root([FromBody] GroupListRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<PagedResult<EGroupStructResponse>>("请求参数为空");
            }
           
            var roleName = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            var result = _eGroupService.GetRoot(model, roleName);
            return OperateResult.Successed(PagedResult<EGroupStructResponse>.Create(result.Count, result.Result));
        }

        [HttpGet]
        public OperateResult<EGroupStructResponse> OneGroup([FromBody] OneGroupListRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<EGroupStructResponse>("请求参数为空");
            }

            var roleName = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            return OperateResult.Successed(_eGroupService.GetOneGroup(model, roleName));
        }

        [HttpGet]
        public async Task<OperateResult<PagedResult<EGroupSearchResponse>>> Search([FromBody] EGroupSearchRequest model)
        {
            if (model == null)
            {
                return OperateResult.Failed<PagedResult<EGroupSearchResponse>>("请求参数为空");
            }

            var roleName = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;

            (int Count, IEnumerable<EGroupSearchResponse> Result) result;

            if (!string.IsNullOrEmpty(model.EquipName))
            {
                result = await _eGroupService.SearchEquip(roleName, model.EquipName, model.PageNo.Value, model.PageSize);
            }
            else if (!string.IsNullOrEmpty(model.SystemName))
            {
                result = await _eGroupService.SearchSystem(roleName, model.SystemName, model.PageNo.Value, model.PageSize);
            }
            else
            {
                result = (0, Array.Empty<EGroupSearchResponse>());
            }

            return OperateResult.Successed(PagedResult<EGroupSearchResponse>.Create(result.Count, result.Result));
        }
    }
}
