// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenterHost.Core.Abstraction.BaseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public interface IBAService
{
    Task<EquipListModelEx> GetOneGroupAsync(OneGroupRequest model);

    Task<bool> MoveEquips(int oldGroupId, int newGroupId);

    Task<bool> MoveEquipsToParentGroup(int oldGroupId);

    Task<EquipYcYxList<EquipListModelEx>> GetGroupListAsync(int? pageNo, int? pageSize, string systemName, string searchWorld);

    Task<EquipYcYxList<EquipListModelEx>> GetEquipTreeList(int? pageNo, int? pageSize, string searchWorld, string systemName);



    Task<(bool IsSuccess, int GroupId, string Message)> AddGroupAsync(string name, int parentId, IEnumerable<int> equipNos);

    Task<bool> DeleteGroupAsync(int groupId,bool isDeleteEquip = false);

    Task<bool> ReNameGroupAsync(int groupId, string newName);

    Task<(bool IsSuccess, string Message)> GroupInsertEquipAsync(int groupId, IEnumerable<int> equipNos);

    Task<bool> GroupDeleteEquipAsync(int groupId, int equipNo);




    Task<EquipYcYxList<EquipYc>> EquipYcListAsync(int staN, int equipNo, int pageNo, int pageSize, string word = null);

    Task<EquipYcYxList<EquipYx>> EquipYxListAsync(int staN, int equipNo, int pageNo, int pageSize, string word = null);

    Task<EquipYcYxList<SetParm>> EquipSetparmListAsync(int staN, int equipNo, int pageNo, int pageSize, string word = null);



    List<EquipValueResponse<GrpcEquipState>> GetEquipStatus(IEnumerable<int> nos);
    EquipValueResponse<IEnumerable<EquipValueResponse<object>>> GetYcStatus(int equipNo, IEnumerable<int> ycps);
    EquipValueResponse<IEnumerable<EquipValueResponse<object>>> GetYxStatus(int equipNo, IEnumerable<int> yxps);


    Task<IEnumerable<CurDataResponse>> GetCurAsyncAsync(CurDataRequest model);


    Task<(int Count, IEnumerable<EquipDetailResponse> Result)> GetEquipDetail(GroupQueryModel model);
    

    Task<IEnumerable<GroupEquipModel>> GetGroupEquipAsync(int? pageNo, int? pageSize, string searchWorld, int groupId);

    Task<IEnumerable<GroupEquipModel>> GetGroupEquipAsync(string searchWorld);

    Task<int> GroupListCount();
}
