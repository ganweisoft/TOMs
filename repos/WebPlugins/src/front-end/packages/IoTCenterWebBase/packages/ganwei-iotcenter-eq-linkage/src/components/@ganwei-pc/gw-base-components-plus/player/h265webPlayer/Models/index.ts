export interface IStreamInfo {
    title: string;
    id: string;
    protocol: string;
    name: string;
    tips: string;
    url: string;
    deviceId: number;
    nvrChannelId: number;
    ptzControl: number;
}

export enum PTZControlTypeEnum {
    GB28181 = 10
}

export enum PTZCommandEnum {
    /// <summary>
    /// 停止PTZ
    /// </summary>
    StopPTZ = 0,
    /// <summary>
    /// 上
    /// </summary>
    Up = 1,
    /// <summary>
    /// 右上
    /// </summary>
    UpRight = 2,
    /// <summary>
    /// 右
    /// </summary>
    Right = 3,
    /// <summary>
    /// 右下
    /// </summary>
    RightDown = 4,
    /// <summary>
    /// 下
    /// </summary>
    Down = 5,
    /// <summary>
    /// 左下
    /// </summary>
    DownLeft = 6,
    /// <summary>
    /// 左
    /// </summary>
    Left = 7,
    /// <summary>
    /// 左上
    /// </summary>
    LeftUp = 8,
    /// <summary>
    /// 变焦放大
    /// </summary>
    ZoomIn = 9,
    /// <summary>
    /// 变焦缩小
    /// </summary>
    ZoomOut = 10,
    /// <summary>
    /// 停止FI
    /// </summary>
    StopFI = 20,
    /// <summary>
    /// 聚焦近
    /// </summary>
    FocusPlus = 21,
    /// <summary>
    /// 聚焦远
    /// </summary>
    FocusMinus = 22,
    /// <summary>
    /// 光圈放大
    /// </summary>
    IrisOn = 23,
    /// <summary>
    /// 光圈缩小
    /// </summary>
    IrisOff = 24,
    /// <summary>
    /// 设置预置位
    /// </summary>
    SetPreset = 30,
    /// <summary>
    /// 调用预置位
    /// </summary>
    GetPreset = 31,
    /// <summary>
    /// 删除预置位
    /// </summary>
    RemovePreset = 32,
    /// <summary>
    /// 未知
    /// </summary>
    UnKnow = 100
}
