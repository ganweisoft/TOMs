// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;


/*
 * 需要分页的接口都使用元组，(int Count,IEnumerable<T>Result) ，获得数据后解构
 */


public interface IEGroupService
{
    (int Count, IEnumerable<EGroupStructResponse> Result) GetRoot(GroupListRequest model, string userName);

    EGroupStructResponse GetOneGroup(OneGroupListRequest model, string userName);


    Task<(int Count, IEnumerable<EGroupSearchResponse>)> SearchEquip(string userName, string equipName, int pageNo, int pageSize);

    Task<(int Count, IEnumerable<EGroupSearchResponse>)> SearchSystem(string userName, string systemName, int pageNo, int pageSize);
}
