import Signalr from './gwSignalr.js';

export default class equipCache {
    constructor() {
        this.eGroupNotify = new Signalr('/eGroupNotify', '', '');
        this.alreadyUpdate = {}
        this.notify = null
    }

    stop () {
        if (this.notify) {
            this.notify.stop()
        }
    }

    Init () {

        //  打开signalr链接
        this.eGroupNotify.openConnect().then((rt) => {

            this.notify = rt;

            // 分组推送--start
            // 通知后台需要推送分组结构
            try {
                rt.invoke('GetEquipGroupTree');
                // 通知后台推送全量分组
                rt.invoke('GetAllEquipGroupTree')
            } catch (error) {
                console.log(error)
            }

            // 获取分组结构
            this.subscribeTo(rt, 'GetEquipGroupTree')
            // 新增分组
            this.subscribeTo(rt, 'AddEquipGroup')
            // 编辑分组
            this.subscribeTo(rt, 'EditEquipGroup')
            // 删除分组
            this.subscribeTo(rt, 'DeleteEquipGroup')
            // 分组推送--end


            // 设备推送--start
            // 通知后台需要推送分组结构

            try {
                rt.invoke('GetGroupEquips');
            } catch (error) {
                console.log(error)
            }
            // 订阅设备推送
            this.subscribeTo(rt, 'GetGroupEquips')
            // 订阅分组结构推送
            this.subscribeTo(rt, 'AddEquip')
            // 订阅分组结构推送
            this.subscribeTo(rt, 'DeleteEquip')
            // 订阅分组结构推送
            this.subscribeTo(rt, 'EditEquip')
            this.subscribeTo(rt, 'moveEquips')

            // 获取全量分组
            this.subscribeTo(rt, 'GetAllEquipGroupTree')
            // 设备推送--end

            rt.onclose((err) => {
                try {
                    this.Init()
                } catch (error) {
                    console.log(error)
                }

                console.log('重连', err);
            });

        }).catch((e) => {
            console.error(e)
        });
    }

    subscribeTo (signalr, func) {
        signalr.off(func)
        signalr.on(func, res => {
            if (res && res.isSuccess) {
                if (this[func]) {
                    this[func](res.data)
                } else {
                    this.notice({ func, data: res.data, key: res.groupId })
                }
            }
        })
    }

    // 获取分组---无权限管理的分组列表--空设备分组不展示
    GetEquipGroupTree (data) {
        if (!window.top.groupList) {
            window.top.groupList = data
            this.notice({ type: 'GetEquipGroupTree' })
        }
    }

    // 获取全量分组---设备管理使用
    GetAllEquipGroupTree (data) {
        console.log(8899, data)
        if (!window.top.groupList_manageMent) {
            window.top.groupList_manageMent = data
            this.notice({ type: 'GetEquipGroupTreeWidthTreeType' })
        }
    }

    // 新增分组
    AddEquipGroup (data) {
        const { parentGroupId, groupId, groupName } = data || {}
        if (groupId) {
            if (!window.top.groupList_manageMent) {
                window.top.groupList_manageMent = []
            }
            window.top.groupList_manageMent.push({
                parentId: parentGroupId,
                id: groupId,
                name: groupName,
                equipCount: 0
            })
            this.notice({ type: 'AddEquipGroup', data })
        }
    }

    // 编辑分组
    EditEquipGroup (data) {
        // 更新window缓存
        const { groupId, groupName } = data
        if (window.top.groupList) {
            window.groupList.forEach(item => {
                if (item.id == groupId) {
                    item.name = groupName
                }
            })
        }
        if (window.top.groupList_manageMent) {
            window.groupList_manageMent.forEach(item => {
                if (item.id == groupId) {
                    item.name = groupName
                }
            })
        }
        this.notice({ type: 'EditEquipGroup', data })
    }

    deleteChildGroup (parentId, list) {
        let deleteGroups = []
        list.forEach(item => {
            if (item.parentId == parentId) {
                deleteGroups.push(item.id)
            }
        })
        deleteGroups.forEach(groupId => {
            let index = list.findIndex(item => item.id == groupId)
            index > -1 && list.splice(index, 1)
            this.deleteChildGroup(groupId, list)
        })
    }


    // 删除分组
    DeleteEquipGroup (data) {
        data.forEach(group => {
            if (window.top.groupList) {
                let index = window.top.groupList.findIndex(item => item.id == group.groupId)
                index > -1 && window.top.groupList.splice(index, 1)
                this.deleteChildGroup(group.groupId, window.top.groupList)
            }
            if (window.top.groupList_manageMent) {
                let index = window.top.groupList_manageMent.findIndex(item => item.id == group.groupId)
                index > -1 && window.top.groupList_manageMent.splice(index, 1)
                this.deleteChildGroup(group.groupId, window.top.groupList_manageMent)
            }
        })
        this.notice({ type: 'DeleteEquipGroup', data })
    }

    // 获取设备
    GetGroupEquips (data) {
        const { groupId, equips } = data || {}
        console.log(this.alreadyUpdate[groupId])
        if (!this.alreadyUpdate[groupId]) {
            if (!window.top.groupCache) {
                window.top.groupCache = {}
            }
            window.top.groupCache[groupId] = {}
            if (groupId && equips && equips instanceof Array) {
                equips.forEach(item => {
                    item.title = item.name
                    item.groupId = groupId
                    item.equipNo = item.id
                    delete item.name

                    if (!window.equipCache) {
                        window.top.equipCache = {}
                    }
                    // 找分组,设备编辑需要用到
                    window.top.equipCache[item.id] = item

                    // 设备状态需要用到
                    window.top.groupCache[groupId][item.id] = item
                })

                window.top[`group-${groupId}`] = equips
                this.notice({ type: 'GetGroupEquips', data: { groupId: groupId } })
                this.alreadyUpdate[groupId] = true
            }
        }
    }


    // 新增设备
    AddEquip (data) {
        const { groupId, equips } = data || {}
        if (!window.top[`group-${groupId}`]) {
            window.top[`group-${groupId}`] = []
        }
        if (!window.top.groupCache) {
            window.top.groupCache = {}
        }
        if (!window.top.groupCache[groupId]) {
            window.top.groupCache[groupId] = {}
        }
        if (!window.top.equipCache) {
            window.top.equipCache = {}
        }
        let length = window.top[`group-${groupId}`].length
        if (groupId && equips) {
            equips.forEach((equip, index) => {
                window.top[`group-${groupId}`].push(
                    {
                        equipNo: equip.id,
                        groupId: groupId,
                        id: equip.id,
                        title: equip.name
                    }
                )

                // 找分组,设备编辑需要用到
                window.top.equipCache[equip.id] = window.top[`group-${groupId}`][length + index]

                // 设备状态需要用到
                window.top.groupCache[groupId][equip.id] = window.top[`group-${groupId}`][length + index]

            })

            if (!this.exist(groupId, window.top.groupList)) {
                let list = this.findParentList(groupId, window.top.groupList_manageMent)
                window.top.groupList.push(...list)
            }

            this.notice({ type: 'AddEquip', data })
        }
    }

    moveEquips (data) {
        // 统计需要更新的分组Id
        let updateGroups = []
        let buildTree = true
        let needAddToGroupList = []
        const { sourceGroup, targetGroupId } = data || {}
        targetGroupId && updateGroups.push(targetGroupId)
        sourceGroup.forEach(group => {
            updateGroups.push(group.groupId)
            if (!window.top[`group-${targetGroupId}`] || !window.top[`group-${targetGroupId}`].length) {
                window.top[`group-${targetGroupId}`] = []
                needAddToGroupList.push(targetGroupId)
            }
            if (!window.top.groupCache) {
                window.top.groupCache = {}
            }
            if (!window.top.groupCache[targetGroupId]) {
                window.top.groupCache[targetGroupId] = {}
            }

            group.equips.forEach(equip => {
                equip.title = equip.name
                equip.groupId = targetGroupId
                equip.equipNo = equip.id

                if (window.top[`group-${group.groupId}`]) {
                    let index = window.top[`group-${group.groupId}`].findIndex(item => item.id == equip.id)
                    index > -1 && window.top[`group-${group.groupId}`].splice(index, 1)
                }

                if (!window.equipCache) {
                    window.top.equipCache = {}
                }
                // 找分组,设备编辑需要用到
                window.top.equipCache[equip.id] = equip

                // 设备状态需要用到
                window.top.groupCache[targetGroupId][equip.id] = equip
                window.top[`group-${targetGroupId}`].push(equip)
            })

            if (!window.top[`group-${group.groupId}`].length) {
                let index = window.top.groupList.findIndex(item => item.id == group.groupId)
                index > -1 && window.top.groupList.splice(index, 1)
                this.deleteChildGroup(group.groupId, window.top.groupList)
            }
        })

        needAddToGroupList.forEach(groupId => {
            if (!this.exist(groupId, window.top.groupList)) {
                let list = this.findParentList(groupId, window.top.groupList_manageMent)
                window.top.groupList.push(...list)
            }
        })

        this.notice({ type: 'moveEquips', data: { updateGroups, buildTree } })
    }

    exist (key, list) {
        if (list) {
            return list.some(item => item.id == key)
        }
        return false
    }

    findParentList (groupId, list) {
        let parentList = []
        if (list) {
            let index = list.findIndex(group => group.id == groupId)
            if (index > -1) {
                parentList.push({ ...list[index] })
            }
            if (list[index].parentId && !this.exist(list[index].parentId, window.top.groupList)) {
                parentList.push(...this.findParentList(list[index].parentId, window.top.groupList_manageMent))
            }
        }
        return parentList
    }

    // 删除设备
    DeleteEquip (data) {
        const { groupId, equips } = data || {}
        if (groupId && equips && equips instanceof Array) {
            if (window.top[`group-${groupId}`]) {
                for (let i = 0, length = equips.length; i < length; i++) {
                    let index = window.top[`group-${groupId}`].findIndex(item => item.id == equips[i].id)
                    index > -1 && window.top[`group-${groupId}`].splice(index, 1)
                    //删除引用
                    delete window.top.equipCache[equips[i].id]
                    delete window.top.groupCache[groupId][equips[i].id]
                }
            }
            this.notice({ type: 'DeleteEquip', data })
        }
    }

    // 编辑设备
    EditEquip (data) {
        const { equipNo, groupId, equipName } = data || {}
        if (groupId && equipNo) {
            if (window[`group-${groupId}`]) {
                window.top.equipCache[equipNo] = equipName
            }
            this.notice({ type: 'EditEquip', data })
        }
    }

    notice (data) {
        // 下发通知
        if (window.top.hasIframe) {
            let iframe = document.getElementsByTagName('iframe');
            for (let item of iframe) {
                item.contentWindow.postMessage(data);
            }
        } else {
            window.postMessage(data, '*');
        }
    }

}