// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganweisoft.IoTCenter.Module.EquipLink.Dto
{
    [Table("northterminals")]
    public class NorthTerminalDto
    {
        [Key]
        public int Id { get; set; }
        public string TerminalNo { get; set; }
        public string Name { get; set; }
        public int EquipNo { get; set; }
        public int ProductId { get; set; }
        public string TerminalTypeIds { get; set; }
        public string TerminalTypeNames { get; set; }

        public string AreaIds { get; set; }
        public string CompanyIds { get; set; }

        public string AreaName { get; set; }
        public string BuildName { get; set; }
        public string Address { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public string CompanyName { get; set; }
        public string GatewayId { get; set; }
        public string ExtendInfo { get; set; }
        public string Remark { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUserName { get; set; }

        public string VideoParam { get; set; }

        public string SceneParam { get; set; }
        public int State { get; set; }
        public string TerminalIdentityId { get; set; }
        public string TerminalParentId { get; set; }
        public string CategoryIds { get; set; }
        public string CategoryNames { get; set; }
        public string CategoryIdsList { get; set; }
        public decimal? Altitude { get; set; }
        public string DeviceApplicationNo { get; set; }
    }

}
