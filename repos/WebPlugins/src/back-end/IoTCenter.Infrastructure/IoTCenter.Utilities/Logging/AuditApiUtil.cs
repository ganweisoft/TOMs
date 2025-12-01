// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenter.Utilities
{
    internal static class AuditApiUtil
    {
        internal static async Task<string> GetRequestBody(HttpContext httpContext)
        {
            var body = httpContext?.Request?.Body;
            var method = httpContext?.Request?.Method;

            if (HttpMethods.IsGet(method) || HttpMethods.IsTrace(method))
            {
                return null;
            }

            if (body == null)
            {
                return null;
            }

            if (!body.CanSeek || !body.CanRead)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                body.Seek(0, SeekOrigin.Begin);
                await body.CopyToAsync(stream);
                body.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
