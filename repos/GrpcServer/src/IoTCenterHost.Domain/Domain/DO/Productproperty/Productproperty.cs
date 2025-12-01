//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Domain.Domain.DO
{
    public class Productproperty
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string PropertyJson { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUserName { get; set; }
        public string ProductType { get; set; }

        public string ProductName { get; set; }
        public string ManufacturerName { get; set; }
        public string ProtocolType { get; set; }

        public string SoftwareVersion { get; set; }

        public string HardwareVersion { get; set; }

        public string ObjectModelInformation { get; set; }

        public string ManufacturerId { get; set; }


        public string ModelVersion { get; set; }

    }
}
