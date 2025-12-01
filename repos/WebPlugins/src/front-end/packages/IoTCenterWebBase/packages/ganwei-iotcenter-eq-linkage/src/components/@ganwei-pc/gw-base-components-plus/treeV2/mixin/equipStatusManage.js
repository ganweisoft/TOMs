import Signalr from '../../equipProcessing/gwSignalr.js';
import { WindowManager } from '../utils/windowManager'
export default class equipStatusManage extends WindowManager {
    constructor(nodesMap, equipStatusObject, groupNodeObject, statusChange, aliasName) {
        super()
        this.equipStatusObject = equipStatusObject
        this.groupNodeObject = groupNodeObject
        this.aleadyUpdateStatus = {}
        this.isGetAllEquipStatus = false
        this.nodesMap = nodesMap

        this.statusChange = statusChange
        this.openSignlr()
        this.aliasName = aliasName
    }

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
            try {
                rt.invoke('GetEquipChangeStatus')
            } catch (error) {
                console.log(error)
            }

            rt.onclose(() => {
                this.openSignlr()
            })

            this.subscribeTo(rt, 'GetEquipChangeStatus')
        })

    }


    subscribeTo (signalr, func) {
        signalr.off(func)
        signalr.on(func, res => {
            if (res && res.isSuccess && res.data) {
                if (this[func]) {
                    this[func](res.data)
                } else {
                    this.notice({ func, data: res.data, key: res.groupId })
                }
            }
        })
    }

    GetAllEquipStatus (data) {
        this.isGetAllEquipStatus = true
        this.resetGroupStatus()
        Object.keys(data).forEach(equipNo => {
            this.equipStatusObject[equipNo] = data[equipNo]
        })
        Object.keys(this.groupNodeObject).forEach(key => {
            this.updateByGroup(key)
        })
    }

    updateByGroup (key) {
        if (!this.aleadyUpdateStatus[key]) {
            let list = this.window[`group-${key}${this.aliasName}`] || []
            if (list.length) {
                this.aleadyUpdateStatus[key] = true
                list.forEach(equip => {
                    if (this.equipStatusObject[equip.equipNo] == 0 || this.equipStatusObject[equip.equipNo] == 2) {
                        this.setGroupStatus(key, true, 2)
                    } else if (this.equipStatusObject[equip.equipNo] == 6) {
                        this.setGroupStatus(key, true, 6)
                    }
                    if (this.nodesMap[`${key}-${equip.equipNo}`]) {
                        this.nodesMap[`${key}-${equip.equipNo}`].status = this.equipStatusObject[equip.equipNo]
                    }
                })
            }
        }
    }

    updateGroupStatus (key) {
        if (this.isGetAllEquipStatus) {
            this.updateByGroup(key)
        }
    }

    GetEquipChangeStatus (data) {
        this.setStatus(data)
    }

    // 设置分组状态 key:扁平化数据中节点索引；type:类型（增加，减少）；status:状态（报警2、双机热备6）
    setGroupStatus (key, type, status) {
        if (type) {
            if (status == 2) {
                this.nodesMap[key].alarmCounts = this.nodesMap[key].alarmCounts + 1
                if (this.nodesMap[key].alarmCounts > 0) {
                    this.nodesMap[key].status = 2
                }
            } else {
                this.nodesMap[key].backUpCounts = this.nodesMap[key].backUpCounts + 1
                if (this.nodesMap[key].alarmCounts == 0 && this.nodesMap[key].backUpCounts > 0) {
                    this.nodesMap[key].status = 6
                }
            }

        } else {
            if (status == 2) {
                this.nodesMap[key].alarmCounts = this.nodesMap[key].alarmCounts - 1
                if (this.nodesMap[key].alarmCounts == 0) {
                    this.nodesMap[key].status = 1
                }
            } else {
                this.nodesMap[key].backUpCounts = this.nodesMap[key].backUpCounts - 1
                if (this.nodesMap[key].alarmCounts == 0) {
                    if (this.nodesMap[key].backUpCounts == 0) {
                        this.nodesMap[key].status = 1
                    } else {
                        this.nodesMap[key].status = 6
                    }
                }
            }

        }
        if (this.nodesMap[key].groupId) {
            this.setGroupStatus(this.nodesMap[key].groupId, type, status)
        }

    }

    setStatus (data) {
        let status = this.equipStatusObject[data.equipNo]
        let groupId = this.window.equipCache && this.window.equipCache[data.equipNo] && this.window.equipCache[data.equipNo].groupId
        if (data.status != 3 && groupId) {
            switch (data.status) {
                case 0:
                    // 由非离线转离线，告警+1
                    if (status != 0) {
                        this.setGroupStatus(groupId, true, 2)
                    }
                    if (status == 6) {
                        // 由双机热备转离线，告警+1，双机热备-1
                        this.setGroupStatus(groupId, false, 6)
                    }
                    break;
                case 1:
                    // 由双机热备状态转正常，双机热备-1
                    if (status == 6) {
                        this.setGroupStatus(groupId, false, 6)
                    } else if (status == 2 || status == 0) {
                        // 由告警或离线状态转正常，告警-1
                        this.setGroupStatus(groupId, false, 2)
                    }
                    break;
                case 2:
                    // 由非告警状态转告警，告警+1
                    if (status != 2) {
                        this.setGroupStatus(groupId, true, 2)
                    }
                    if (status == 6) {
                        // 由双机热备转告警，告警+1、双机热备-1
                        this.setGroupStatus(groupId, false, 6)
                    }
                    break;

                case 6:
                    if (status == 2 || status == 0) {
                        // 由告警或离线转双机热备，告警-1，双机热备+1
                        this.setGroupStatus(groupId, false, 2)
                    }
                    // 由非双机热备状态转双机热备,双机热备+1
                    if (status != 6) {
                        this.setGroupStatus(groupId, true, 6)
                    }
                    break;

            }
            // 更新缓存设备状态
            this.equipStatusObject[data.equipNo] = data.status
            let equipKey = `${groupId}-${data.equipNo}`

            // 更新已展开的设备状态
            this.nodesMap[equipKey] && (this.nodesMap[equipKey].status = data.status)

            // 通知外层当前设备状态已经改变
            this.statusChange(groupId, data.equipNo, data.status)
        }
    }

    // 重置分组状态
    resetGroupStatus () {
        for (let item in this.groupNodeObject) {
            this.groupNodeObject[item].alarmCounts = 0;
            this.groupNodeObject[item].backUpCount = 0;
        }
    }
}

