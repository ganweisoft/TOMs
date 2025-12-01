﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
namespace GWDataCenter
{
    interface ICanReset
    {
        bool ResetWhenDBChanged(params object[] o);
    }
}
