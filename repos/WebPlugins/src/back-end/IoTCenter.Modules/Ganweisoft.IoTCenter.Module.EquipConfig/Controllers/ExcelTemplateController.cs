// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.ExcelHelper;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Ganweisoft.IoTCenter.Module.EquipConfig.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
 
    public class ExcelTemplateController : DefaultController
    {
        private readonly IWebHostEnvironment _environment;
        public ExcelTemplateController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Download([FromQuery] string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return Content("<html><head><meta http-equiv='Content-Type' content='text/html' charset='utf-8'/><script type='text/javascript'>alert('请提供下载模板名称!');</script></head><body></body></html>", "text/html");
            }

            var templateDir = Path.Combine(_environment.WebRootPath, "templates");
            if (!Directory.Exists(templateDir))
            {
                return Content("<html><head><meta http-equiv='Content-Type' content='text/html' charset='utf-8'/><script type='text/javascript'>alert('模板不存在，请联系平台管理员处理!');</script></head><body></body></html>", "text/html");
            }

            var fileName = $"{templateName}.xlsx";

            var templatePath = Path.Combine(templateDir, fileName);
            if (!System.IO.File.Exists(templatePath))
            {
                return Content("<html><head><meta http-equiv='Content-Type' content='text/html' charset='utf-8'/><script type='text/javascript'>alert('模板不存在，请联系平台管理员处理!');</script></head><body></body></html>", "text/html");
            }
            var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, MimeTypes.ApplicationOctetStream, fileName);
        }
    }
}
