import { PTZCommandEnum } from "../h265webPlayer/Models";
import { AsyncSignalRController } from "../signalr/AsyncSignalRController";
import { Disposable, ITaskRequest, ITaskResponse, PageData, SuspenseTask, SuspenseTaskQueue } from './SuspenseTask'

export interface PTZRequest extends ITaskRequest {
    speed: number,
    command: number
}

export interface PTZResponse<T = any> extends ITaskResponse<T> {
    duration?: number,
    requestTime?: string,
    responseTime?: string
    userName?: string
}

export interface DefaultPTZControlStrategy {
    doPTZControl(cmd: PTZCommandEnum, args: object): PTZRequest
}

export interface PTZControlService extends Disposable {
    doPTZControl(cmd: PTZCommandEnum, args: object): Promise<PTZResponse>
    doPresetQuery(): Promise<PageData<IPresetData>>
}
export class PTZSuspenseTask extends SuspenseTask {
    override process(res: ITaskResponse<null>) {
        if (!res.id || res.id != this.id) {
            return false
        }
        super.process(res)
        this.result.push(res)
        if (!res.response) {
            this.failed = res
            this.reject(this.failed.responseMessage)
            return true
        }
        if (this.result.length >= this.size) {
            this.resolve({
                id: res.id,
                response: true,
                responseData: null,
                responseMessage: ''
            })
            return true
        }
        return false
    }
}

export interface IPresetData {
    presetId: number,
    presetName: string
}
export interface PresetQueryTaskResponse extends ITaskResponse<PageData<IPresetData>> {
    isCompleted: boolean
}

export class PresetQuerySuspenseTask extends SuspenseTask<PresetQueryTaskResponse, PageData<IPresetData>> {
    override process(res: PresetQueryTaskResponse) {
        if (!res.id || res.id != this.id) {
            return false
        }
        super.process(res)
        this.result.push(res)
        if (!res.response) {
            this.failed = res
            this.reject(this.failed.responseMessage)
            return true
        }
        if (res.isCompleted) {
            this.resolve(res.responseData)
            return true
        }
        return false
    }
}

export class DefaultPTZControlService implements PTZControlService {
    private timeout = 10000; // 10s
    private _id = 1
    asyncPTZSignalRController: AsyncPTZSignalRController
    strategies: Record<PTZCommandEnum, DefaultPTZControlStrategy | undefined> = {
        [PTZCommandEnum.StopPTZ]: undefined,
        [PTZCommandEnum.Up]: undefined,
        [PTZCommandEnum.UpRight]: undefined,
        [PTZCommandEnum.Right]: undefined,
        [PTZCommandEnum.RightDown]: undefined,
        [PTZCommandEnum.Down]: undefined,
        [PTZCommandEnum.DownLeft]: undefined,
        [PTZCommandEnum.Left]: undefined,
        [PTZCommandEnum.LeftUp]: undefined,
        [PTZCommandEnum.ZoomIn]: undefined,
        [PTZCommandEnum.ZoomOut]: undefined,
        [PTZCommandEnum.FocusPlus]: undefined,
        [PTZCommandEnum.FocusMinus]: undefined,
        [PTZCommandEnum.IrisOn]: undefined,
        [PTZCommandEnum.IrisOff]: undefined,
        [PTZCommandEnum.SetPreset]: undefined,
        [PTZCommandEnum.GetPreset]: undefined,
        [PTZCommandEnum.RemovePreset]: undefined,
        [PTZCommandEnum.UnKnow]: undefined,
        [PTZCommandEnum.StopFI]: undefined
    }
    private suspenseTaskQueue: SuspenseTaskQueue<PTZResponse>

    constructor(private deviceId: number, private nvrChannelId: number, timeout = 10000) {
        this.timeout = timeout
        this.suspenseTaskQueue = new SuspenseTaskQueue<PTZResponse>()
        this.asyncPTZSignalRController = new AsyncPTZSignalRController(this.suspenseTaskQueue.processSuspenseTask.bind(this.suspenseTaskQueue))
        this.initStrategies()
    }

    private generateId() {
        return this._id++
    }

    private initStrategies() {
        const directionPTZControlStrategy = new DirectionPTZControlStrategy()
        for (const cmd in PTZCommandEnum) {
            const cmdNumber = parseInt(cmd)
            if (isNaN(cmdNumber)) {
                continue
            }
            if (cmdNumber >= PTZCommandEnum.StopPTZ && cmdNumber <= PTZCommandEnum.ZoomOut) {
                this.strategies[cmd] = directionPTZControlStrategy
            }
        }
    }

    addStrategies(cmd: PTZCommandEnum, s: DefaultPTZControlStrategy) {
        if (this.strategies[cmd]) {
            console.warn(`PTZCommandEnum ${cmd} already has a strategy`)
        }
        this.strategies[cmd] = s
    }

    doPTZControl(cmd: PTZCommandEnum, args: object, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            command: cmd,
            speed: 0,
            ...args,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const data = this.strategies[cmd]?.doPTZControl(cmd, _args) || _args
        const task = new PTZSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncPTZSignalRController.invokePTZControl(data)
        return task.promise
    }

    doPresetQuery() {
        const id = this.generateId()
        const data = {
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new PresetQuerySuspenseTask(id, 2, this.timeout)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncPTZSignalRController.invokeGetPresets(data)
        return task.promise
    }

    dispose() {
        this.asyncPTZSignalRController.stop()
        this.suspenseTaskQueue.dispose()
        this.strategies = {} as Record<PTZCommandEnum, DefaultPTZControlStrategy | undefined>
    }
}

export class DirectionPTZControlStrategy implements DefaultPTZControlStrategy {
    doPTZControl(cmd: PTZCommandEnum, args: object) {
        return {
            ...args,
            command: cmd,
        } as PTZRequest
    }
}

export class AsyncPTZSignalRController extends AsyncSignalRController {
    eventName = 'ShowLog'
    onShowLog: (log: any, connectionId: string) => void = () => { /* empty */ }
    constructor(showLog?: AsyncPTZSignalRController['onShowLog']) {
        super('/mediaPlatformPTZ', false)
        if (showLog) {
            this.onShowLog = showLog
        }
        this.onShowLogMethod()
        this.start()
    }

    public invokePTZControl(data: PTZRequest) {
        this.signalR.invoke('PTZControl', data)
    }

    public invokeGetPresets(data: any) {
        if (!this.connectionResult.connection) {
            throw new Error('connection is not ready')
        }
        this.connectionResult.connection?.then(() => {
            this.signalR.invoke('PresetQuery', data)
        })
    }

    private onShowLogMethod() {
        this.signalR.on(this.eventName, (log: any) => {
            this.onShowLog(log, this.connectionId)
        })
    }

    stop() {
        this.signalR.off(this.eventName)
        this.onShowLog = () => { /* empty */ }
        super.stop()
    }
}
