export default class utils {
    static formateList (list, level) {
        let arr = []
        for (const item of list) {
            let dataItem = {}
            dataItem.count = 0             //分组下挂载的设备总数(包含子孙分组所挂载的设备)
            dataItem.equipSelectCount = 0        // 设备选中数量（包含子孙分组设备选中数量）
            dataItem.controlSelectCount = 0        // 控制项选中数量（包含子孙分组设备选中数量）
            dataItem.equipCount = item.equipCount || 0        //分组下挂载的设备数量（仅当前分组，不包含子孙分组所挂载设备）
            dataItem.title = item.name
            dataItem.key = item.id
            dataItem.isGroup = true        //是否是分组
            dataItem.children = []
            dataItem.status = 1            //分组状态
            dataItem.level = level || 1      //分组层级，按照层级缩进
            dataItem.expand = !level       //是否展开
            dataItem.equips = []              //设备存储区,展开每次从缓存中拿，关闭分组即清空equips
            dataItem.groupId = item.parentId   //父级分组id
            dataItem.groups = []           //子分组节点存储区（不包含孙分组节点）
            dataItem.alarmCounts = 0       //当前分组设备报警数量(包含子孙分组)（2）
            dataItem.backUpCounts = 0       //当前分组双机热备数量(包含子孙分组)（6）
            dataItem.noComCounts = 0       //当前分组离线数量(包含子孙分组)（0）
            dataItem.normalCounts = 0       //当前分组正常设备数量(包含子孙分组)（1）
            dataItem.lsSetCounts = 0       //当前分组设置中设备数量(包含子孙分组)（3）
            dataItem.initializeCounts = 0   //当前分组初始化中设备数量(包含子孙分组)（4）
            dataItem.withdrawCounts = 0     //当前分组回撤设备数量(包含子孙分组)（5）
            dataItem.indeterminate = false
            dataItem.checked = false
            dataItem.visible = true
            dataItem.nodeEquipSelectCount = 0  //当前节点设备选中数量

            dataItem.checkedEquips = []
            dataItem.halfCheckedEquips = []

            dataItem.selectControlCount = 0
            if (item.children && item.children.length > 0) {
                dataItem.groups = [
                    ...this.formateList(item.children, level ? level + 1 : 2)
                ]
            }

            arr.push(dataItem)
        }
        return arr
    }

    static deepClone (source, level, needsSettings, parentId, equipCheckObject, equipStatusObject) {
        let arr = [];
        if (source) {
            for (let i = 0, length = source.length; i < length; i++) {
                arr.push(
                    {
                        isGroup: needsSettings,
                        key: `${parentId}-${source[i].id}`,
                        status: equipStatusObject[source[i].id] || 0,
                        title: source[i].title,
                        level: level,
                        expand: false,
                        isEquip: true,
                        loading: false,
                        indeterminate: equipCheckObject[source[i].equipNo] && equipCheckObject[source[i].equipNo].indeterminate || false,
                        checked: equipCheckObject[source[i].equipNo] && equipCheckObject[source[i].equipNo].checked || false,
                        groupId: parentId,
                        equipNo: source[i].id,
                        visible: true,
                        settings: []
                    }
                )
            }
        }

        return arr;
    }

    static copyOrigin (list) {
        let arr = []
        list.forEach(item => {
            arr.push({ ...item })
        })
        return arr
    }

    // 获取分组位置
    static getPosition (key, visibleList) {
        let index = 0;
        for (let i = 0, length = visibleList.length; i < length; i++) {
            if (visibleList[i].key == key) {
                index = i - 30;
                break;
            }
        }
        return index;
    }

    // 扁平化
    static flattern (data, groupNodeObject) {
        data.forEach(item => {
            if (item.isGroup) {
                groupNodeObject[`${item.key}`] = null;
                groupNodeObject[`${item.key}`] = item;
            }
            if (item.groups && item.groups.length) {
                this.flattern(item.groups, groupNodeObject)
            }
        })
    }


    /**
      * @description 生成唯一Id
      * @param {}  不用传参
      * @return {string}
    */
    static generateUUID () {
        let d = new Date().getTime();
        if (window.performance && typeof window.performance.now === 'function') {
            d += performance.now(); // use high-precision timer if available
        }
        let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
        });
        return uuid;
    }

    /**
       * @description  将普通列表转换为树结构的列表
       * @param {list} 可构建树形结构的普通列表  
       * @return {string}
    */
    static listToTreeList (list) { //
        const data = list;
        const result = [];

        const map = {};
        data.forEach(item => {
            map[item.id] = item;
        })
        data.forEach(item => {
            const parent = map[item.parentId];
            if (parent) {
                (parent.children || (parent.children = [])).push(item);
            } else {
                result.push(item);
            }
        })
        return result;
    }



}