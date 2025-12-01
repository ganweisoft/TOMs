// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;
 
public class GroupEquipModel
{
    public string Title { get; set; }
    
    public int GroupId { get; set; }
    
    public int Count { get; set; }
    
    public int Key { get; set; }
    
    public bool IsGroup { get; set; }
    
    public string ProcAdvice { get; set; }
    
    public int StaNo { get; set; }
    
    public int? Status { get; set; }
    
    public string RelatedView { get; set; }
    
    public List<GroupEquipModel> Children { get; set; }
}