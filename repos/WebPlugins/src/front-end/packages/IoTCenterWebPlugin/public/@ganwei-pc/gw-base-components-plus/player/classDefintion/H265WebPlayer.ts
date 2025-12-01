// import './h265web/src/h265webjs'
import { PTZCommandEnum, PTZControlTypeEnum } from '../h265webPlayer/Models/index';
import { DefaultPTZControlService, IPresetData, PTZControlService } from './DefaultPTZControlService';
import { AbstractPlayer } from './Player';
import { PageData } from './SuspenseTask';

const PRESET_CONFIG = {
    "player": "glplayer0",
    "width": 640,
    "height": 360,
    "token": "base64:QXV0aG9yOmNoYW5neWFubG9uZ3xudW1iZXJ3b2xmLEdpdGh1YjpodHRwczovL2dpdGh1Yi5jb20vbnVtYmVyd29sZixFbWFpbDpwb3JzY2hlZ3QyM0Bmb3htYWlsLmNvbSxRUTo1MzEzNjU4NzIsSG9tZVBhZ2U6aHR0cDovL3h2aWRlby52aWRlbyxEaXNjb3JkOm51bWJlcndvbGYjODY5NCx3ZWNoYXI6bnVtYmVyd29sZjExLEJlaWppbmcsV29ya0luOkJhaWR1",
    "extInfo": {
        "probeSize": 8192,
        "ignoreAudio": 0,
        "coreProbePart": 0.1,
        "autoPlay": true,
        "cacheLength": 50,
        "rawFps": 24
    }
};

type H265WebPlayerConfig = Partial<typeof PRESET_CONFIG>

export class H265WebPlayer implements AbstractPlayer {
    config: H265WebPlayerConfig = {};
    instance: any
    PTZControlService?: PTZControlService
    performance = -1
    constructor(config: H265WebPlayerConfig) {
        if (config) Object.assign(this.config, PRESET_CONFIG, config);
    }

    eventsOn(events: Record<string, (e: any) => void>) {
        for (const event in events) {
            this.instance[event] = events[event]
        }
        this.instance['onReadyShowDone'] = this.onPerformance.bind(this)
        return true
    }

    onPerformance() {
        this.performance = performance.now() - this.performance
        this.instance?.onPerformance?.(this.performance)
    }

    init(url: string) {
        this.performance = performance.now()
        // eslint-disable-next-line @typescript-eslint/ban-ts-comment
        // @ts-ignore
        const result = window.new265webjs ? window.new265webjs : window.top.new265webjs
        this.instance = result(url, this.config);
    }

    start() {
        this.instance.do()
    }

    play() {
        this.instance.play()
    }

    destroy() {
        this.PTZControlService?.dispose()
        this.instance.release();
    }

    setVoice(voice: number): void {
        this.instance.setVoice(voice)
    }

    pause(): void {
        this.instance.pause()
    }
    fullScreen(): void {
        this.instance.fullScreen()
    }
    snapshot(): void {
        this.instance.snapshot()
    }

    enablePTZ(deviceId: number, nvrChannelId: number, ptzControl: number) {
        if (ptzControl === PTZControlTypeEnum.GB28181) {
            this.PTZControlService = new DefaultPTZControlService(deviceId, nvrChannelId)
        } else {
            throw new Error('暂不支持该PTZ协议')
        }
    }

    ptzControl(cmd: PTZCommandEnum, args: object) {
        if (this.PTZControlService) {
            return this.PTZControlService?.doPTZControl(cmd, args)
        }
        throw new Error('PTZControlService is not initialized')
    }

    doPresetQuery(): Promise<PageData<IPresetData>> {
        if (this.PTZControlService) {
            return this.PTZControlService?.doPresetQuery()
        }
        throw new Error('PTZControlService is not initialized')
    }
}
