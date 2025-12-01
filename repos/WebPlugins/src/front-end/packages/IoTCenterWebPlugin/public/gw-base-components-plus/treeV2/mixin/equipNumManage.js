export default class equipNumManage {
    // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
    constructor(groupNodeObject, aliasName) {
        this.aliasName = aliasName
        this.groupNodeObject = groupNodeObject
    }

    // 重新计算分组设备数量
    resetGroupNum (isSearchStatus) {
        this.clearAllEquipNum()
        for (let key in this.groupNodeObject) {
            let arr = []
            if (isSearchStatus) {
                arr = window.top[`group-${key}-search`]
            } else {
                arr = window.top[`group-${key}${this.aliasName}`]
            }
            let num = arr ? arr.length : 0
            if (num) {
                this.groupNodeObject[key].equipCount = num
                this.setGroupNum(key, num)
            }
        }
    }

    /**
     * 清除所有设备数量
     */
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
            if (window.top[`group-${this.groupNodeObject[item].key}${this.aliasName}`]) {
                total = total + window.top[`group-${this.groupNodeObject[item].key}${this.aliasName}`].length
            }
        }
        return total
    }

}