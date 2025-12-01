export default{
    methods:{
    // key(Number):节点key值
      timeoutLoad(key) {
        let data;
        if (this.searchName) {
          data = this.deepClone(window.top.storage[this.nodeObject[key].title + '-' + key + '-search'])
        } else {
          data = this.deepClone(window.top.storage[this.nodeObject[key].title + '-' + key])
        }

        if (data) {
          // let group = this.nodeObject[key].children.filter(item => item.isGroup && !item.isEquip)
          this.nodeObject[key].loading = true
          let timeout = setTimeout(() => {
            data.forEach(item => {
              item.isEquip = true;
              if (this.needsSettings) {
                item.isGroup = true; item.groupUpdate = false; item.children = []
              }
            })

            // this.nodeObject[key].children = [].concat(data).concat(group)
            this.nodeObject[key].groupUpdate = true

            // 判断是否已经扁平化
            if (!this.nodeObject[key].childFormate) {
              this.formateEquips(data, this.nodeObject[key])
              this.nodeObject[key].childFormate = true
            }

            if (data.length > 3500) {
              this.expanding = true
              let advanceLoadData = data.slice(0, 3500)
              let delayLoadData = data.splice(3500, data.length)
              // 更新到树形结构  
              this.updateWithEquips(key, [...advanceLoadData])
                      // 设置为当前选中分组节点
              this.setExpand(key)


              if (delayLoadData.length > 0) {
                // 如果设备过多，则分次加载
                this.otherLoad(advanceLoadData[advanceLoadData.length - 1].key, delayLoadData, key)
              }else{
                this.expanding = false
                this.nodeObject[key].loading = false
                //   更新设备选中状态
                if (this.showCheckbox) {
                  this.refreshCheckStatus(key)
                }
              
              }
            } else {
                // 更新到树形结构  
                this.updateWithEquips(key, [...data])
                // 设置为当前选中分组节点
                this.setExpand(key)

              //   更新设备选中状态
              if (this.showCheckbox) {
                this.refreshCheckStatus(key)
              }
              this.nodeObject[key].loading = false
              this.$forceUpdate()
            }
            clearTimeout(timeout)
            timeout = null
          }, 200)
        } else {
          this.nodeObject[key].loading = false
        }

      },


      // 设备量多，分段加载
      otherLoad(key, data, parentKey) {
        let insertData = data.slice(0, 2000)
        let otherData = data.slice(2000, data.length)
        // 记录前一个节点key
        let preKey = ''
        let time = setTimeout(() => {
          insertData.forEach(item => {
            // 数据填充在前一个节点后面
            this.insertAfter(item, preKey || key)
            preKey = item.key
          })
          this.$nextTick(() => {
            if (otherData.length > 0) {
              this.otherLoad(preKey, otherData, parentKey)
            } else {
              // 更新选中状态
              if (this.showCheckbox) {
                this.refreshCheckStatus(parentKey)
              }

              this.nodeObject[parentKey].loading = false

              // 设置当前展开分组
              this.setExpand(parentKey)
              this.expanding = false

            }
          })
          clearTimeout(time)
          time = null
        }, 200)

      },
    }
}