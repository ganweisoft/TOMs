//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.AppServices.Domain.PO
{
    public class GWMenu
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Menuname { get; set; }
        public int? Parentid { get; set; }


        public string? Route { get; set; }


        public string? Path { get; set; }

        public string? Icon { get; set; }

        public int? Nodetype { get; set; }

        public int? Order { get; set; }

        public int? Menuowner { get; set; }

        public bool Enabled { get; set; }


        public string? Packageid { get; set; }






    }
}