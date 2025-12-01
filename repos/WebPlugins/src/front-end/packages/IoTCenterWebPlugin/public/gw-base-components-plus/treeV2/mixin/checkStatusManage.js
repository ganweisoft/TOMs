export default class checkStatusManage {
    constructor(groupNodeObject, nodesMap, equipControllObject, controlObject, equipCheckObject, aliasName) {
        // 设备控制项，以设备号为键，绑定已选择控制项列表，如：this.equipControllObject.xxx设备=[控制项]
        this.equipControllObject = equipControllObject
        //分组节点对象，树形结构的扁平化对象 
        this.groupNodeObject = groupNodeObject
        // 已展开的分组下所挂载的设备对象
        this.nodesMap = nodesMap

        // 所展开的设备控制项
        this.controlObject = controlObject
        //设备选中状态记录 equipCheckObject：{xxx设备号：{indeterminate: false,checked: false,groupId:xxx}}
        this.equipCheckObject = equipCheckObject
        // 搜索状态下
        this.isSearchStatus = false

        // 搜索状态下分组数据保存原有数据,选择节点时需要更新,非搜索状态下,清空
        this.searchStatusGroupObject = {}

        this.aliasName = aliasName
    }

    /**
     * @description 重置选中状态
     */
    resetCheckedStatus () {
        // 重置所有实例化节点选中效果
        Object.values(this.nodesMap).forEach(item => {
            this.setCheckStatus(item.key, false, false)
            if (item.isGroup && !item.isEquip) {
                // 清除选中设备
                this.clearCheckedEquips(item.key)

                // 清除之前选中的数量
                this.updateEquipSelectCount(item.key, true)

            }
            item.halfCheckedEquips && (item.halfCheckedEquips = [])
        })

        Object.keys(this.equipCheckObject).forEach(item => {
            this.setEquipCheckObject(item, false, false, this.equipCheckObject[item].groupId)
        })
    }

    // 搜索状态下,更新分组选中状态
    /**
     *  1:备份原有选中的设备数据量,父级分组Id
     *  2:清空之前分组的设备选中数量
     *  3:依照搜索结果,重新计算分组选中的设备数据量
     *  */
    reComputedCheckNum (isSearchStatus) {
        this.isSearchStatus = isSearchStatus
        if (this.isSearchStatus) {
            // 1备份原有数据,父级分组Id
            Object.values(this.groupNodeObject).forEach(item => {
                this.searchStatusGroupObject[item.key] = {
                    groupId: item.groupId,
                    equipSelectCount: item.equipSelectCount,
                    halfCheckedEquips: item.halfCheckedEquips,
                    ...this.searchStatusGroupObject[item.key]
                }
            })
            // 2:清空之前分组的设备选中数量
            Object.values(this.groupNodeObject).forEach(item => {
                this.updateEquipSelectCount(item.key, true)
                item.halfCheckedEquips = []
            })

            // 3:依照搜索结果,重新计算分组选中的设备数据量
            Object.values(this.groupNodeObject).forEach(item => {
                let list = window.top[`group-${item.key}-search`] || []
                list.forEach(equip => {
                    if (this.equipCheckObject[equip.equipNo] && this.equipCheckObject[equip.equipNo].checked) {
                        this.updateEquipSelectCount(item.key, false, 1)
                    } else if (this.equipCheckObject[equip.equipNo] && this.equipCheckObject[equip.equipNo].indeterminate) {
                        item.halfCheckedEquips.push(equip.equipNo)
                    }
                })
            })

        } else {
            // 非搜索状态下,还原原有数据
            Object.values(this.groupNodeObject).forEach(item => {
                let data = this.searchStatusGroupObject[item.key]
                if (data) {
                    item.equipSelectCount = data.equipSelectCount
                    item.halfCheckedEquips = [...data.halfCheckedEquips, ...item.halfCheckedEquips]
                    item.halfCheckedEquips = [...new Set(item.halfCheckedEquips)]
                }
            })
            this.searchStatusGroupObject = {}
        }
    }

    // 根据传过来的设备选中数据、回显选中状态
    /**
     *  1:记录进全局设备选中状态
     *  2:设备设备选中状态
     *  3:更新分组选中的设备数量
     *  4:更新展开的设备控制项选中状态
     *  5:更新分组选中状态
     *  */
    updateCheckedStatusWithEquips (list) {
        window.setTimeout(() => {
            for (let index = 0, length = list.length; index < length; index++) {
                // 从缓存中看有没有该设备，防止是脏数据
                let data = window.top.equipCache && window.top.equipCache[list[index]]
                if (data && data.groupId) {
                    // 1:记录进全局设备选中状态
                    this.setEquipCheckObject(list[index], true, false, data.groupId)
                    // 2:设备设备选中状态
                    this.setCheckStatus(`${data.groupId}-${list[index]}`, true, false)
                    // 3:更新分组选中的设备数量
                    this.updateEquipSelectCount(data.groupId, false, 1)
                    // 4:更新展开的设备控制项选中状态
                    this.updateExpandControlCheckStatus(data.groupId, list[index])
                }
            }
            // 5:更新分组选中状态
            this.updateGroupCheckStatus()
        }, 200)
    }

    // 更新展开的设备控制项选中状态
    updateExpandControlCheckStatus (groupId, equipNo) {
        let equipNode = this.nodesMap[`${groupId}-${equipNo}`]
        if (equipNode && equipNode.settings && equipNode.settings.length) {
            equipNode.settings.forEach(set => {
                if (this.nodesMap[`${groupId}-${equipNo}-${set.setNo}`]) {
                    this.setCheckStatus(`${groupId}-${equipNo}-${set.setNo}`, true, false)
                }
            })
        }

    }


    // 根据传过来的设备控制项，回显设备分组选中状态
    /**
     *  1:记录全局设备控制项选中
     *  2:设置设备和设备控制项选中状态
     *  3:记录分组的半选设备
     *  4:记录全局设备半选状态
     *  5:更新分组选中状态
     *  */
    updateCheckedStatusWithControls (list) {
        window.setTimeout(() => {
            // 半选设备
            for (let index = 0, length = list.length; index < length; index++) {
                let arr = list[index].split('.')
                let equipNo = arr[0]
                let setNo = arr[1]
                if (!this.equipControllObject[equipNo]) {
                    this.equipControllObject[equipNo] = []
                }
                // 1:记录全局设备控制项选中
                this.equipControllObject[equipNo].push(Number(setNo))
                if (window.top.equipCache && window.top.equipCache[equipNo]) {
                    let groupId = window.top.equipCache[equipNo].groupId
                    let setKey = `${groupId}-${equipNo}-${setNo}`
                    // 2:设置设备和设备控制项选中状态
                    if (this.nodesMap[setKey]) {
                        this.setCheckStatus(setKey, true, false)
                    }
                    if (this.nodesMap[`${groupId}-${equipNo}`]) {
                        this.setCheckStatus(`${groupId}-${equipNo}`, false, true)
                    }
                    // 3:记录分组的半选设备
                    this.nodesMap[`${groupId}`].halfCheckedEquips && (this.nodesMap[`${groupId}`].halfCheckedEquips.push(equipNo))
                    // 4:记录全局设备半选状态
                    this.setEquipCheckObject(equipNo, false, true, groupId)
                }

            }

            //  5:更新分组选中状态
            this.updateGroupCheckStatus()
        }, 100)
    }

    // 更新分组选中状态
    updateGroupCheckStatus () {
        Object.values(this.groupNodeObject).forEach(item => {
            if (item.count > 0 && item.count == item.equipSelectCount) {
                this.setCheckStatus(item.key, true, false)
            } else if ((item.count && item.equipSelectCount && item.count > item.equipSelectCount) || item.halfCheckedEquips.length) {
                this.setGroupHalfChecked(item.key)
            } else {
                this.setCheckStatus(item.key, false, false)
            }
        })
    }

    // 设置父级分组半选状态
    setGroupHalfChecked (key) {
        if (this.nodesMap[key]) {
            this.setCheckStatus(key, false, true)
        }
        if (this.nodesMap[key].groupId) {
            this.setGroupHalfChecked(this.nodesMap[key].groupId)
        }
    }

    // 设置设备控制项状态
    setControlStatus (setNos, groupId, equipNo, checked) {
        if (setNos) {
            setNos.forEach(item => {
                if (this.nodesMap[`${groupId}-${equipNo}-${item.setNo}`]) {
                    this.setCheckStatus(`${groupId}-${equipNo}-${item.setNo}`, checked, false)
                }
            })
        }
    }

    // 更新设备控制项选中状态
    updateControlCheckStatus () {
        Object.keys(this.controlObject).forEach(item => {
            let node = this.nodesMap[`${this.controlObject[item].groupId}-${item}`]
            if (node) {
                if (node.checked) {
                    let setNos = node.settings
                    this.setControlStatus(setNos, this.controlObject[item].groupId, item, true)

                } else if (!node.checked && !node.indeterminate) {
                    let setNos = node.settings
                    this.setControlStatus(setNos, this.controlObject[item].groupId, item, false)
                }
                else {
                    if (this.equipControllObject[item]) {
                        this.equipControllObject[item].forEach(setNo => {
                            let key = `${this.controlObject[item].groupId}-${item}-${setNo}`
                            if (this.nodesMap[key]) {
                                this.setCheckStatus(key, true, false)
                            }
                        })
                    }
                }
            }

        })
    }


    // 获取选中的分组
    getGroupChecked () {
        let group = []
        Object.keys(this.groupNodeObject).forEach(item => {
            if (this.groupNodeObject[item].checked) {
                group.push(item)
            }
        })
        return group
    }


    // 获取选中设备
    getEquipSelectd () {
        let equips = []
        Object.keys(this.equipCheckObject).forEach(equipNo => {
            if (this.equipCheckObject[equipNo].checked) {
                equips.push(Number(equipNo))
            }
        })
        return equips
    }

    // 获取选中的设备控制箱
    getControlSelected () {
        let controls = []
        Object.keys(this.equipControllObject).forEach(item => {
            controls.push(...this.equipControllObject[item].map(child => `${item}.${child}`))
        })
        return controls
    }

    // 点击选中
    /**
     *  1:如果是分组选择，触发分组选中事件
     *  2:如果是设备选择，触发设备选中事件
     *  3:如果是设备控制项选择，触发设备控制项选中事件
     *  */
    onChecked (node, isSearchStatus) {
        this.isSearchStatus = isSearchStatus || false
        if (node.isGroup && !node.isEquip) {
            this.selectGroup(node)
        } else if (node.isEquip) {
            this.selectEquip(node)
        } else if (node.isSetting) {
            this.selectControl(node)
        }
    }

    /**
     * 
     * @param {*} key 节点key
     * @param {*} checked 是否全选
     * @param {*} indeterminate 是否半选
     * @description 从全局映射中，设置节点选中状态
     */
    setCheckStatus (key, checked, indeterminate) {
        if (this.nodesMap[key]) {
            this.nodesMap[key].checked = checked
            this.nodesMap[key].isGroup && (this.nodesMap[key].indeterminate = indeterminate)
        }
    }

    // 清除选中的设备
    clearCheckedEquips (key) {
        if (this.groupNodeObject[key]) {
            this.groupNodeObject[key].equipSelectCount = 0
            this.groupNodeObject[key].halfCheckedEquips && this.groupNodeObject[key].halfCheckedEquips.forEach(item => {
                if (this.equipControllObject[item]) {
                    this.equipControllObject[item].forEach(child => {
                        // 清除选中状态
                        this.setCheckStatus(`${key}-${item}-${child}`, false, false)
                    })
                }
            })
            this.groupNodeObject[key].halfCheckedEquips = []
            if (this.groupNodeObject[key].groupId) {
                this.clearCheckedEquips(this.groupNodeObject[key].groupId)
            }
        }
    }

    // 搜索状态下，更新分组选中的设备数量
    updateSearchStatusObject (key, count) {
        if (this.searchStatusGroupObject[key]) {
            this.searchStatusGroupObject[key].equipSelectCount = this.searchStatusGroupObject[key].equipSelectCount + count
            if (this.searchStatusGroupObject[key].groupId) {
                this.updateSearchStatusObject(this.searchStatusGroupObject[key].groupId, count)
            }
        }
    }

    /**
     * 
     * @param {*} key 分组key
     * @param {*} clear 是否清除选中的数量
     * @param {*} count 数量
     * @description 更新分组设备选中数量
     */
    updateEquipSelectCount (key, clear, count) {
        if (this.nodesMap[key]) {
            if (clear) {
                this.nodesMap[key].equipSelectCount = 0
            } else {
                this.nodesMap[key].equipSelectCount = this.nodesMap[key].equipSelectCount + count
            }
            if (this.nodesMap[key].groupId) {
                this.updateEquipSelectCount(this.nodesMap[key].groupId, clear, count)
            }
        }
    }

    // 保存全局设备选中状态
    setEquipCheckObject (key, checked, indeterminate, groupId) {
        if (!this.equipCheckObject[key]) {
            this.equipCheckObject[key] = {}
        }
        this.equipCheckObject[key].checked = checked
        this.equipCheckObject[key].indeterminate = indeterminate
        this.equipCheckObject[key].groupId = groupId
    }


    // 更新当前展开的分组所选中的设备、设置分组选中的设备数量、更新选中状态（已经实例化的节点）
    updateEquipSelect (key, list, checked, count) {
        if (this.nodesMap[key]) {
            if (list.length) {
                list.forEach(item => {
                    if (checked) {
                        this.setEquipCheckObject(item.equipNo, true, false, key)
                        this.nodesMap[`${key}-${item.equipNo}`] && (this.setCheckStatus(`${key}-${item.equipNo}`, true, false))
                    } else {
                        this.setEquipCheckObject(item.equipNo, false, false, key)
                        this.nodesMap[`${key}-${item.equipNo}`] && (this.setCheckStatus(`${key}-${item.equipNo}`, false, false))
                    }
                    this.equipControllObject[item.equipNo] && (delete this.equipControllObject[item.equipNo])
                    if (this.nodesMap[key].halfCheckedEquips.includes(item.equipNo)) {
                        this.nodesMap[key].halfCheckedEquips = this.nodesMap[key].halfCheckedEquips.filter(equipNo => equipNo != item.equipNo)
                        if (this.searchStatusGroupObject[key]) {
                            this.searchStatusGroupObject[key].halfCheckedEquips = this.nodesMap[key].halfCheckedEquips
                        }
                    }
                })
            }
        }
    }


    // 根据分组更新选中的设备，全选分组或者取消选择分组时调用
    /**
     *  1:统计需要更新的设备数量
     *  2:把所有子孙分组设备全部统计，并且记录所有当前分组选中的设备，更新除分组之外的选中状态（已经实例化的节点）
     *  3:更新自身设备选择数量、向上级分组累加已经选中的设备数量
     *  4:更新搜索状态下分组副本数据
     *  */
    // list(Array):所要更新的分组，type(String):更新类型，add为新增，delete为删除;
    updateGroupSelect (list, checked) {
        list.forEach(item => {
            if (this.groupNodeObject[item]) {
                if (this.groupNodeObject[item].groups && this.groupNodeObject[item].groups.length > 0) {
                    this.updateGroupSelect(this.groupNodeObject[item].groups.map(i => i.key), checked)
                }
                let list = window.top[`group-${item}${this.aliasName}`] || []
                if (this.isSearchStatus) {
                    list = window.top[`group-${item}-search`] || []
                }

                // 1:统计需要更新的设备数
                let addCount = 0
                let deleteCount = 0
                list.forEach(equip => {
                    let node = this.equipCheckObject[equip.equipNo]
                    if (!node || !node.checked) {
                        addCount++
                    } else {
                        deleteCount--
                    }
                })

                let count = checked ? addCount : deleteCount

                // 1：把所有子孙分组设备全部统计，并且记录所有当前分组选中的设备、更新除分组之外的选中状态（已经实例化的节点）
                this.updateEquipSelect(item, list, checked, count)

                // 3:更新自身设备选择数量、向上级分组累加已经选中的设备数量
                this.updateEquipSelectCount(item, false, count)

                // 4:更新搜索状态下分组副本数据
                this.updateSearchStatusObject(item, count)

            }
        })
    }

    // 清除分组挂载的半选设备
    clearHalfCheckedEquips (list) {
        list.forEach(groupId => {
            this.groupNodeObject[groupId].halfCheckedEquips && this.groupNodeObject[groupId].halfCheckedEquips.forEach(equip => {
                if (this.equipControllObject[equip]) {
                    this.equipControllObject[equip].forEach(setNo => {
                        // 清除选中状态
                        this.setCheckStatus(`${groupId}-${equip}-${setNo}`, false, false)
                    })
                }
            })
            this.groupNodeObject[groupId].halfCheckedEquips = []

            if (this.groupNodeObject[groupId].groups && this.groupNodeObject[groupId].groups.length > 0) {
                this.clearHalfCheckedEquips(this.groupNodeObject[groupId].groups.map(i => i.key))
            }
        })

    }

    // 触发分组选择
    /**
     * 1:清除之前半选设备
    *  2:更新分组中设备选择
    *  3:更新所有分组选中状态
    *  4:更新控制项选中状态
    *  */
    selectGroup (node) {
        // 1:清除之前半选设备
        this.clearHalfCheckedEquips([node.key])
        // 2:更新分组中设备选择
        this.updateGroupSelect([node.key], node.checked)
        // 3:更新控制项选中状态
        this.updateControlCheckStatus()
        // 4:更新所有分组选中状态 
        this.updateGroupCheckStatus()
    }


    // 触发设备选择
    /**
    *  1:更新分组选择的设备
    *  2:更新分组所选择的设备数量
    *  3:更新控制项选中状态
    *  4:更新分组状态
    *  */
    selectEquip (node) {
        if (this.nodesMap[node.groupId]) {
            let list = [{ equipNo: node.equipNo }]
            let count = (node.checked ? 1 : -1) * list.length
            // 1:更新分组选择的设备
            this.updateEquipSelect(node.groupId, list, node.checked, count)
            // 2:更新分组所选择的设备数量
            this.updateEquipSelectCount(node.groupId, false, count)
            // 3:更新搜索状态下分组副本数据
            this.updateSearchStatusObject(node.groupId, count)
            // 4:更新控制项选中状态
            this.updateControlCheckStatus()
            // 5:更新分组状态
            this.updateGroupCheckStatus()
        }
    }

    // 更新设备控制项存储
    updateEquipControl (equipNo, setNo, checked) {

        if (!this.equipControllObject[equipNo]) {
            this.equipControllObject[equipNo] = []
        }
        if (checked) {
            this.equipControllObject[equipNo].push(setNo)
        } else {
            this.equipControllObject[equipNo] = this.equipControllObject[equipNo].filter(item => item != setNo)
        }

    }

    // 触发控制项选择
    /**
    *  1:更新设备控制项
    *  2:更新设备选中状态
       (1)选择情况下：
            1）如果设备即将全选，清除分组中半选设备、清除设备控制项记录（equipControllObject），分组全选设备数组+1，半选设备数组中有则-1,设置设备全选
            2) 如果之前设备没选择情况下(非半选情况下)，分组中半选设备数组+1，设置设备半选
            3）如果设备是半选状态。不做处理
      （2）取消选择情况下
            1）如果设备是全选情况下、分组全选设备数组-1
                1）如果不仅有一个测点，设备半选分组+1，设备控制项（equipControllObject）记录剩余设备，设置设备半选
                2）如果仅有一个测点，设置设备不选
            2) 如果设备是半选情况下，如果还是半选，不做处理，如果从半选到不选情况下，分组挂载的半选分组-1，设备控制项有则-1，设置设备不选
    *  3:更新分组状态
    *  */
    selectControl (node) {
        let equipNode = this.nodesMap[`${node.groupId}-${node.equipNo}`]
        if (equipNode) {
            this.updateEquipControl(node.equipNo, node.setNo, node.checked)
            if (node.checked) {
                // 如果设备所绑定的控制项与设备控制项记录（equipControllObject）长度相同情况下，则设备是全选
                // 要么是是半选,要么没选
                // 要么之前是半选(两个),要么只有一个
                // 如果之前是半选,现在还是半选,不处理
                if (equipNode.settings && equipNode.settings.length == this.equipControllObject[node.equipNo].length) {
                    delete this.equipControllObject[node.equipNo]
                    this.updateEquipSelectCount(node.groupId, false, 1)
                    this.updateSearchStatusObject(node.groupId, 1)
                    this.nodesMap[node.groupId].halfCheckedEquips = this.nodesMap[node.groupId].halfCheckedEquips.filter(equipNo => equipNo != node.equipNo)
                    this.setEquipCheckObject(node.equipNo, true, false, node.groupId)
                    this.setCheckStatus(`${node.groupId}-${node.equipNo}`, true, false)
                } else if (equipNode && !equipNode.indeterminate) {
                    this.nodesMap[`${node.groupId}`].halfCheckedEquips.push(node.equipNo)
                    this.setCheckStatus(`${node.groupId}-${node.equipNo}`, false, true)
                    this.setEquipCheckObject(node.equipNo, false, true, node.groupId)
                }
            } else {
                if (equipNode.checked) {
                    this.updateEquipSelectCount(node.groupId, false, -1)
                    this.updateSearchStatusObject(node.groupId, -1)
                    if (equipNode.settings.length > 1) {
                        this.nodesMap[node.groupId].halfCheckedEquips.push(node.equipNo)
                        let otherSetNos = equipNode.settings.map(item => item.setNo).filter(setNo => setNo != node.setNo)
                        otherSetNos.forEach(setNo => {
                            this.updateEquipControl(node.equipNo, setNo, true)
                        })
                        this.setEquipCheckObject(node.equipNo, false, true, node.groupId)
                        this.setCheckStatus(`${node.groupId}-${node.equipNo}`, false, true)
                    } else {
                        this.setEquipCheckObject(node.equipNo, false, false, node.groupId)
                        this.setCheckStatus(`${node.groupId}-${node.equipNo}`, false, false)
                    }
                } else if (equipNode.indeterminate) {
                    if (this.equipControllObject[node.equipNo] && !this.equipControllObject[node.equipNo].length) {
                        this.nodesMap[node.groupId].halfCheckedEquips = this.nodesMap[node.groupId].halfCheckedEquips.filter(equipNo => equipNo != node.equipNo)
                        this.setEquipCheckObject(node.equipNo, false, false, node.groupId)
                        this.setCheckStatus(`${node.groupId}-${node.equipNo}`, false, false)
                    }
                }
            }
            this.updateGroupCheckStatus()
        }

    }

}