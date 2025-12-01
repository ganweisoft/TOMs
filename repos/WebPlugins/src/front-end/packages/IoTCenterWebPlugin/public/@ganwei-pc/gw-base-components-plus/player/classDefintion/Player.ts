import { PTZCommandEnum } from "../h265webPlayer/Models/index"
import { IPresetData, PTZControlService, PTZResponse } from "./DefaultPTZControlService"
import { H265WebPlayer } from "./H265WebPlayer"
import { PageData } from "./SuspenseTask"

export interface AbstractPlayer<T = any> {
    instance: T
    PTZControlService?: PTZControlService
    init(url: string): void
    eventsOn(events: Record<string, (...args: any[]) => void>): boolean
    start(): void
    play(): void
    destroy(): void
    setVoice(voice: number): void
    pause(): void
    fullScreen(): void
    snapshot(): void
    enablePTZ(deviceId: number, nvrChannelId: number, ptzControl: number): void
    ptzControl(cmd: PTZCommandEnum, args: object): Promise<PTZResponse>
    doPresetQuery(): Promise<PageData<IPresetData>>
}

export enum SupportPlayerEnum {
    H265WebPlayer,
}

const SupportPlayerEnumMap = {
    [SupportPlayerEnum.H265WebPlayer]: H265WebPlayer,
} as const

export function createPlayer<T = any>(playerEnum: SupportPlayerEnum, config: object): AbstractPlayer<T> {
    return new SupportPlayerEnumMap[playerEnum](config)
}
