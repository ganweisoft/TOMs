// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTCenter.Data.Model
{
    public partial class GwziChanRecord
    {
        [Key]
        public int Id { get; set; }
        public string ZiChanId { get; set; }
        public DateTime WeiHuDate { get; set; }
        public string WeiHuName { get; set; }
        public string WeiHuRecord { get; set; }
        public string ItemAddMan { get; set; }
        public DateTime ItemAddDate { get; set; }
        public string Pictures { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }

        [NotMapped]
        public string ZiChanName { get; set; }
    }
}
