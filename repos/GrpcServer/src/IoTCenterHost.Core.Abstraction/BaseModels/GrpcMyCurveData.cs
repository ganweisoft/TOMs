//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.BaseModels
{
    public enum GrpcCurveDataState
    {
        Normal = 1,
        NoRecord = 2
    }
    public enum GrpcCurveType
    {
        Double = 1,
        State = 2
    }

    public struct GrpcMyCurveData
    {
        public DateTime datetime;
        public double value;
        public GrpcCurveDataState state;
    }

    public struct GrpcMyCurveData4Seq
    {
        public DateTime datetime;
        public List<double> datalist;
    }
    public struct GrpcMyCurveData4String
    {
        public DateTime datetime;
        public string info;
    }
}
