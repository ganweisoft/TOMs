// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenter.Utilities
{
    public class OperateResult
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; }
        public bool Succeeded { get; set; }

        public static readonly OperateResult Success = new OperateResult()
        {
            Succeeded = true,
            Message = "操作成功"
        };

        public static OperateResult Failed(string message = "参数为空")
        {
            return new OperateResult()
            {
                Succeeded = false,
                Code = 400,
                Message = message
            };
        }

        public static OperateResult Failed(int code, string message)
        {
            return new OperateResult()
            {
                Succeeded = false,
                Code = code,
                Message = message
            };
        }

        public static OperateResult<T> Failed<T>(string message)
        {
            return new OperateResult<T>()
            {
                Succeeded = false,
                Code = 400,
                Message = message
            };
        }

        public static OperateResult<T> Failed<T>(string message, T data)
        {
            return new OperateResult<T>()
            {
                Succeeded = false,
                Code = 400,
                Message = "Failed",
                Data = data
            };
        }

        public static OperateResult<T> Failed<T>(int code, string message)
        {
            return new OperateResult<T>()
            {
                Succeeded = false,
                Code = code,
                Message = message
            };
        }

        public static OperateResult<T> Successed<T>(T data)
        {
            return new OperateResult<T>()
            {
                Code = 200,
                Message = "操作成功",
                Succeeded = true,
                Data = data
            };
        }

        public static OperateResult<T> Successed<T>(T data, string message)
        {
            return new OperateResult<T>()
            {
                Code = 200,
                Message = message,
                Succeeded = true,
                Data = data
            };
        }
        public static OperateResult SuccessedWithMessage(string message)
        {
            return new OperateResult()
            {
                Code = 200,
                Message = message,
                Succeeded = true,
            };
        }

    }
    public class OperateResult<T> : OperateResult
    {
        public T Data { get; set; }
    }
}
