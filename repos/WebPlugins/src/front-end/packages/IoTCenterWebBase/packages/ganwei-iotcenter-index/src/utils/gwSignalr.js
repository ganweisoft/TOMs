import * as signalR from '@aspnet/signalr';

export default class Signalr {
    constructor(path, connectionId, equipNo, reconnect) {
        this.url = path;
        this.connectionId = connectionId;
        this.equipNo = equipNo;
        this.signalr = null;
        this.reconnect = reconnect
    }

    openConnect () {

        // 停止连接
        if (this.signalr) {
            this.signalr.stop();
            this.signalr = null;
        }

        //  连接
        this.signalr = new signalR.HubConnectionBuilder()
            .withUrl(this.url)
            .build();

        // 停止监听
        this.signalr.onclose((err) => {
            if (this.reconnect) {
                try {
                    this.signalr.invoke('reconnect')
                } catch (error) {
                    console.log(error)
                }
            }
            // this.openConnect()
            console.log('onclose', err);
        });

        this.signalr.serverTimeoutInMilliseconds = 500000000;
        this.signalr.keepaliveintervalinmilliseconds = 500000000

        return new Promise((resolve, reject) => {
            this.signalr
                .start() // 启动连接
                .then(() => {
                    if (this.connectionId) {
                        this.send();
                    }

                    resolve(this.signalr);
                })
                .catch((e) => {
                    console.log(e);
                    reject(e);
                });
        });
    }

    async send () { // 发送请求
        try {
            await this.signalr.invoke(this.connectionId, this.equipNo);
        } catch (error) {
            console.log('connectHub 连接失败' + error);
        }
    }

}