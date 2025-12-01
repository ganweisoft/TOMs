import * as signalR from '@aspnet/signalr'

export default{
    methods:{
           // 计算设备报警数量以及双机热备设备数量和设置分组状态
     resolveData(key) {
      if(key){
        let list = []
        if (this.searchName) {
          list = window.top.storage[`${this.nodeObject[key].title}-${key}-search`]
        } else {
          list = window.top.storage[`${this.nodeObject[key].title}-${key}`]
        }
        if(list&&list.length>0){
          for (let i = 0, length = list.length; i < length; i++) {
            if (list[i].status == 2 || list[i].status == 0) {
              if (!this.alarmList.includes(list[i].key)) {
                this.alarmList.push(list[i].key)
                this.setGroupStatus(key, true, 2)
              }
        
            } else if (list[i].status == 6) {
              if (!this.backupList.includes(list[i].key)) {
                this.backupList.push(list[i].key)
                this.setGroupStatus(key, true, 6)
              }
            }
          }
        }
      }else{
        Object.defineProperty(this.flatternEquipObject, 'key', {
          enumerable: false
        })
       let keys = Object.getOwnPropertyNames(this.flatternEquipObject)
       for(let index=0,length=keys.length;index<length;index++){
        if(this.flatternEquipObject[keys[index]]){
          let status=this.flatternEquipObject[keys[index]].status
            if(status!=null&&status!=undefined){
              if(status==2||status==0){
                this.alarmList.push(keys[index])
                this.setGroupStatus(this.flatternEquipObject[keys[index]].groupId, true, 2)
              }else if(status==6){
                this.backupList.push(keys[index])
                this.setGroupStatus(this.flatternEquipObject[keys[index]].groupId, true, 6)
              }
            }
        }
       }
      }

      // this.$forceUpdate()
    },
    
      // 设置分组状态 key:扁平化数据中节点索引；type:类型（增加，减少）；status:状态（报警2、双机热备6）
      setGroupStatus(key, type, status) {
        if (type) {
          if (status == 2) {
            this.nodeObject[key].alarmCounts = this.nodeObject[key].alarmCounts + 1
            if (this.nodeObject[key].alarmCounts > 0) {
              this.nodeObject[key].status = 2
            }
          } else {
            this.nodeObject[key].backUpCounts = this.nodeObject[key].backUpCounts + 1
            if (this.nodeObject[key].alarmCounts == 0 && this.nodeObject[key].backUpCounts > 0) {
              this.nodeObject[key].status = 6
            }
          }

        } else {
          if (status == 2) {
            this.nodeObject[key].alarmCounts = this.nodeObject[key].alarmCounts - 1
            if (this.nodeObject[key].alarmCounts == 0) {
              this.nodeObject[key].status = 1
            }
          } else {
            this.nodeObject[key].backUpCounts = this.nodeObject[key].backUpCounts - 1
            if (this.nodeObject[key].alarmCounts == 0 && this.nodeObject[key].backUpCounts == 0) {
              this.nodeObject[key].status = 1
            }
          }

        }
        if (this.nodeObject[key].parentKey) {
          this.setGroupStatus(this.nodeObject[key].parentKey, type, status)
        }

      },

      // 设备数据推送
      connectHubEquipState(equipNo) {
        if (this.signalrConnectionEquip) {
          this.signalrConnectionEquip.stop()
          this.signalrConnectionEquip = null;
        }

        this.signalrConnectionEquip = new signalR.HubConnectionBuilder()
          .withUrl(this.$api.getSignalrHttp() + '/monitor', {})
          .build()

        this.signalrConnectionEquip.serverTimeoutInMilliseconds = 500000000;
        this.signalrConnectionEquip.keepaliveintervalinmilliseconds = 500000000

        let that = this

        // 开始连接this.signalrConnectionr
        that.signalrConnectionEquip
          .start()
          .then(() => {
            that.signalrConnectionEquip.invoke('OnConnect', equipNo)
          })
          .catch(function (ex) {
            console.log('connectHub 连接失败' + ex)
          })
        that.signalrConnectionEquip.off('EquipDataChanged')
        that.signalrConnectionEquip.on('EquipDataChanged', (res) => {

          let data = JSON.parse(res)
          if (this.aleadyLoadAll) {
            this.setStatus(data)
          } else {
            this.statusList = [...data, ...this.statusList]
          }
        })

        that.signalrConnectionEquip.onclose(async (error) => {
          let time = setTimeout(() => {
            this.connectHubEquipState(equipNo)
            clearTimeout(time)
            time = null
          }, 2000)
        })
      },


      setStatus(data) {
        for (const item of data) {
          let equipNo = this.originEquipMap[item.equipNo]
          let key = this.flatternEquipObject[equipNo].groupId;
          let status = this.flatternEquipObject[equipNo].status
          if (
            (item.equipState == 2 && status != 2) || (item.equipState == 0 && status != 0)
          ) {
            if (key) {
              this.setGroupStatus(key, true, 2) // 加一
            }
          } else if (
            (status == 2 &&
              item.equipState != 2) || (status == 0 &&
                item.equipState != 0)
          ) {
            if (key) {
              this.setGroupStatus(key, false, 2) // 减一
            }
          } else if (item.equipState == 6 && status != 6) {
            this.setGroupStatus(key, true, 6) // 加一
          } else if (status == 6 &&
            item.equipState != 6) {
            this.setGroupStatus(key, false, 6) // 减一
          }

          this.flatternEquipObject[equipNo].status = item.equipState
          if (this.searchName) {
            if (this.searchEquipObject[equipNo]) {
              this.searchEquipObject[equipNo].equipState = item.equipState
              this.searchEquipObject[equipNo].status = item.equipState
            }

          } else {
            if (this.equipObject[equipNo]) {
              this.equipObject[equipNo].equipState = item.equipState
              this.equipObject[equipNo].status = item.equipState
            }
          }


          this.$emit('statusChange', item.equipState, item.equipNo)

          if (item.status == 2) {
            let alarmIndex = this.alarmList.indexOf(equipNo)
            let backupIndex = this.backupList.indexOf(equipNo)
            if (alarmIndex > -1) {
              this.alarmList.splice(index, 1)
            }
            if (backupIndex > -1) {
              this.backupList.splice(index, 1)
            }
          }
        }
      },
    }
}