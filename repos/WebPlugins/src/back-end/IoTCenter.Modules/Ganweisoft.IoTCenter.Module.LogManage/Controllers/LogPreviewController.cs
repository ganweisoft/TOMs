// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterCore.Modules;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.LogManage.Controllers
{
    [Route("/IoT/api/v3/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class LogPreviewController : DefaultController
    {
        private readonly string GateWayLogPath;
        private readonly string WebLogPath;

        private readonly Session _session;
        private readonly ILoggingService _apiLog;
        public LogPreviewController(
            Session session,
            ILoggingService apiLog,
            IApplicationContext applicationContext)
        {
            _apiLog = apiLog;
            _session = session;

            var contextPath = new DirectoryInfo(AppContext.BaseDirectory);
            GateWayLogPath = Path.Combine(contextPath.Parent.Parent.FullName, "log");
            WebLogPath = Path.Combine(contextPath.FullName, "Logs");
        }

        [HttpGet]
        [SkipCustomFilter]
        public async Task<ActionResult> DownLoadLog(int logType, string relativePath)
        {
            if (relativePath.IsEmpty() || logType < 0)
            {
                return NoContent();
            }

            var logPath = string.Empty;
            if (logType == 0)
            {
                logPath = GateWayLogPath;
            }
            else if (logType == 1)
            {
                logPath = WebLogPath;
            }
            if (logPath.IsEmpty())
            {
                return NoContent();
            }

            if (!Directory.Exists(logPath))
            {
                return NoContent();
            }

            if (relativePath.StartsWith(Path.DirectorySeparatorChar))
            {
                relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar);
            }

            var fileName = Path.Combine(logPath, relativePath);

            try
            {
                var fileFullPath = Path.GetFullPath(fileName);
                if (!fileFullPath.StartsWith(logPath, StringComparison.OrdinalIgnoreCase))
                {
                    return NoContent();
                }
            }
            catch
            {
                return NoContent();
            }

            if (!System.IO.File.Exists(fileName))
            {
                return NoContent();
            }

            await using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            await using var ms = new MemoryStream();
            
            await fileStream.CopyToAsync(ms);
            
            var fileBytes = ms.ToArray();

            await _apiLog.Audit(new AuditAction(_session.UserName)
            {
                ResourceName = "下载服务日志接口",
                EventType = "下载服务日志事件",
                Result = new AuditResult { Default = "下载成功" }
            });
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fileName));
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public PagedResult<LogTreeResponse> GetLogFileTree([FromQuery] LogFileTreeQueryRequest request)
        {
            var result = new List<LogTreeResponse>();
            var path = string.Empty;
            if (request.LogType == 0)
            {
                path = GateWayLogPath;
            }
            else if (request.LogType == 1)
            {
                path = WebLogPath;
            }
            if (!Directory.Exists(path))
            {
                return PagedResult<LogTreeResponse>.Create();
            }
            
            var dirInfo = new DirectoryInfo(path);
            
            object[] allDirectories = dirInfo.GetDirectories()?.OrderByDescending(x => x.CreationTime).ToArray();
            
            object[] allFiles = dirInfo.GetFiles().OrderByDescending(x => x.LastWriteTime).ToArray();
            
            if (allDirectories.IsEmpty() && allFiles.IsEmpty())
            {
                return PagedResult<LogTreeResponse>.Create();
            }
            
            IEnumerable<object> allObj = allDirectories.Union(allFiles);

            var objs = allObj.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize);
            foreach (var item in objs)
            {
                if (item is DirectoryInfo dir)
                {
                    result.Add(AddDireRsp(dir, path));
                }
                else if (item is FileInfo file)
                {
                    result.Add(AddFileRsp(file, path));
                }
            }
            return PagedResult<LogTreeResponse>.Create(allObj.Count(), result);
        }

        private List<LogTreeResponse> GetTree(string path, out long length, string rootPath)
        {
            length = 0;
            var result = new List<LogTreeResponse>();
            if (!Directory.Exists(path))
            {
                _apiLog.Error($"Ganweisoft.IoTCenter.Module.LogManage GetTree not found：{path}");
                return result;
            }
            var dirInfo = new DirectoryInfo(path);
            foreach (var item in dirInfo.GetFiles())
            {
                var file = AddFileRsp(item, rootPath);
                length += file.Size;
                result.Add(file);
            }
            foreach (var item in dirInfo.GetDirectories())
            {
                var dir = AddDireRsp(item, rootPath);
                length += dir.Size;
                result.Add(dir);
            }
            return result;
        }
        private LogTreeResponse AddFileRsp(FileInfo item, string rootPath)
        {
            return new LogTreeResponse
            {
                IsDirectory = false,
                Name = item.Name,
                FullPath = item.FullName.Replace(rootPath, ""),
                ModifyTime = item.LastWriteTime,
                Size = item.Length
            };
        }
        private LogTreeResponse AddDireRsp(DirectoryInfo item, string rootPath)
        {
            var dir = new LogTreeResponse
            {
                Name = item.Name,
                FullPath = item.FullName.Replace(rootPath, ""),
                IsDirectory = true,
                ModifyTime = item.LastWriteTime,
                Childs = GetTree(item.FullName, out var dirLength, rootPath)
            };
            dir.Size = dirLength;
            return dir;
        }
    }
}
