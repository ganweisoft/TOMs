// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWOpcUAStandard.STD.Model;
using Newtonsoft.Json;
using System.Text;

namespace GWOpcUAStandard.STD.Service
{
    public class LinkStateFileService
    {
        private const string BaseDirName = "GWOpcUAStandard.STD";
        private const string BaseFileName = "GWOpcUAStandard.STD.json";
        public async Task UpdateLinkStateFile(List<LinkStateFileModel> list)
        {
            var directoryInfo = Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent.FullName, "dll");
            var newDir = Path.Combine(directoryInfo, BaseDirName);
            if (Directory.Exists(newDir))
            {
                directoryInfo = newDir;
            }

            var filePath = Path.Combine(directoryInfo, BaseFileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(list));
                await fs.WriteAsync(bytes, 0, bytes.Length);
                fs.Close();
            }
        }
    }
}
