// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Net.Sockets;

namespace IoTClient.Common.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class SocketHelper
    {
        /// <summary>
        /// 安全关闭
        /// </summary>
        /// <param name="socket"></param>
        public static void SafeClose(this Socket socket)
        {
            return;
            try
            {
                if (socket?.Connected ?? false)
                    socket?.Shutdown(SocketShutdown.Both);//正常关闭连接
                else
                {
                    //如果连接打开，就不要释放
                    return;
                }
            }
            catch { }

            try
            {
                socket?.Close();
                socket.Dispose();
            }
            catch { }
        }
    }
}
