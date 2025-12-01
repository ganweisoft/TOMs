import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr"

export class AsyncSignalRController {
    signalR: HubConnection
    url = ''
    connectionId = ''

    autoCloseTimeout = -1
    autoTimeout = true
    isStop = false

    connectionResult: { connection: Promise<void> | null, success: boolean } = {
        connection: null,
        success: false,
    }

    constructor(url: string, autoTimeout = true) {
        this.autoTimeout = autoTimeout
        if (url) {
            this.url = url
        }
        this.signalR = new HubConnectionBuilder()
            .withUrl(this.url)
            .build()
        this.signalR.serverTimeoutInMilliseconds = 500000000;
        this.signalR.keepAliveIntervalInMilliseconds = 500000000

        this.onConnectionId()
    }

    start() {
        this.connectionResult.connection = this.signalR.start();
        this.connectionResult.connection.then(() => {
            this.connectionResult.success = true
        }).catch((err) => {
            this.connectionResult.success = false
            console.log(err);
        }).finally(() => {
            if (this.autoTimeout) {
                this.autoCloseTimeout = window.setTimeout(() => {
                    this.stop()
                }, 30000)
            }
        })
    }

    private onConnectionId() {
        this.signalR.on('ConnectionSucceeded', (res) => {
            this.connectionId = res.data
        })
    }

    stop() {
        if (this.isStop) {
            return
        }
        this.isStop = true
        console.log('stop', this.connectionId);
        this.connectionResult = {
            connection: null,
            success: false,
        }
        if (this.signalR.state === 1) {
            this.signalR.stop()
        }
    }
}
