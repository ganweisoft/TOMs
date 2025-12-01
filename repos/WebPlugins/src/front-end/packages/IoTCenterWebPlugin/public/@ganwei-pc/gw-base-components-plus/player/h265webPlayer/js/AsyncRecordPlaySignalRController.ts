import { AsyncSignalRController } from "@/components/player/signalr/AsyncSignalRController"

import { ITaskResponse, SuspenseTask, SuspenseTaskQueue } from "../../classDefintion/SuspenseTask"

export interface RecordRequest {
    deviceId: number,
    nvrChannelId: number,
}

export interface IDeviceRecordsRequest extends RecordRequest {
    startTime: string,
    endTime: string
}

export interface IRandomDragRequest extends RecordRequest {
    time: number,
}

export interface IRecordStopPlayRequest extends RecordRequest {
    streamId: string
}

export interface IRecordSpeedPlayRequest extends RecordRequest {
    speed: number
}

export class AsyncRecordPlaySignalRController extends AsyncSignalRController {
    eventName = 'ShowLog'
    onShowLog: Array<(log: any, connectionId: string) => void> = []
    constructor(showLog?: (log: any, connectionId: string) => void) {
        super('/mediaPlatformRecordPlay', false)
        if (showLog) {
            this.onShowLog.push(showLog)
        }
        this.onShowLogMethod()
        this.start()
    }

    public sendDeviceRecordsQuery(data: IDeviceRecordsRequest) {
        return this.signalR.invoke('SendDeviceRecordsQuery', data)
    }

    public recordPlay(data: IDeviceRecordsRequest) {
        return this.signalR.invoke('RecordPlay', data)
    }

    public randomDragDropPlay(data: IRandomDragRequest) {
        return this.signalR.invoke('RandomDragDropPlay', data)
    }

    public recordStopPlay(data: IRecordStopPlayRequest) {
        return this.signalR.invoke('StopPlay', data)
    }

    public recordSpeedPlay(data: IRecordSpeedPlayRequest) {
        return this.signalR.invoke('SpeedPlay', data)
    }

    public recordPlayPause(data: RecordRequest) {
        return this.signalR.invoke('Pause', data)
    }

    public recordPlayResume(data: RecordRequest) {
        return this.signalR.invoke('Play', data)
    }

    private onShowLogMethod() {
        this.signalR.on(this.eventName, (log: any) => {
            this.onShowLog.forEach(fn => {
                fn(log, this.connectionId)
            })
        })
    }

    stop() {
        this.signalR.off(this.eventName)
        this.onShowLog = []
        super.stop()
    }
}

export interface Disposable {
    dispose(): void
}

export interface IRecordPlayFile {
    address: string,
    deviceID: string,
    startTime: string
    endTime: string
    filePath: string
    name: string
    recorderID?: number
    secrecy: number
    type: string
}

export interface IRecordFileTaskResponse {
    total: number,
    rows: IRecordPlayFile[]
}

export interface IRecordPlayResponse {
    deviceId: number,
    deviceName: string,
    deviceType: number,
    streamType: number,
    streamId: string,
    protocol: string,
    playStatus: number
    playInfos: Array<{
        protocol: string,
        url: string
    }>
}

export interface IRecordPlayTaskResponse extends ITaskResponse<IRecordPlayResponse> {
    duration: number
    playStatus: number
    requestTime: string
    responseTime: string
}

export class RecordOperateSuspenseTask extends SuspenseTask<ITaskResponse, boolean> {
    override process(res: ITaskResponse<any>) {
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
        if (!res.isCompleted) {
            return false
        }
        this.resolve(true)
        return true
    }
}

export class RecordFileSuspenseTask extends SuspenseTask<ITaskResponse<IRecordFileTaskResponse>, IRecordPlayFile[]> {
    total = 0
    resolveValue: IRecordPlayFile[] = []
    override process(res: ITaskResponse<IRecordFileTaskResponse>) {
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
        if (!res.isCompleted) {
            return false
        }
        const rows = res.responseData.rows || []
        this.resolve(rows)
        return true
    }
}

export class RecordPlaySuspenseTask extends SuspenseTask<IRecordPlayTaskResponse, IRecordPlayTaskResponse> {
    override process(res: IRecordPlayTaskResponse) {
        if (!res.id || res.id != this.id) {
            return false
        }
        super.process(res)
        if (res.responseData && res.responseData.playInfos && ([60, 100].includes(res.responseData.playStatus))) {
            this.resolve(res)
            return true
        }
        return false
    }
}

export interface RecordPlayService extends Disposable {
    doSendDeviceRecordsQuery(data: Omit<IDeviceRecordsRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void): Promise<any>
    doRecordPlay(data: Omit<IDeviceRecordsRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void): Promise<any>
    doRandomDragDropPlay(data: Omit<IRandomDragRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void): Promise<any>
}

export class DefaultRecordPlayService implements RecordPlayService {
    private timeout = 10000; // 1min
    private _id = 1
    asyncRecordPlaySignalRController: AsyncRecordPlaySignalRController
    private suspenseTaskQueue: SuspenseTaskQueue

    constructor(private deviceId: number, private nvrChannelId: number, timeout = 10000) {
        this.timeout = timeout
        this.suspenseTaskQueue = new SuspenseTaskQueue()
        this.asyncRecordPlaySignalRController = new AsyncRecordPlaySignalRController(this.suspenseTaskQueue.processSuspenseTask.bind(this.suspenseTaskQueue))
    }

    private generateId() {
        return this._id++
    }

    doSendDeviceRecordsQuery(data: Omit<IDeviceRecordsRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            ...data,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordFileSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.sendDeviceRecordsQuery(_args).catch(err => {
            task.reject('查询录像调用失败')
        })
        return task.promise
    }

    doRecordPlay(data: Omit<IDeviceRecordsRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            ...data,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordPlaySuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.recordPlay(_args).catch(err => {
            task.reject('播放录像调用失败')
        })
        return task.promise
    }

    doRandomDragDropPlay(data: Omit<IRandomDragRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            ...data,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordOperateSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.randomDragDropPlay(_args).catch(err => {
            task.reject('停止播放调用失败')
        })
        return task.promise
    }

    doRecordStopPlay(data: Omit<IRecordStopPlayRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            ...data,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordOperateSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.recordStopPlay(_args).catch(err => {
            task.reject('调用失败')
        })
        return task.promise
    }

    doRecordSpeedPlay(data: Omit<IRecordSpeedPlayRequest, 'deviceId' | 'nvrChannelId'>, onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            ...data,
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordOperateSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.recordSpeedPlay(_args).catch(err => {
            task.reject('调用失败')
        })
        return task.promise
    }

    doRecordPlayPause(onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordOperateSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.recordPlayPause(_args).catch(err => {
            task.reject('调用失败')
        })
        return task.promise
    }

    doRecordPlayResume(onMessage?: (res: any) => void) {
        const id = this.generateId()
        const _args = {
            id,
            deviceId: this.deviceId,
            nvrChannelId: this.nvrChannelId
        }
        const task = new RecordOperateSuspenseTask(id, 2, this.timeout, onMessage)
        this.suspenseTaskQueue.addSuspenseTask(task)
        this.asyncRecordPlaySignalRController.recordPlayResume(_args).catch(err => {
            task.reject('调用失败')
        })
        return task.promise
    }

    dispose(): void {
        this.asyncRecordPlaySignalRController.stop()
        this.suspenseTaskQueue.dispose()
    }
}
