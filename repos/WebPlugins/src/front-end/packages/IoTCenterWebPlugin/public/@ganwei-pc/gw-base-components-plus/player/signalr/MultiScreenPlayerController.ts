import { InjectionKey, ref } from "vue"

import { AsyncPlayerSignalRController } from "./AsyncPlayerSignalRController"
import { AsyncSignalRController } from "./AsyncSignalRController"

export const MultiScreenPlayerControllerKey: InjectionKey<ICanAddScreen> =
    Symbol('MultiScreenPlayerControllerKey')

export interface ICanAddScreen {
    addScreen(index: number, args: ConstructorParameters<typeof AsyncPlayerSignalRController>): AsyncSignalRController
}

const panelControl = {
    panelControl: true, // 控制条
    fullScreen: true, // 全屏
    play: false, // 播放
    pause: false, // 暂停
    stop: false, // 停止
    record: false, // 录像
    playback: false, // 回放
    speed: false, // 倍数
    time: false, // 时间
    volume: true, // 音量
    setting: false, // 设置
    share: false, // 分享
    more: false, // 更多
    close: true, // 关闭
    capture: false, // 截图
    title: true, // 标题
    beforePlayInfo: true, // 播放前信息
    streamList: true, // 流列表
    ptzControl: false, // 云台
}

export class MultiScreenPlayerController implements ICanAddScreen {
    private signalRSet: Array<AsyncSignalRController> = []
    private _panelControls = ref<Array<typeof panelControl>>([JSON.parse(JSON.stringify(panelControl))])
    closeScreenStrategy: ICloseScreenStrategy = defaultCloseScreenStrategy

    indexRef = ref(0)
    sizeRef = ref(1)

    get panelControls() {
        return this._panelControls.value
    }

    get currentPanelControl() {
        if (!this._panelControls.value[this.index]) {
            this._panelControls.value[this.index] = JSON.parse(JSON.stringify(panelControl))
        }
        return this._panelControls.value[this.index]
    }

    set index(val) {
        this.indexRef.value = val % (this.size)
    }

    get index() {
        return this.indexRef.value
    }

    get size() {
        return this.sizeRef.value
    }

    set size(val: number) {
        // Close Screen Strategy
        if (this.sizeRef.value > val) {
            // 减少
            this.sizeRef.value = val
            this.signalRSet = this.signalRSet.filter((signalR, index) => {
                const shouldClose = this.closeScreenStrategy(val, index)
                shouldClose && signalR.stop()
                return !shouldClose
            })
            this._panelControls.value = this._panelControls.value.filter((panelControl, index) => {
                const shouldClose = this.closeScreenStrategy(val, index)
                return !shouldClose
            })
            if (this.signalRSet.length > val) {
                this.signalRSet.slice(val).forEach((signalR) => {
                    signalR.stop()
                })
                this.signalRSet = this.signalRSet.slice(0, val)
                this._panelControls.value = this._panelControls.value.slice(0, val)
            }
            if (this.index > val) {
                this.index = val
            }
        } else {
            // 增加
            this.sizeRef.value = val
        }
        this.index = this.signalRSet.length
    }

    constructor(size?: number, closeScreenStrategy?: ICloseScreenStrategy) {
        if (size) {
            this.sizeRef.value = size
        }

        if (closeScreenStrategy) {
            this.closeScreenStrategy = closeScreenStrategy
        }
    }

    addScreen(index: number, args: ConstructorParameters<typeof AsyncPlayerSignalRController>) {
        if (this.signalRSet[index]) {
            console.log(index, 'stop')
            this.signalRSet[index].stop()
        }
        this.signalRSet[index] = new AsyncPlayerSignalRController(...args)
        this.index = index + 1
        return this.signalRSet[index]
    }

    createScreen() {
        this.index += 1
        return this.index
    }
}

export interface ICloseScreenStrategy {
    (size: number, index: number): boolean
}

/**
 * @param {number} size 容量
 * @param {number} index 当前索引
 * @param {AsyncPlayerSignalRController} signalR 异步播放推送
 * @returns {boolean} 返回 `true` 则删除
 */
export function defaultCloseScreenStrategy(size: number, index: number) {
    if (index > size) {
        return true
    }
    return false
}
