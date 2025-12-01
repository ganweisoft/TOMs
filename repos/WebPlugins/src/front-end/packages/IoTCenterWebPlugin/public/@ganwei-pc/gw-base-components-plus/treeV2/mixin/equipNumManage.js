import { WindowManager } from '../utils/windowManager'
export default class equipNumManage extends WindowManager {
    // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
    constructor(groupNodeObject, aliasName) {
        super()
        this.aliasName = aliasName
        this.groupNodeObject = groupNodeObject
    }

    // 重新计算分组设备数量
    resetGroupNum (isSearchStatus) {
        this.clearAllEquipNum()
        for (let key in this.groupNodeObject) {
            let arr = []
            if (isSearchStatus) {
                arr = this.window[`group-${key}-search`]
            } else {
                arr = this.window[`group-${key}${this.aliasName}`]
            }
            let num = arr ? arr.length : 0
            if (num) {
                this.groupNodeObject[key].equipCount = num
                this.setGroupNum(key, num)
            }
        }
    }

    clearAllEquipNum () {
        for (let key in this.groupNodeObject) {
            this.groupNodeObject[key].equipCount = 0
            this.groupNodeObject[key].count = 0
        }
    }


    // 设置分组设备数量
    setGroupNum (key, num) {
        if (this.groupNodeObject[key]) {
            this.groupNodeObject[key].count = Number(this.groupNodeObject[key].count) + Number(num)
            this.setGroupNum(this.groupNodeObject[key].groupId, num)
        }
    }

    // 获取设备总数
    getAllEquipsNum () {
        let total = 0
        for (let item in this.groupNodeObject) {
            if (this.window[`group-${this.groupNodeObject[item].key}${this.aliasName}`]) {
                total = total + this.window[`group-${this.groupNodeObject[item].key}${this.aliasName}`].length
            }
        }
        return total
    }

}
