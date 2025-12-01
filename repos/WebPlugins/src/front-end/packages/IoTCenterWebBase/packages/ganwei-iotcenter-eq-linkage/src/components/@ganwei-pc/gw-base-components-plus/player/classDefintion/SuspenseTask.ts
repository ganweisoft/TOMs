/* eslint-disable @typescript-eslint/ban-ts-comment */

export type ResultPair<T = ITaskResponse> = [true, T] | [false, string | undefined]

export interface Disposable {
    dispose(): void
}

export interface ITaskRequest {
    id: number | string,
    deviceId: number,
    nvrChannelId: number,
}

export interface PageData<T> {
    rows: T[],
    total: number
}

export interface ITaskResponse<T = object | null> {
    id: number | string
    response: boolean,
    responseData: T
    responseMessage: string,
    isCompleted: boolean,
}

export interface ISuspenseTask<T extends ITaskResponse = ITaskResponse> {
    process(res: T): boolean
}

export abstract class SuspenseTask<T extends ITaskResponse = ITaskResponse, R = T> implements ISuspenseTask<T> {
    size = 2
    result: T[] = []
    failed: T | null = null
    canceled = false
    promise: Promise<R>
    // @ts-ignore
    resolve: (value: R) => void
    // @ts-ignore
    reject: (reason: string) => void
    onMessage: (res: T) => void = () => {
        // empty
    }
    constructor(protected id: number | string, size = 2, timeout = 60000, onMessage?: (res: T) => void) {
        this.size = size
        onMessage && (this.onMessage = onMessage)
        this.promise = new Promise((resolve, reject) => {
            const timeoutId = setTimeout(() => {
                this.reject('命令超时')
            }, timeout)
            this.resolve = (val: any) => {
                resolve(val);
                clearTimeout(timeoutId);
            };
            this.reject = (val: any) => {
                reject(val);
                clearTimeout(timeoutId);
            };
        })
    }

    process(res: T) {
        this.onMessage(res)
        return false
    }

    cancel() {
        this.canceled = true
        this.reject('--canceled--')
    }

    static isCanceled(value: any) {
        return typeof value === 'string' && value === '--canceled--'
    }
}

export class SuspenseTaskQueue<T extends ITaskResponse = ITaskResponse> implements Disposable {
    private suspenseTasks: Array<SuspenseTask<T>> = []

    addSuspenseTask(task: SuspenseTask<any>) {
        this.suspenseTasks.push(task)
        return task
    }
    processSuspenseTask(res: T) {
        if (!res.id) {
            return
        }
        this.suspenseTasks = this.suspenseTasks.filter(task => {
            const isFinished = task.process(res)
            return !isFinished
        })
    }

    dispose() {
        for (const task of this.suspenseTasks) {
            task.cancel()
        }
        this.suspenseTasks = []
    }
}
