// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenter.Data.Model
{
    public  class GwsnapshotConfig
    {
        public int Id { get; set; }
        public string SnapshotName { get; set; }
        public int SnapshotLevelMin { get; set; }
        public int SnapshotLevelMax { get; set; }
        public int MaxCount { get; set; }
        public int IsShow { get; set; }
        public string IconRes { get; set; }
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
    }
}
