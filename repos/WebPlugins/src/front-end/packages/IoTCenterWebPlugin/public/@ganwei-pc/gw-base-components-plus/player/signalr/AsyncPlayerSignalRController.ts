import { AsyncSignalRController } from "./AsyncSignalRController"

export class AsyncPlayerSignalRController extends AsyncSignalRController {
    eventName = 'ShowLog'
    invokePlayMethod = 'Play'

    onShowLog: (log: any, connectionId: string) => void = () => { /* empty */ }

    constructor(private streamId: string, onShowLog: AsyncPlayerSignalRController['onShowLog']) {
        super('/mediaPlatformPlay')

        this.onConnectedSuccess()
        this.onShowLogMethod()
        this.onShowLog = onShowLog
    }

    private inVokePlay() {
        this.signalR.invoke(this.invokePlayMethod, this.streamId)
    }

    private onShowLogMethod() {
        this.signalR.on(this.eventName, (log: any) => {
            this.onShowLog(log, this.connectionId)
        })
    }

    private onConnectedSuccess() {
        this.signalR.on('ConnectionSucceeded', (res) => {
            if (this.streamId) {
                this.inVokePlay()
            }
        })
    }

    stop() {
        this.signalR.off(this.eventName)
        this.onShowLog = () => { /* empty */ }
        super.stop()
    }
}
