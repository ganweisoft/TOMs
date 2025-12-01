import api from '../request/api'
export default class requestManage {
    constructor(nodesMap, equipControllObject) {
        this.nodesMap = nodesMap
        this.equipControllObject = equipControllObject
    }

    // 获取设备控制项
    async getSetting (key, name, level, checked) {
        let [groupId, equipNo] = String(key).split('-')
        let data = {
            equipNo: equipNo
        }

        await api.getSetParm(data).then(res => {
            if (res.data.code == 200) {
                let list = res && res.data && res.data.data && res.data.data.rows || []
                if (!this.nodesMap[key].settings) {
                    this.nodesMap[key].settings = []
                }
                if (list && list.length > 0 && this.nodesMap[key]) {
                    list.forEach(item => {
                        item.title = item.setNm;
                        item.key = `${groupId}-${equipNo}-${item.setNo}`;
                        item.level = Number(level) + 1;
                        item.checked = checked || this.equipControllObject[equipNo] && this.equipControllObject[equipNo].includes(item.setNo);
                        item.isSetting = true;
                        item.equipNo = equipNo;
                        item.groupId = groupId;
                        item.setNo = item.setNo
                        item.equipName = name
                    })
                    this.nodesMap[key].settings = [...list]
                }
            }
        })
    }

}
