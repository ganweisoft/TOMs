export default{
  data(){
    return{
      // 记录选中状态列表
      groupObj: {},
    }
  },
  watch:{
        // 传进来的默认选择列表
    selectEquips(newVal) {
    this.equipSelectsByGroup = {}
    this.$nextTick(() => {
    this.mySetChecked(newVal)
    })
    },
    // 传进来的控制项列表（权限管理使用）
    controllList(newVal) {
    this.controllObject = {}
    // 半选设备
    let halfArray = []
    for (let index = 0, length = newVal.length; index < length; index++) {
    let arr = newVal[index].split('.')
    let equipNo = this.originEquipMap[arr[0]]
    halfArray.push(arr[0])
    if (!this.controllObject[equipNo]) {
      this.controllObject[equipNo] = {}
      this.controllObject[equipNo].list = []
    }
    this.controllObject[equipNo].list.push(Number(arr[1]))
    if (this.store.nodesMap[arr[0] + '-' + arr[1]]) {
      this.setChecked(arr[0] + '-' + arr[1], true, true)
    }
    }
    
    halfArray = Array.from(new Set(halfArray))
    this.mySetChecked(halfArray, true)
    
    },
    },
    
    methods:{
       // newVal(Array):设备数组,allHalf(Boolean):是否都是半选
       mySetChecked(newVal, allHalf) {

        // 从扁平化的数据中找到所属分组，再计算分组所拥有的设备数量
        for (let index = 0, length = newVal.length; index < length; index++) {
          let equipNo = this.originEquipMap[newVal[index]]
          if (this.flatternEquipObject[equipNo]) {
            let groupId = this.flatternEquipObject[equipNo].groupId
            if (!this.groupObj[groupId]) {
              this.groupObj[groupId] = {}
              this.groupObj[groupId].leng = 0
              this.groupObj[groupId].list = []
            }
            
            let type = 'checked'
            if (!allHalf) {
              //  this.setChecked(Number(newVal[index]),true,true)
            
            
              if (this.store.nodesMap[equipNo]) {
                this.store.nodesMap[equipNo].checked = true
              }
              if (!this.equipSelectsByGroup[groupId]) {
                this.equipSelectsByGroup[groupId] = []
              }
              this.groupObj[groupId].leng = this.groupObj[groupId].leng + 1
              this.equipSelectsByGroup[groupId].push(equipNo)
              this.isExpendEquip[equipNo] && this.setChecked(equipNo, true, true)
            
            } else {
              type = 'indeterminate'
              if (this.store.nodesMap[equipNo]) {
                this.store.nodesMap[equipNo].indeterminate = true
              }
            }
            
            this.groupObj[groupId].list.push({
              key: equipNo,
              type: type
            })
            
          }
        }
            
            
        if (allHalf) {
          // 设备半选情况（权限管理用到）
          for (let item in this.groupObj) {
            this.store.nodesMap[item].indeterminate = true
            // 将父级节点设置成半选
            this.setParentHalf(item)
          }
        } else {
          // 设备全选情况
          for (let item in this.groupObj) {
            
            if (window.top.storage[`${this.nodeObject[item].title}-${item}`].length != this.groupObj[item].leng) {
            
              this.store.nodesMap[item].indeterminate = true
              this.store.nodesMap[item].checked = false
              // 将父级节点设置成半选
              this.setParentHalf(item)
            } else {
              this.store.nodesMap[item].checked = true
              this.store.nodesMap[item].indeterminate = false
            }
          }
        }
            
      },
    // 设置父级分组节点半选
      setParentHalf(key) {
        if (this.nodeObject[key].parentKey) {
          this.store.nodesMap[this.nodeObject[key].parentKey].indeterminate = true
          this.store.nodesMap[this.nodeObject[key].parentKey].checked = false
          this.setParentHalf(this.nodeObject[key].parentKey)
        }

      },

        // 获取选中的设备以及设置控制项
        getChecked() {
           let equips = []
           let setting = []
           // 保存待有分组号的设备id
           let equipWithGroupId = []
           // this.initGroupCheckNum()
            
           for (let item in this.groupObj) {
             this.groupObj[item].list = []
             this.groupObj[item].list.length = 0
           }
            
           for (let item in this.equipSelectsByGroup) {
             // equips = [...equips, ...this.equipSelectsByGroup[item]]
             if (!this.groupObj[item]) {
               this.groupObj[item] = {}
               this.groupObj[item].list = []
             }
             let length = this.equipSelectsByGroup[item].length
            
             for (let index = 0; index < length; index++) {
               let key = this.equipSelectsByGroup[item][index]
               equipWithGroupId.push(key)
               if (typeof key == 'string') {
                 equips.push(Number(key.split('-')[1]))
               } else {
                 equips.push(key)
               }
            
               this.groupObj[item].list.push(
                 {
                   key: key,
                   type: 'checked'
                 }
               )
             }
           }
           equips = Array.from(new Set(equips))
           equipWithGroupId = Array.from(new Set(equipWithGroupId))

           Object.keys(this.controllObject).forEach(item=>{
            if(this.controllObject[item]&&this.controllObject[item].list){
              this.controllObject[item].list.forEach(childItem=>{
                setting.push(`${item.split('-').pop()}.${childItem}`)
              })
            let groupId = this.equipObject[item]?this.equipObject[item].groupId:''
            if(groupId){
              this.groupObj[groupId].list.push(
                {
                  key: item,
                  type: 'indeterminate'
                }
              )
            }
            }
           })
           setting = Array.from(new Set(setting))
            
           return { equips, setting, equipWithGroupId }
            
         },


       // 重置复选框
       resetChecked() {
        // this.setCheckedKeys([], false, [])
        this.resetSelectNum()
        this.groupObj = {}
        this.controllObject = {}
        // this.initGroupCheckNum()
      },

      
      // 更新选中状态
      refreshCheckStatus(key) {
        if (this.store.nodesMap[key].checked) {
          this.setChecked(key, true, true)
        } else {
          // 设置设备的选中状态
          this.$nextTick(() => {
            if (this.groupObj[key] && this.groupObj[key].list) {
              let list = this.groupObj[key].list
              list.forEach(item => {
                if (item.type == 'indeterminate') {
                  this.store.nodesMap[item.key].indeterminate = true
                } else {

                  this.store.nodesMap[item.key].checked = true
                  //  this.setChecked(item.key,true,true)
                }

              })
            }
          })

        }
      },

      // 清除分组已选设备
      clearGroupEquips(keys){
        keys.forEach(item=>{
          if(this.equipSelectsByGroup[item]){
            this.equipSelectsByGroup[item].length=0

            if (this.nodeObject[item].nodeChilds && this.nodeObject[item].nodeChilds.length > 0) {
              this.clearGroupEquips(this.nodeObject[item].nodeChilds)
            }
          }
        })
 
      },

      // 根据分组更新选中的设备，全选分组或者取消选择分组时调用
      // list(Array):所要更新的分组，type(String):更新类型，add为新增，delete为删除;
      updateToList(list, type) {
        list.forEach(item => {
          if (this.nodeObject[item]) {
            if (!this.equipSelectsByGroup[item]) {
              this.equipSelectsByGroup[item] = []
            }
            if (type == 'add') {
              let arr = []
              if (this.searchName) {
                arr = window.top.storage[`${this.nodeObject[item].title}-${this.nodeObject[item].key}-search`] || []
              } else {
                arr = window.top.storage[`${this.nodeObject[item].title}-${this.nodeObject[item].key}`] || []
              }

              if (arr) {
                for (let index = 0, length = arr.length; index < length; index++) {
                  this.equipSelectsByGroup[item].push(arr[index].key)
                }
              }
            } else {
              this.equipSelectsByGroup[item].length = 0
            }
            if (this.nodeObject[item].nodeChilds && this.nodeObject[item].nodeChilds.length > 0) {
              this.updateToList(this.nodeObject[item].nodeChilds, type)
            }
          }
        })
      },

    },

    created(){
        this.$on('check', (node) => {
            if (node.checked) {
              if (node.data.isGroup && !node.data.isEquip) {
                this.clearGroupEquips([node.data.key])
                this.updateToList([node.data.key], 'add',true)
              } else if (node.data.isEquip) {
                try {
                  let equipObj = this.searchName ? 'searchEquipObject' : 'equipObject'
                  let groupId = this[equipObj][node.data.key].parentKey
                  if (this[equipObj][node.data.key] && groupId) {
                    if (!this.equipSelectsByGroup[groupId]) {
                      this.equipSelectsByGroup[groupId] = []
                    }
                    this.equipSelectsByGroup[groupId].push(node.data.key)
                  }
                } catch (error) {
                  console.log(error)
                }

                if(this.controllObject[node.data.key]){
                  delete this.controllObject[node.data.key]
                }
    
              } else if (node.data.isSetting) {

                let groupId = this.equipObject[this.originEquipMap[node.data.equipNo]].groupId
                if (!this.equipSelectsByGroup[groupId]) {
                  this.equipSelectsByGroup[groupId] = [];
                }
                this.$nextTick(() => {
                  if (node.parent.checked) {
                    this.equipSelectsByGroup[groupId].push(this.originEquipMap[node.data.equipNo])
                    // 从半选列表中删除
                    if(this.controllObject[this.originEquipMap[node.data.equipNo]]){
                      delete this.controllObject[this.originEquipMap[node.data.equipNo]]
                    }
                  }else{
                    if(!this.controllObject[this.originEquipMap[node.data.equipNo]]){
                      this.controllObject[this.originEquipMap[node.data.equipNo]]={}
                      this.controllObject[this.originEquipMap[node.data.equipNo]].list=[]
                    }
                    this.controllObject[this.originEquipMap[node.data.equipNo]].list.push(Number(node.data.setNo))
                  }
                })
    
              }
    
            } else {
              if (node.data.isGroup && !node.data.isEquip) {
                this.updateToList([node.data.key], 'delete',true)
              } else if (node.data.isEquip) {
                let equipObj = this.searchName ? 'searchEquipObject' : 'equipObject'
                let arr = this.equipSelectsByGroup[this[equipObj][node.data.key].parentKey]
                for (let index = 0, length = arr.length; index < length; index++) {
                  if (arr[index] == node.data.key) {
                    arr.splice(index, 1);
                    break;
                  }
                }

                if(this.controllObject[node.data.key]){
                  delete this.controllObject[node.data.key]
                }
    
              } else if (node.data.isSetting) {
                let list = this.controllObject[this.originEquipMap[node.data.equipNo]]?this.controllObject[this.originEquipMap[node.data.equipNo]].list:''
                if(list){
                   list = list.filter(item=>item!=item.data.setNo)
                }
    
              }
            }
            this.$nextTick(()=>{
              this.setSelectNum()
              this.$emit('myGetChecked')
            })
          })
    }
}