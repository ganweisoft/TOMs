// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTCenterWebApi.BaseCore;

[Authorize(AuthenticationSchemes = "Bearer,Cookies")]
[ResponseCache(VaryByHeader = "User-Agent", Duration = 1)]
public class DefaultController : ControllerBase
{
}
