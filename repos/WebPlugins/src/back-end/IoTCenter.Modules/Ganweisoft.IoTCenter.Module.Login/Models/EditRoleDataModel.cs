// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.Login;

public class EditRoleDataModel
{
    [Required]
    public string RoleName { get; set; }

    public string Remark { get; set; }

    public List<NameAndId> ControlEquipList { get; set; } = new List<NameAndId>();

    public List<NameAndId> ControlSetItemList { get; set; } = new List<NameAndId>();

    public List<NameAndId> BrowseEquipList { get; set; } = new List<NameAndId>();

    public List<NameAndId> BrowseSpecialEquipList { get; set; } = new List<NameAndId>();

    public List<NameAndId> BrowsePagesList { get; set; } = new List<NameAndId>();

    public List<NameAndId> AddinModuleList { get; set; } = new List<NameAndId>();

}


public class NameAndId
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Sid { get; set; }
}

    

public class EditRoleModel
{
    [Required]
    public string RoleName { get; set; }

    public string Remark { get; set; }

    public List<int> ControlEquipList { get; set; } = new List<int>();

    public List<string> ControlSetItemList { get; set; } = new List<string>();

    public List<int> BrowseEquipList { get; set; } = new List<int>();

    public List<string> BrowseSpecialEquipList { get; set; } = new List<string>();

    public List<int> BrowsePagesList { get; set; } = new List<int>();

    public List<int> AddinModuleList { get; set; } = new List<int>();


}
