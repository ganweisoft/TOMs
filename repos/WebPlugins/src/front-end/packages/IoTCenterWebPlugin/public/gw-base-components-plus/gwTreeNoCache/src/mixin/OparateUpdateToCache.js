
export default{
  methods:{
    // 删除分组
    // groupId(Number):所删除的分组key，deleteChildEquips(Boolean):是否同时删除设备，若否，则将设备往父级分组移动
    deleteGroup(groupId, deleteChildEquips) {
      let arr = this.deepClone(window.top.storage[this.nodeObject[groupId].title + '-' + groupId])
      if (!deleteChildEquips) {
        // 将数组往上一级分组移动
        let parentKey = this.nodeObject[groupId].parentKey
        let cacheName = this.nodeObject[parentKey].title + '-' + parentKey
        if (!window.top.storage[cacheName]) {
          window.top.storage[cacheName] = []
        }
        for (let index = 0, length = arr.length; index < length; index++) {
          window.top.storage[cacheName].push(
            {
              key: arr[index].key,
              title: arr[index].title,
              staNo: arr[index].staNo,
              status: 1,
              isGroup: false,
              groupId: parentKey,
              procAdvice: arr[index].procAdvice,
              relatedView: arr[index].relatedView
            }
          )
        }
      } else {
        // 减掉所删除的设备数量
        this.statisticsNum = Number(this.statisticsNum) - Number(arr.length)
        this.allEquipNum = this.allEquipNum - Number(arr.length)
      }

      // 更新缓存
      if (window.top.storage[this.nodeObject[groupId].title + '-' + groupId]) {
        window.top.storage[this.nodeObject[groupId].title + '-' + groupId].length = 0
        delete window.top.storage[this.nodeObject[groupId].title + '-' + groupId]
      }

      // 重新计算所选择的设备数量
      this.resetSelectNum()
    },

    // 当分组名字改变时，更新缓存中的名字
    updateGroupNameFromCache(groupId, newName) {
      // 如果搜索状态下存在旧分组缓存，直接删除
      if (window.top.storage[this.nodeObject[groupId].title + '-' + groupId + '-search']) {
        window.top.storage[this.nodeObject[groupId].title + '-' + groupId + '-search'].length = 0
        delete window.top.storage[this.nodeObject[groupId].title + '-' + groupId + '-search']
      }
      // 如果非搜索状态下，缓存中存在旧分组，则更新旧分组名称
      if (window.top.storage[this.nodeObject[groupId].title + '-' + groupId]) {
        window.top.storage[newName + '-' + groupId] = window.top.storage[this.nodeObject[groupId].title + '-' + groupId]
        window.top.storage[this.nodeObject[groupId].title + '-' + groupId] = null
        delete window.top.storage[this.nodeObject[groupId].title + '-' + groupId]
      }

    },


    // 新增设备
    // groupId(Number):将要添加设备所在的分组key，data(Array)：设备数组
    addEquip(groupId, data) {
      if (this.nodeObject[groupId]) {
        let equips = []
        data.forEach(item => {

          this.originEquipMap[item.equipNo] = `${groupId}-${item.equipNo}`

          equips.push({
            key: `${groupId}-${item.equipNo}`,
            title: item.equipName,
            staNo: item.staNo,
            status: 1,
            isGroup: false,
            groupId: groupId,
            procAdvice: item.procAdvice,
            relatedView: item.relatedView,
            isEquip: true,
            parentKey: groupId,
          })

          // 更新缓存
          if (!window.top.storage[this.nodeObject[groupId].title + '-' + groupId]) {
            window.top.storage[this.nodeObject[groupId].title + '-' + groupId] = []
          }
          window.top.storage[this.nodeObject[groupId].title + '-' + groupId].unshift(
            {
              key: `${groupId}-${item.equipNo}`,
              title: item.equipName,
              staNo: item.staNo,
              status: 1,
              isGroup: false,
              groupId: groupId,
              parentKey: groupId,
              procAdvice: item.procAdvice,
              relatedView: item.relatedView,
              isEquip: true
            }
          )

          let insertData = {
            key: `${groupId}-${item.equipNo}`,
            title: item.equipName,
            staNo: item.staNo,
            status: 1,
            isGroup: false,
            groupId: groupId,
            procAdvice: item.procAdvice,
            relatedView: item.relatedView,
            isEquip: true,
            parentKey: groupId,
          }
          if (this.nodeObject[groupId].children[0]) {
            this.insertBefore(insertData, this.nodeObject[groupId].children[0].key)
          } else {
            this.append(insertData, groupId)
          }

        })

        this.statisticsNum = Number(this.statisticsNum) + Number(data.length)

        this.allEquipNum = this.allEquipNum + Number(data.length)
        // this.recycling(groupId)
        // this.timeoutLoad(groupId)
        this.formateEquips(this.nodeObject[groupId].children,this.nodeObject[groupId])
       
        this.flatternEquips(window.top.storage[this.nodeObject[groupId].title + '-' + groupId])
        this.resetSelectNum()

        // this.expandAndTimeoutLoad(groupId)
      }
    },


    //  批量删除
    deleteEquips() {
      let equips = this.getChecked().equipWithGroupId
      let groupObject = {}
      // 按照设备分组归类提取设备
      for (let index = 0, length = equips.length; index < length; index++) {
        if (this.flatternEquipObject[equips[index]]) {
          const { groupId, isGroup, key, procAdvice, relatedView, staNo, status, title, count } = this.flatternEquipObject[equips[index]]
          if (!groupObject[groupId]) {
            groupObject[groupId] = {}
            groupObject[groupId].children = []
          }
          groupObject[groupId].children.push(
            {
              count: count,
              groupId: groupId,
              isGroup: isGroup,
              key: key,
              parentKey: groupId,
              parentTitle: this.nodeObject[groupId].title,
              procAdvice: procAdvice,
              relatedView: relatedView,
              staNo: staNo,
              status: status,
              title: title,
            }
          )
        }

      }
      for (let item in groupObject) {
        groupObject[item].children.forEach(childItem => {
          // 如果是搜索状态，则更新搜索状态下的缓存
          if (this.searchName) {
            let searchArr = window.top.storage[this.nodeObject[item].title + '-' + item + '-search']
            for (let index = 0, length = searchArr.length; index < length; index++) {
              if (childItem.key == searchArr[index].key) {
                searchArr.splice(index, 1)
                break;
              }
            }
          }
          // 更新非搜索状态下缓存
          let arr = window.top.storage[this.nodeObject[item].title + '-' + item]
          for (let index = 0, length = arr.length; index < length; index++) {
            if (childItem.key == arr[index].key) {
              arr.splice(index, 1)
              // 减掉所删除的设备数量
              this.statisticsNum = Number(this.statisticsNum) - 1
              break;
            }
          }

          // 从树形结构中移除设备
          this.remove(childItem.key)
        })
        // this.nodeObject[item].childFormate = false

        // this.resetSelectNum()

      }

      this.setCheckedKeys([],false,false)
      this.equipSelectsByGroup={}
      this.setSelectNum()

    },


    // 删除当前设备
    deleteCurrentEquip(groupName, groupNo, equipNo) {
      console.log(groupName, groupNo, equipNo,'groupName, groupNo, equipNo')
      this.setCheckedKeys([],false,false)
      this.equipSelectsByGroup={}
      this.setSelectNum()
      this.nodeObject[groupNo].childFormate = false
        let cacheGroupName = groupName + "-" + groupNo;
        if (window.top.storage[cacheGroupName]) {
          let arr = window.top.storage[cacheGroupName];
          for (let i = 0, length = arr.length; i < length; i++) {
            if (arr[i].key == this.originEquipMap[equipNo]) {
              arr.splice(i, 1);
              if (!this.searchName) {
                // 非搜索状态下删除
                this.nodeObject[groupNo].children.splice(i, 1)
                // this.time
              } else {
                // 搜索状态下删除
                let children = this.nodeObject[groupNo].children
                for (let index = 0, length = children.length; index < length; index++) {
                  if (children[index].key == this.originEquipMap[equipNo]) {
      
                    children.splice(index, 1)
                    break;
                  }
                }
              }
              break;
            }
          }
          this.$nextTick(()=>{
            this.timeoutLoad(groupNo)
          })
          // window.top.storage[cacheGroupName] = arr;
        }
        // 减掉删除的设备数量
        this.allEquipNum = this.allEquipNum - 1
        this.statisticsNum = this.statisticsNum - 1
      },
      
      // 更新当前设备名
      updateCurrentEquipName(groupId, groupName, equipNo, equipName) {
        let haveChange = false
        try {
          let equipKey = this.originEquipMap[equipNo]
          // 更新树形结构的设备名
          if (this.searchName) {
          
            // 搜索状态下
            if (this.searchEquipObject[equipKey].title != equipName) {
              haveChange = true
              this.searchEquipObject[equipKey].title = equipName
            }
          } else {
            // 非搜索状态下
            if (this.equipObject[equipKey].title != equipName) {
              haveChange = true
              this.equipObject[equipKey].title = equipName
            }
          }
        } catch (error) {
          console.log(error)
        }
      
        // 从缓存中更新设备名
        if (haveChange) {
          this.flatternEquipObject[groupId+'-'+equipNo].title = equipName
        }
      
      },
      
      
      
      //移动设备
      removeEqups(getKey) {
              let groupObject = {}
      
              let equips = this.getChecked().equipWithGroupId
              // 按照设备分组归类提取设备
              for (let index = 0, length = equips.length; index < length; index++) {
                if (this.flatternEquipObject[equips[index]]) {
                  const { isGroup, key, groupId, procAdvice, relatedView, staNo, status, title, count } = this.flatternEquipObject[equips[index]]
                  if (!groupObject[groupId]) {
                    groupObject[groupId] = {}
                    groupObject[groupId].children = []
                  }
                  groupObject[groupId].children.push(
                    {
                      count: count,
                      groupId: groupId,
                      isGroup: isGroup,
                      key: key,
                      parentKey: getKey,
                      parentTitle: this.nodeObject[getKey].title,
                      procAdvice: procAdvice,
                      relatedView: relatedView,
                      staNo: staNo,
                      status: status,
                      title: title,
                    }
                  )
                }
              }
      
              // 更新缓存
              for (let item in groupObject) {
                if (item != getKey) {
                  if (!window.top.storage[this.nodeObject[getKey].title + '-' + getKey]) {
                    window.top.storage[this.nodeObject[getKey].title + '-' + getKey] = []
                  }
                  if (this.searchName) {
                    if (!window.top.storage[this.nodeObject[getKey].title + '-' + getKey + '-search']) {
                      window.top.storage[this.nodeObject[getKey].title + '-' + getKey + '-search'] = []
                    }
                  }
                  // this.nodeObject[item].count = this.nodeObject[item].count - groupObject[item].children.length
                  groupObject[item].children.forEach(childItem => {
                    window.top.storage[this.nodeObject[getKey].title + '-' + getKey].unshift({
                      count: childItem.count,
                      groupId: getKey,
                      isGroup: childItem.isGroup,
                      key: childItem.key,
                      parentKey: getKey,
                      parentTitle: childItem.parentTitle,
                      procAdvice: childItem.procAdvice,
                      relatedView: childItem.relatedView,
                      staNo: childItem.staNo,
                      status: childItem.status,
                      title: childItem.title,
                      isEquip: true
                    })
      
                    if (this.searchName) {
                      window.top.storage[this.nodeObject[getKey].title + '-' + getKey + '-search'].unshift({
                        count: childItem.count,
                        groupId: getKey,
                        isGroup: childItem.isGroup,
                        key: childItem.key,
                        parentKey: getKey,
                        parentTitle: childItem.parentTitle,
                        procAdvice: childItem.procAdvice,
                        relatedView: childItem.relatedView,
                        staNo: childItem.staNo,
                        status: childItem.status,
                        title: childItem.title,
                        isEquip: true
                      })
                    }
      
                    let arr = window.top.storage[this.nodeObject[item].title + '-' + item]
                    for (let i = 0, length = arr.length; i < length; i++) {
                      if (arr[i].key == childItem.key) {
                        arr.splice(i, 1)
                        break;
                      }
                    }
                    if (this.searchName) {
                      let arr = window.top.storage[this.nodeObject[item].title + '-' + item + '-search']
                      for (let i = 0, length = arr.length; i < length; i++) {
                        if (arr[i].key == childItem.key) {
                          arr.splice(i, 1)
                          break;
                        }
                      }
                    }
                    this.remove(childItem.key)
                    let insertData = {
                      count: childItem.count,
                      groupId: getKey,
                      isGroup: childItem.isGroup,
                      key: childItem.key,
                      parentKey: childItem.parentKey,
                      parentTitle: childItem.parentTitle,
                      procAdvice: childItem.procAdvice,
                      relatedView: childItem.relatedView,
                      staNo: childItem.staNo,
                      status: childItem.status,
                      title: childItem.title,
                      isEquip: true
                    }
                    if (this.nodeObject[getKey].children[0]) {
                      this.insertBefore(insertData, this.nodeObject[getKey].children[0].key)
                    } else {
                      this.append(insertData, getKey)
                    }
      
                    this.flatternEquips([{
                      count: childItem.count,
                      groupId: getKey,
                      isGroup: childItem.isGroup,
                      key: childItem.key,
                      parentKey: childItem.parentKey,
                      parentTitle: childItem.parentTitle,
                      procAdvice: childItem.procAdvice,
                      relatedView: childItem.relatedView,
                      staNo: childItem.staNo,
                      status: childItem.status,
                      title: childItem.title,
                      isEquip: true
                    }])
      
                  })
                  this.nodeObject[item].childFormate = false
                }
      
              }
      
              // 重新计算分组设备数量
              this.recomputedGroupCount()
              // 重置选择的节点数量
              this.resetSelectNum()
      
      },

         // 重新计算分组数量
    recomputedGroupCount() {
      for (let item in this.nodeObject) {
        this.nodeObject[item].count = 0
      }

      // 搜索状态下
      if (this.searchName) {
        for (let item in this.nodeObject) {
          if (window.top.storage[this.nodeObject[item].title + '-' + item + '-search']) {
            let equipNum = window.top.storage[this.nodeObject[item].title + '-' + item + '-search'].length
            this.setGroupNum(item, equipNum)
          }

        }
      } else {  //非搜索状态下
        for (let item in this.nodeObject) {
          if (window.top.storage[this.nodeObject[item].title + '-' + item]) {
            let equipNum = window.top.storage[this.nodeObject[item].title + '-' + item].length
            this.setGroupNum(item, equipNum)
          }

        }
      }

    },
      
  }
}