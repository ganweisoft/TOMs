// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IoTCenter.Data.Model
{
    public class EGroup
    {
        public int GroupId { get; set; }


        [Required(ErrorMessage = "分组名称不能为空")]
        public string GroupName { get; set; }

        public List<EGroupList> Lists { get; set; }

        [Required(ErrorMessage = "分组父节点不能为空")]
        public int? ParentGroupId { get; set; }
    }
}
