using System;
using Newtonsoft.Json;

namespace IoTCenterCore.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        [JsonIgnore]
        public bool IsReadonly { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool Enable { get; set; } = true;
        public string Schedule { get; set; } = "* * * * *";
        public string Description { get; set; } = string.Empty;
    }
}
