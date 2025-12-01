// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class OneEGroup : IEquatable<OneEGroup>
{
    public OneEGroup(int groupId, string groupName, int parentGroupId, IEnumerable<OneEGroupEquip> equips)
    {
        GroupId = groupId;
        GroupName = groupName;
        ParentGroupId = parentGroupId;
        Equips = new EGroupCollection<OneEGroupEquip>();
        Equips.CopyTo(equips);
        Children = new EGroupCollection<OneEGroup>();
    }

    public OneEGroup(int groupId, string groupName, int? parentGroupId, IEnumerable<OneEGroupEquip> equips)
    {
        GroupId = groupId;
        GroupName = groupName;
        ParentGroupId = parentGroupId.GetValueOrDefault();
        Equips = new EGroupCollection<OneEGroupEquip>();
        Equips.CopyTo(equips);
        Children = new EGroupCollection<OneEGroup>();
    }

    public int GroupId { get; private set; }
    public string GroupName { get; private set; }
    public int ParentGroupId { get; private set; }

    public EGroupCollection<OneEGroupEquip> Equips { get; private set; }

    public OneEGroup Parent { get; private set; }

    public EGroupCollection<OneEGroup> Children { get; private set; }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() == typeof(OneEGroup))
        {
            var tmp = (OneEGroup)obj;
            if (tmp.GroupId == this.GroupId)
                return true;
        }
        else if (obj.GetType() == typeof(int))
        {
            return this.GroupId == (int)obj;
        }
        return false;
    }

    public void SetParent(OneEGroup parent)
    {
        Parent = parent;
    }

    public void RemoveParent()
    {
        Parent = null;
    }

    public void ReName(string name)
    {
        GroupName = name;
    }

    public override int GetHashCode()
    {
        return this.GroupId;
    }

    public static bool operator ==(OneEGroup left, OneEGroup right)
    {
        return left.GroupId == right.GroupId;
    }

    public static bool operator !=(OneEGroup left, OneEGroup right)
    {
        return !(left.GroupId == right.GroupId);
    }

    public bool Equals(OneEGroup other)
    {
        return this.GroupId == other.GroupId;
    }
}
