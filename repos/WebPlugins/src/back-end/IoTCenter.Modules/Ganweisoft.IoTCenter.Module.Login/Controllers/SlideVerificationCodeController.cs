// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterCore.SlideVerificationCode;
using IoTCenterHost.Proxy;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;

namespace Ganweisoft.IoTCenter.Module.Login.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]/[action]")]
    [AllowAnonymous]
    public class SlideVerificationCodeController : DefaultController
    {
        private readonly IMemoryCacheService memoryCacheService;
        private readonly ILoggingService _apiLog;
        private readonly CaptchaHelper captcha;
        private readonly IWebHostEnvironment environment;
        public SlideVerificationCodeController(CaptchaHelper captcha,
            IWebHostEnvironment environment,
            IMemoryCacheService memoryCacheService,
            ILoggingService apiLog)
        {
            this.captcha = captcha;
            this.environment = environment;
            this.memoryCacheService = memoryCacheService;
            _apiLog = apiLog;
        }

        [HttpGet]
        public string Get()
        {
            var pic = System.IO.Path.Combine(environment.WebRootPath, "gallery");

            var code = captcha.GetVerificationCode(pic);

            memoryCacheService.Set("code", captcha._PositionX.ToString(), DateTimeOffset.Now.AddMinutes(1));

            memoryCacheService.Set("code_errornum", string.Empty);

            return code;
        }

        [HttpPost]
        public IActionResult Check()
        {
            var formPoint = Request.Form["point"];
            var formdateList = Request.Form["datelist"];

            if (StringValues.IsNullOrEmpty(formPoint) ||
                StringValues.IsNullOrEmpty(formdateList))
            {
                return Ok(new { state = -1, msg = "坐标值为空" });
            }

            var last_point = formPoint.ToString();
            var date_list = formdateList.ToString();

            if (!int.TryParse(last_point, out var point))
            {
                return Ok(new { state = -1, msg = "坐标值为空" });
            }

            if (point <= 0)
            {
                return Ok(new { state = -1, msg = "坐标值为空" });
            }

            if (string.IsNullOrEmpty(date_list))
            {
                return Ok(new { state = -1, msg = "参数date_list为空" });
            }

            var code = memoryCacheService.Get("code");

            _apiLog.Debug($"slide verlification code：{code}");

            if (string.IsNullOrEmpty(code) || !int.TryParse(code, out var old_point))
            {
                return Ok(new { state = -2, msg = "验证码已过期请刷新重试" });
            }

            if (Math.Abs(old_point - point) > CaptchaHelper._deviationPx)
            {
                var li_count = 0;
                var errorNum = memoryCacheService.Get("code_errornum");
                if (!string.IsNullOrEmpty(errorNum) && int.TryParse(errorNum, out var num))
                {
                    li_count = num;
                }
                li_count++;

                if (li_count > CaptchaHelper._MaxErrorNum)
                {
                    memoryCacheService.Set("code", null);

                    return Ok(new { state = -1, msg = "超过最大错误次数" });

                }

                memoryCacheService.Set("code_errornum", li_count.ToString());

                return Ok(new { state = -1, msg = $"第【{li_count}】次验证错误" });
            }


            memoryCacheService.Set("isCheck", "OK");
            memoryCacheService.Set("code_errornum", string.Empty);
            memoryCacheService.Set("code", string.Empty);

            return Ok(new { state = 0, info = "正确", data = point });
        }

        [HttpPost]
        public IActionResult Verify()
        {
            var isOk = memoryCacheService.Get("isCheck");

            if (string.IsNullOrEmpty(isOk))
            {
                return Ok(new { errcode = -1, errmsg = "校验未通过，未进行过校验" });
            }

            if (isOk != "OK")
            {
                return Ok(new { errcode = -1, errmsg = "校验未通过" });
            }
            else
            {
                return Ok(new { errcode = 0, errmsg = "校验通过" });
            }
        }
    }
}
