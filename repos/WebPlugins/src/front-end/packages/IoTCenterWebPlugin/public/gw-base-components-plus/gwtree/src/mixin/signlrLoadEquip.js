import * as signalR from '@aspnet/signalr'
export default {
    methods: {

        // 更新设备数据 key(Number):当前设备分组key，arr(Array):需要挂载在当前分组的设备；cache(Boolean):数据是否从缓存拿；load(Boolean):展开情况下
        updateEquips (key, arr, cache, load, noEmit) {

            // && !this.nodeObject[key].groupUpdate
            if (this.nodeObject[key] && !this.searchName) {

                window.top[`${this.nodeObject[key].title}-${key}`] = this.deepClone(arr)

                let equipNum = arr.length

                // 判断当前是否已经计算进缓存，没有，则更新缓存数量,
                if (!this.nodeObject[key].computedNum) {
                    this.statisticsNum = this.statisticsNum + equipNum
                    this.nodeObject[key].computedNum = true
                    this.setGroupNum(key, equipNum)
                    this.flatternEquips(window.top[`${this.nodeObject[key].title}-${key}`])
                }

                if (this.showStatus && !this.signalrConnectionEquip && arr.length > 0) {
                    let equipNo = typeof arr[0].key == 'string' ? arr[0].key.split('-')[1] : arr[0].key
                    this.connectHubEquipState(Number(equipNo))
                }

                if (!cache) {
                    if (!noEmit) {
                        this.$emit('updateGroup', key)
                    }
                }

                this.aleadyLoadAll = this.allEquipNum <= this.statisticsNum
                if (this.aleadyLoadAll) {
                    if (this.currentNodeKey && this.originEquipMap[this.currentNodeKey]) {
                        this.currentSelect = this.originEquipMap[this.currentNodeKey]
                    }
                    if (this.showStatus) {
                        this.resolveData()
                    }

                    if (this.signalREquip) {
                        let timeout = setTimeout(() => {
                            if (this.signalREquip) {
                                this.signalREquip.stop()
                                this.signalREquip = null
                            }

                            clearTimeout(timeout)
                            timeout = null
                        }, 2000)
                    }

                    if (this.showStatus) {
                        this.setStatus(this.statusList)
                    }

                    this.$emit('loadAll', true)
                    this.groupLoading = false
                }

                // 展开条件下直接加载
                if (load) {
                    this.timeoutLoad(key)
                }
                this.expandAndTimeoutLoad(key)
            }
        },

        // 设备状态连接
        SignalREquipConnect () {
            if (this.needSignalRLoad && !this.signalREquip) {
                if (this.signalREquip) {
                    this.signalREquip.stop()
                    this.signalREquip = null;
                }

                this.signalREquip = new signalR.HubConnectionBuilder()
                    .withUrl(this.$api.getSignalrHttp() + '/eGroupNotify', {})
                    .build();
                this.signalREquip.serverTimeoutInMilliseconds = 500000000;
                this.signalREquip.keepaliveintervalinmilliseconds = 500000000
                this.signalREquip
                    .start();

                // 判断连接状态
                this.signalREquip.off('eGroup');
                this.signalREquip.on('eGroup', (res) => {
                    // res.children[0].status = 6
                    if (res) {
                        if (res.key && this.nodeObject[res.key]) {
                            if (!this.nodeObject[res.key].computedNum) {
                                this.updateEquips(res.key, res.children, false)
                            }
                        }
                    }
                })

                this.signalREquip.onclose(async (error) => {
                    let timeout = setTimeout(() => {
                        if (!this.aleadyLoadAll && !this.destory) {
                            this.SignalREquipConnect()
                        }
                        clearTimeout(timeout)
                        timeout = null

                    }, 2000)
                })
            }

        }
    }
}