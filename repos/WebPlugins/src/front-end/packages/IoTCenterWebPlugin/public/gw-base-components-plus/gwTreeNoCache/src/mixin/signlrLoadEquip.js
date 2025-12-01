import * as signalR from '@aspnet/signalr'
import api from '../request/api.js'
export default {
    methods: {

        // 更新设备数据 key(Number):当前设备分组key，arr(Array):需要挂载在当前分组的设备；cache(Boolean):数据是否从缓存拿；load(Boolean):展开情况下
        updateEquips(key, arr, cache, load, noEmit) {

            // && !this.nodeObject[key].groupUpdate
            if (this.nodeObject[key] && !this.searchName) {

                if(!window.top.storage){
                    window.top.storage={}
                }
                window.top.storage[`${this.nodeObject[key].title}-${key}`] = this.deepClone(arr)
                

                let equipNum = arr.length

                // 判断当前是否已经计算进缓存，没有，则更新缓存数量,
                if (!this.nodeObject[key].computedNum) {
                    this.statisticsNum = this.statisticsNum + equipNum
                    this.nodeObject[key].computedNum = true
                    this.setGroupNum(key, equipNum)
                    this.flatternEquips(window.top.storage[`${this.nodeObject[key].title}-${key}`])
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

                // 展开条件下直接加载
                if (load) {
                    this.timeoutLoad(key)
                }
                this.expandAndTimeoutLoad(key)
            }
        },

        // 设备状态连接
        SignalREquipConnect() {
            api.getEquipsByGroup({gatewayId:this.gatewayId}).then(res=>{
                if(res.data.code==200){
                    res.data.data.forEach(item=>{
                        this.updateEquips(item.key, item.children, false)
                    })
                }
            })

        }
    }
}