export default class cacheManage {
    // 分组节点扁平化对象、设备扁平化对象、window缓存设备扁平化对象
    constructor(groupNodeObject, nodesMap, controlObject) {
        this.groupNodeObject = groupNodeObject
        this.nodesMap = nodesMap
        this.controlObject = controlObject
    }

    // 增加映射
    addNodesMap (list) {
        if (list) {
            list.forEach(item => {
                this.nodesMap[item.key] = item
            })
        }
    }

    // 移除映射
    removeNodesMap (list) {
        if (list) {
            list.forEach(item => {
                delete this.nodesMap[item.key]
            })
        }
    }

    // 回收所有分组节点缓存
    recycleAllNodeCache (isDelete) {
        for (let item in this.groupNodeObject) {
            this.groupNodeObject[item].expand = false
            this.groupNodeObject[item].count = 0
            this.recycleGroupCache(this.groupNodeObject[item].key)
            isDelete && delete this.groupNodeObject[item]
        }
    }

    // 回收分组节点内存
    recycleGroupCache (key) {
        if (this.groupNodeObject[key]) {
            this.groupNodeObject[key].children = []
            this.removeNodesMap(this.groupNodeObject[key].equips)
            this.groupNodeObject[key].equips = []
            this.groupNodeObject[key].equips.length = 0
            this.groupNodeObject[key].expand = false
            for (let i in this.groupNodeObject) {
                if (this.groupNodeObject[i].groupId == key) {
                    this.recycleGroupCache(this.groupNodeObject[i].key)
                }
            }
        }
    }


    // 关闭兄弟分组节点及回收内存
    closeBrotherNode (key) {
        if (!key) return;
        if (this.groupNodeObject[key]) {
            let groupId = this.groupNodeObject[key].groupId;
            for (let item in this.groupNodeObject) {
                if (this.groupNodeObject[item].groupId == groupId && this.groupNodeObject[item].key != key) {
                    if (this.groupNodeObject[item].expand) {
                        this.groupNodeObject[item].expand = false;
                        this.recycleGroupCache(this.groupNodeObject[item].key)
                    }
                }
            }
        }
    }

}