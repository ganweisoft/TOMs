import api from 'gw-base-components-plus/gwtree/src/request/api.js'
export default{
    methods:{
        // 加载设备节点
        clickLoadEquip(item) {
        // 如果分组下面没有设备
        let equips = this.nodeObject[item.key].children.filter(childItem => !childItem.isGroup)
        if (equips.length == 0 && !this.nodeObject[item.key].loading && !item.groupUpdate) {
          // 从后台请求数据
          let query = {SearchName: null, GroupId: null}
          query.GroupId = item.key
          this.nodeObject[item.key].loading = true
          api.getEquip(query).then(res => {
            if (res.data.code == 200) {
              try {
                // 如果当前分组还没加载，则更新设备信息到树形结构
                if (!item.groupUpdate) {
                  let dataArr = res.data.data.filter(item => !item.isGroup)
                  this.updateEquips(item.key, dataArr, false, true)
                }
              } catch (error) {
                console.log(error)
              }

              let timeout = setTimeout(() => {
                this.nodeObject[item.key].loading = false
                this.$forceUpdate()
                clearTimeout(timeout)
                timeout = null
              }, 100)

            } else {
              this.nodeObject[item.key].loading = false
            }

          }).catch(err => {
            this.nodeObject[item.key].loading = false
          })
        }
      },

       //   增加设置控制项
       loadSetting(key, checked) {
        if (!this.nodeObject[key].loadSetting) {
          this.nodeObject[key].loading = true
          api.getSetParm({ equipNo: Number(key.split('-')[1]) }).then(rt => {
            let objectChildren = []
            if (rt.data.code == 200 && rt.data.data) {
              this.nodeObject[key].loadSetting = true
              let data = JSON.parse(rt.data.data.list);
    
              data.forEach(childItem => {
                objectChildren.push({
                  title: childItem.setNm,
                  key: childItem.equipNo.toString() + '-' + childItem.setNo,
                  staNo: childItem.staN,
                  value: childItem.equipNo + '.' + childItem.setNo,
                  equipNo: childItem.equipNo,
                  setType: childItem.setType,
                  isSetting: true,
                  setValue: childItem.value,
                  setNo: childItem.setNo,
                  equipName: this.flatternEquipObject[key].title
                })
    
              })
    
              this.nodeObject[key].children = JSON.parse(JSON.stringify(objectChildren))
    
              this.updateKeyChildren(key, objectChildren)
              // 记录所有已展开设备号
              this.isExpendEquip[key] = key
    
            }
            this.nodeObject[key].loading = false
    
    
            this.$nextTick(() => {
              if (checked) {
                // 如果当前设备节点是全选，则将控制项全部全选
                this.setChecked(key, true, true)
              }
              // 如果记录设备对应控制项中找到对应的记录，则设置对应的控制项选中状态
              if (this.controllObject[key]) {
                objectChildren.forEach(childItem => {
                  let list = this.controllObject[key].list;
                  if (list.includes(Number(childItem.setNo))) {
                    this.setChecked(childItem.equipNo + '-' + childItem.setNo, true, true)
                    //    this.store.nodesMap[].checked=true
                  }
                })
              }
            })
          }).catch(err => {
            this.nodeObject[key].loading = false
          })
        }
    
      },
    }
}