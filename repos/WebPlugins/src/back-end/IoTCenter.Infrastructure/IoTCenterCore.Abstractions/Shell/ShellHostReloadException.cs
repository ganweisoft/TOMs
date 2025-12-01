using System;

namespace IoTCenterCore.Environment.Shell
{
    public class ShellHostReloadException : Exception
    {
        public ShellHostReloadException(string message) : base(message)
        {
        }
    }
}
