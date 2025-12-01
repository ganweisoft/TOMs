import Signalr from '../../equipProcessing/gwSignalr.js';
export default class equipStatusManage {
    constructor(nodesMap, equipStatusObject, groupNodeObject, statusChange, aliasName) {
        this.equipStatusObject = equipStatusObject
        this.groupNodeObject = groupNodeObject
        this.aleadyUpdateStatus = {}
        this.isGetAllEquipStatus = false
        this.nodesMap = nodesMap
        this.statusChange = statusChange
        this.openSignlr()
        this.aliasName = aliasName
        this.statusMap = {
            0: 'noComCounts',
            1: 'normalCounts',
            2: 'alarmCounts',
            3: 'lsSetCounts',
            4: 'initializeCounts',
            5: 'withdrawCounts',
            6: 'backUpCounts',
        }
        this.reverseStatusMap = {
            'noComCounts': 0,
            'normalCounts': 1,
            'alarmCounts': 2,
            'lsSetCounts': 3,
            'initializeCounts': 4,
            'withdrawCounts': 5,
            'backUpCounts': 6,
        }
    }

    // 创建singlar,并且通知后台推送全量设备状态
    openSignlr () {
        this.equipStatusSignlr = new Signalr('/equipStatusMonitor', '', '');
        this.equipStatusSignlr.openConnect().then(rt => {
            // 通知后台推送全量设备状态
            try {
                rt.invoke('GetAllEquipStatus')
            } catch (error) {
                console.log(error)
            }

            this.subscribeTo(rt, 'GetAllEquipStatus')
            // 通知后台推送差异设备状态

            rt.onclose(() => {
                this.openSignlr()
            })
        })
    }


    // 订阅后台推送的方法
    subscribeTo (signalr, func) {
        signalr.off(func)
        signalr.on(func, res => {
            if (res && res.isSuccess && res.data) {
                if (this[func]) {
                    this[func](res.data)
                } else {
                    this.notice({ func, data: res.data, key: res.groupId })
                }
                // 通知后台推送差异性设备状态
                if (func === 'GetAllEquipStatus') {
                    try {
                        signalr.invoke('GetEquipChangeStatus')
                    } catch (error) {
                        console.log(error)
                    }
                    this.subscribeTo(signalr, 'GetEquipChangeStatus')
                }
            }
        })
    }

    // 从singlar获取的全量设备状态
    GetAllEquipStatus (data) {
        this.isGetAllEquipStatus = true
        // 重置分组状态
        this.resetGroupStatusCounts()
        // 将设备状态保存到全局中，通过equipStatusObject对象进行保存
        Object.keys(data).forEach(equipNo => {
            this.equipStatusObject[equipNo] = data[equipNo]
        })

        // 根据内存中的设备、全局设备状态，计算每个状态的设备数量
        Object.keys(this.groupNodeObject).forEach(key => {
            this.updateByGroup(key)
        })
        this.updateAllGroupStatus()
    }

    //重新渲染分组状态，当操作设备的时候，如增、删、移动设备
    reRenderGroupStatus () {
        this.resetGroupStatusCounts()
        Object.keys(this.aleadyUpdateStatus).forEach(key => {
            this.aleadyUpdateStatus[key] = false
        })
        Object.keys(this.groupNodeObject).forEach(key => {
            this.updateByGroup(key)
        })
        this.updateAllGroupStatus()
    }

    /**
     * description:更新分组中设备状态数量
     * @param {id} key 分组key
     * @param {String} statusCountsMap 设备状态，格式为{noComCounts,normalCounts,alarmCounts,lsSetCounts,initializeCounts,withdrawCounts,backUpCounts}中的某一个
     * @param {Number} value 需要更新的数量
     */
    updateStatusCounts (key, statusCountsMap, value) {
        if (this.groupNodeObject[key]) {
            this.groupNodeObject[key][statusCountsMap] = this.groupNodeObject[key][statusCountsMap] + value;
            let groupId = this.groupNodeObject[key].groupId
            if (groupId) {
                this.updateStatusCounts(groupId, statusCountsMap, value)
            }
        }
    }

    /**
     * @description 从全量设备状态统计设备状态数量
     * @param {id} key 分组key
     */
    updateByGroup (key) {
        if (!this.aleadyUpdateStatus[key]) {
            let list = window.top[`group-${key}${this.aliasName}`] || []
            let length = list.length
            if (length) {
                this.aleadyUpdateStatus[key] = true
                this.updateStatusCounts(key, this.statusMap[1], length)

                list.forEach(equip => {
                    this.updateGroupStatusCounts(key, this.equipStatusObject[equip.equipNo], 1)
                    if (this.nodesMap[`${key}-${equip.equipNo}`]) {
                        this.nodesMap[`${key}-${equip.equipNo}`].status = this.equipStatusObject[equip.equipNo]
                    }
                })
            }
        }
    }

    /**
     * @description 更新分组中新旧状态的数量
     * @param {id} key 分组key
     * @param {Number} newStatus 新状态，在分组中，新状态数量+1
     * @param {Number} oldStatus 旧状态数量-1
     */
    updateGroupStatusCounts (key, newStatus, oldStatus) {
        if (this.nodesMap[key] && this.statusMap[newStatus] && this.statusMap[oldStatus]) {
            let newStatusMap = this.statusMap[newStatus], oldStatusMap = this.statusMap[oldStatus]
            // 新状态+1
            this.updateStatusCounts(key, newStatusMap, 1)
            // 旧状态-1
            this.updateStatusCounts(key, oldStatusMap, -1)
        }
    }

    /**
     * @description 当设备数据比较晚推送，设备状态数据先推送过来情况下，设备数据推送过来了，需要通过这个方法更新分组的各个设备状态数量
     * @param {id} key 分组key
     */
    updateGroupStatusByKey (key) {
        if (this.isGetAllEquipStatus) {
            this.updateByGroup(key)
            this.updateAllGroupStatus()
        }
    }

    /**
     * @description 更新所有分组状态
     */
    updateAllGroupStatus () {

        //  1:单纯某种颜色，分组就是某种状态颜色
        //  2：分组下有告警，分组告警
        //  3：分组下有离线的且有其他状态，则为告警
        //  4：有热备且有其他状态，以其他状态为主
        //  5：其他情况，以最后状态为主

        // 0: 'noComCounts',
        // 1: 'normalCounts',
        // 2: 'alarmCounts',
        // 3: 'lsSetCounts',
        // 4: 'initializeCounts',
        // 5: 'withdrawCounts',
        // 6: 'backUpCounts',
        Object.keys(this.groupNodeObject).forEach(key => {
            let item = this.groupNodeObject[key]
            let singleStatus = this.getSingleStatus(item)
            if (singleStatus !== -1) {
                // 1：获取单纯的某个状态颜色，状态如果不为-1,则为过滤到的某个状态
                this.nodesMap[item.key].status = singleStatus
            } else if (item.alarmCounts > 0) {
                //  2：分组下有告警，分组告警
                this.nodesMap[item.key].status = 2
            } else if (item.noComCounts > 0 && (item.normalCounts > 0 || item.lsSetCounts > 0 || item.initializeCounts > 0 || item.withdrawCounts > 0 || item.backUpCounts > 0)) {
                //  3：分组下有离线的且有其他状态，则为告警
                this.nodesMap[item.key].status = 2
            } else if (item.backUpCounts > 0 && (item.normalCounts > 0 || item.lsSetCounts > 0 || item.initializeCounts > 0 || item.withdrawCounts > 0)) {
                // 4：有热备且有其他状态，以其他状态为主
                let arr = this.getStatusArray(item).filter(child => child.statusMap != 'backUpCounts' && child.value > 0)

                if (arr.length > 0) {
                    this.nodesMap[item.key].status = arr.pop().status
                } else {
                    this.nodesMap[item.key].status = 6
                }
            } else if (item.normalCounts > 0 || item.lsSetCounts > 0 || item.initializeCounts > 0 || item.withdrawCounts > 0) {
                // 5：其他情况，以最后状态为主
                let arr = this.getStatusArray(item).filter(child => child.value > 0)
                if (arr.length > 0) {
                    this.nodesMap[item.key].status = arr.pop().status
                } else {
                    this.nodesMap[item.key].status = 1
                }
            }
        })
    }

    /**
     *
     * @param {Object} groupItem 当前分组对象，保存各个设备状态数量，该方法为提取这些数量，以数组形式保存
     * @returns
     */
    getStatusArray (groupItem) {
        let statusValues = []
        let statusMaps = Object.values(this.statusMap)
        Object.keys(groupItem).forEach(key => {
            if (statusMaps.includes(key)) {
                statusValues.push({
                    statusMap: key,
                    status: this.reverseStatusMap[key],
                    value: groupItem[key]
                })
            }
        })
        return statusValues
    }

    /**
     * @description 获取单纯的某个状态颜色
     * @param {Object} groupItem
     * @returns {Number} 如果分组下设备只有一种状态，则返回该状态，否则返回-1
     */
    getSingleStatus (groupItem) {
        let statusValues = this.getStatusArray(groupItem)
        let nums = statusValues.filter(item => item.value == 0)
        if (nums.length == statusValues.length) {
            return 1
        } else if (nums.length == 6) {
            let item = statusValues.find(item => item.value > 0)
            if (item) {
                return item.status
            }
        }
        return -1
    }

    /**
     * @description singlar差异性推送的设备数据
     * @param {Object} data
     */
    GetEquipChangeStatus (data) {
        this.setStatus(data)
        this.updateAllGroupStatus()
    }

    /**
     * @description 设置singlar推送的设备状态
     * @param {Object} data
     */
    setStatus (data) {
        let status = this.equipStatusObject[data.equipNo]
        let groupId = window.top.equipCache && window.top.equipCache[data.equipNo] && window.top.equipCache[data.equipNo].groupId
        if (data.status != 3 && groupId && status != data.status) {
            this.updateGroupStatusCounts(groupId, data.status, status)

            // 更新缓存设备状态
            this.equipStatusObject[data.equipNo] = data.status
            let equipKey = `${groupId}-${data.equipNo}`

            // 更新已展开的设备状态
            this.nodesMap[equipKey] && (this.nodesMap[equipKey].status = data.status)

            // 通知外层当前设备状态已经改变
            this.statusChange(groupId, data.equipNo, data.status)

            // 更新所有分组状态
            this.updateAllGroupStatus()
        }
    }

    // 重置分组状态
    resetGroupStatusCounts () {
        for (let item in this.groupNodeObject) {
            Object.values(this.statusMap).forEach(status => {
                this.groupNodeObject[item][status] = 0
            })
        }
    }

    countGroupStatus (isSearchStatus) {
        this.resetGroupStatusCounts()
        for (let key in this.groupNodeObject) {
            let arr = []
            if (isSearchStatus) {
                arr = window.top[`group-${key}-search`]
            } else {
                arr = window.top[`group-${key}${this.aliasName}`]
            }
            let num = arr ? arr.length : 0
            if (num) {
                for (let equip of arr) {
                    const status = this.equipStatusObject[equip.equipNo]
                    this.setGroupNum(equip.groupId, status)
                }
            }
        }
    }

    setGroupNum (groupId, status) {
        if (!this.groupNodeObject[groupId]) {
            return
        }

        this.groupNodeObject[groupId][this.statusMap[status]] += 1
        this.setGroupNum(this.groupNodeObject[groupId].groupId, status)
    }
}

