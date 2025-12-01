import utils from '../utils/utils'
export default class searchManage {
    // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
    constructor(groupNodeObject, showSettings, aliasName) {
        this.showSettings = showSettings
        this.groupNodeObject = groupNodeObject
        this.aliasName = aliasName
    }

    // 触发搜索
    filterMethod (searchName) {
        for (let item in this.groupNodeObject) {
            if (searchName) {
                this.updateBySearch(item, searchName)
            }
        }
    }

    // 搜索状态将搜索的结果存放缓存中
    updateBySearch (item, searchName) {
        let arr = window.top[`group-${this.groupNodeObject[item].key}${this.aliasName}`]
        if (arr) {
            arr = arr.filter(item => item.title.includes(searchName))
            window.top[`group-${this.groupNodeObject[item].key}-search`] = utils.copyOrigin(arr)
        }
    }
}