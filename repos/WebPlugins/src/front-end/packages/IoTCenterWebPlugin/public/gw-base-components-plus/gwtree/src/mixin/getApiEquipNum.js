import api from 'gw-base-components-plus/gwtree/src/request/api.js'
export default {
    methods: {
        // 获取设备总数数量
        getEquipTotal () {
            api.getEquipCount().then(res => {
                if (res.data.code == 200) {
                    this.allEquipNum = res.data.data
                    this.$emit('getTotal', this.allEquipNum)
                    // 初始化设备数据
                    this.initData()
                    // 是否已经全部从缓存加载完
                    this.aleadyLoadAll = this.allEquipNum <= this.statisticsNum

                    if (this.aleadyLoadAll) {
                        // 如果加载完，则设置对应的设备状态
                        // this.setStatus(this.statusList)
                        // 通知模块已经全部加载完
                        this.$emit('loadAll', true)
                        this.groupLoading = false
                    } else {
                        this.SignalREquipConnect()
                    }
                }
            })
        },
    }
}