// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Diagnostics.CodeAnalysis;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class OneEGroupEquip : IEquatable<OneEGroupEquip>
{
    public int GroupId { get; set; }
    public int EGroupListId { get; set; }
    public int StaNo { get; set; }
    public int EquipNo { get; set; }
    public string EquipName { get; set; }


    public string RelatedView { get; set; }
    public string SystemName { get; set; }
    public object EquipState { get; set; }


    public bool Equals([AllowNull] OneEGroupEquip other)
    {
        if (other.EGroupListId == this.EGroupListId)
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        return this.EGroupListId;
    }


    public static bool operator ==(OneEGroupEquip left, OneEGroupEquip right)
    {
        return left.EGroupListId == right.EGroupListId;
    }

    public static bool operator !=(OneEGroupEquip left, OneEGroupEquip right)
    {
        return !(left.EGroupListId == right.EGroupListId);
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() == typeof(OneEGroupEquip))
        {
            var tmp = (OneEGroupEquip)obj;
            if (tmp.EGroupListId == this.EGroupListId)
                return true;
        }
        else if (obj.GetType() == typeof(int))
        {
            return this.GroupId == (int)obj;
        }

        return false;
    }

    public bool Equals(OneEGroup other)
    {
        return this.GroupId == other.GroupId;
    }
}
