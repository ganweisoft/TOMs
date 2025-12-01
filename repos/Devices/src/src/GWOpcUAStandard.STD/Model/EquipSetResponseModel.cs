// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace GWOpcUAStandard.STD
{
    public class EquipSetResponseModel
    {
        public int Code { get; set; } = 200;

        public string Message { get; set; }

        public object Data { get; set; }

        public void Fail(string msg, object data = null)
        {
            Data = data;
            Code = 400;
            Message = msg;
        }

        public void Ok(object data = null)
        {
            Data = data;
            Code = 200;
            Message = "命令执行成功";
        }
    }
}
