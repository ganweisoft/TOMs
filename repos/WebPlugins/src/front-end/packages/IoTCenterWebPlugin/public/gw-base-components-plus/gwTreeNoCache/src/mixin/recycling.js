export default{

    methods:{
    // 注销树形结构
      destoryTree(obj) {
        for (let item in this.store.nodesMap) {
          this.store.deregisterNode(this.store.nodesMap[item])
        }

        this.root.childNodes.length = 0
        this.visibleList.length = 0
      },
        clearAll() {
            this.preNode = null
            this.showNumber = 0
            this.destoryTree()
            this.store.deregisterNode(this.root.childNodes[0])
            for (let item in this.nodeObject) {
              this.nodeObject[item].children.length = 0
              this.nodeObject[item].nodeChilds.length = 0
              this.updateWithEquips(item, [])
              // this.setExpand(item)
              this.nodeObject[item] = null
            }
            this.nodeObject = null
    
            for (let item in this.flatternEquipObject) {
              this.flatternEquipObject[item] = null
            }
            this.flatternEquipObject = null
            for (let item in this.equipObject) {
              this.equipObject[item] = null
            }
            this.equipObject = null
    
            for (let item in this.searchEquipObject) {
              this.searchEquipObject[item] = null
            }
            this.searchEquipObject = null
            this.originEquipMap = null
    
    
            this.$off('check');
            this.$off('saveOpenStatus');
            this.$off('node-expand');
    
            this.destory = true
            this.root = null;
            this.store.root = null
            this.store = null
            this.checkboxItems = null;
            this.treeItems = null
    
    
            this.SignalRStop()
          },

      // 内存回收，将扁平化时指向都置空
      // key:回收的分组节点
      recycling(key) {
        if (this.nodeObject[key]) {
        //   let groups = this.nodeObject[key].children.filter(item => item.isGroup && !item.isEquip)
          let childs = this.nodeObject[key].children.filter(item => item.isEquip)
          for (let index = 0, length = childs.length; index < length; index++) {
            if (this.equipObject[childs[index].key]) {
              this.equipObject[childs[index].key] = null
            }
            if (this.searchEquipObject[childs[index].key]) {
              this.searchEquipObject[childs[index].key] = null
            }
            if (this.nodeObject[childs[index].key]) {
              this.nodeObject[childs[index].key] = null
            }
          }
          if(this.nodeObject[key].nodeChilds){
            this.nodeObject[key].nodeChilds.forEach(item=>{
              if(this.store.nodesMap[item].expanded){
                  this.recycling(item)
                  this.store.nodesMap[item].expanded = false
              }
            })
          }

          this.nodeObject[key].children.length = 0
          // 清空当前key分组设备节点
          this.updateWithEquips(key, [])
          this.nodeObject[key].childFormate = false
          this.nodeObject[key].groupUpdate = false
        }
      },
    }
    

}