import {generateUUID } from '../model/util'
export default{
  watch:{
        // 搜索名字
        searchName(newVal, oldVal) {
            this.searchLoading = true
            // this.$emit('searching', true)
            // 清空选中状态
            this.setCheckedKeys([], false, [])
            // 重置选中节点数量
            this.resetSelectNum()
            this.alarmList = []
            this.backupList = []
    
             // 初始化分组所挂载的设备信息
            this.initEquipNum()
            // 初始化展开的分组
            this.initExpandKeys()
            this.id = generateUUID()
    
            let treetimeout = setTimeout(() => {
              for (let item in this.nodeObject) {
                this.nodeObject[item].childFormate = false
                this.updateWithEquips(item, [])
                if (newVal) {
                  let data = window.top.storage[this.nodeObject[item].title + '-' + item] || []
                  let searchEquip = data.filter(childItem => !childItem.isGroup && childItem.title.includes(newVal))
                  window.top.storage[this.nodeObject[item].title + '-' + item + '-search'] = this.deepClone(searchEquip)
                  let equipNum = searchEquip.length
                  this.setGroupNum(item, equipNum)
    
                } else {
                  let data = window.top.storage[this.nodeObject[item].title + '-' + item]
    
                  if (data) {
                    let equipNum = data.length
                    this.setGroupNum(item, equipNum)
                  }
                }
                // if (this.showStatus) {
                //   this.resolveData(item)
                // }
              }
    
              this.timeoutLoad(this.data[0].key)
    
              this.$nextTick(() => {
                this.$emit('searching', false)
                this.id = generateUUID()
              })
              clearTimeout(treetimeout)
              treetimeout = null
              this.searchLoading = false
            }, 200)
    
          },
    
  }
}