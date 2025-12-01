export default{
    methods:{
              // 数据扁平化,记录所有分组
      formateEquips(list, parent) {
        for (let i = 0, length = list.length; i < length; i++) {
          // 如果是分组
          if (list[i].isGroup) {
            this.nodeObject[list[i].key] = {}
            this.nodeObject[list[i].key] = list[i]
            this.nodeObject[list[i].key].alarmCounts = 0  //报警数量
            this.nodeObject[list[i].key].backUpCounts = 0  //双机热备数量
            this.nodeObject[list[i].key].loading = false
            this.nodeObject[list[i].key].count = 0        //所挂载的设备数量
            this.nodeObject[list[i].key].groupUpdate = false  //分组是否已经加载了设备
            this.nodeObject[list[i].key].computedNum = false  //是否已经计算了设备
            this.nodeObject[list[i].key].childFormate = false //设备节点是否已经扁平化
            this.nodeObject[list[i].key].status = 1          //初始化分组状态为1（正常状态）
            this.nodeObject[list[i].key].nodeChilds = []     //初始化子分组节点列表为空
            this.nodeObject[list[i].key].groupId = list[i].groupId //记录父级Id

            if (parent) {
              this.nodeObject[list[i].key].parentKey = parent.key //记录父级key
              this.nodeObject[list[i].key].parentTitle = parent.title //记录父级title
              if (!list[i].isEquip) {  //如果当前非设备节点（当需要加载设备控制项时、设备也可为分组）
                // 记录所有子分组节点
                this.nodeObject[parent.key].nodeChilds.push(Number(list[i].key))
              }
            }
            if (list[i].children) {
              this.formateEquips(list[i].children, list[i])
            }

          }


          // 如果是设备
          if (list[i].isEquip) {
            let equipObj = this.searchName ? 'searchEquipObject' : 'equipObject'

            this[equipObj][list[i].key] = list[i]
            // this[equipObj][list[i].key].parent = parent
            this[equipObj][list[i].key].parentKey = parent.key
            this[equipObj][list[i].key].parentTitle = parent.title
          }
        }

      },
    }
}