﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
namespace GWDataCenter
{
    public interface ISetItem
    {
        bool DoSetItem();
        bool m_bTemporarilyBreak
        {
            get;
            set;
        }
    }
}
