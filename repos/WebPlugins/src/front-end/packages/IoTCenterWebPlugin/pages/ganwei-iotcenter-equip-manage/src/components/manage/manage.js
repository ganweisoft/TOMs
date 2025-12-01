const ycYxSet = () => import('../layout/ycYxSet/index.vue')
const equipDialog = () => import('../dialog/equip/index.vue')
const deleteEquip = () => import('../dialog/deleteEquip/index.vue')
export default {
    components: { ycYxSet, equipDialog, deleteEquip },
    props: {
        selecteItem: {
            type: Object,
            default: () => {}
        }
    },
    provide() {
        return {
            getWaveFilenames: () => this.waveFilenames,
            getPlanNoDropdown: () => this.planNoDropdown,
            getVideoDropdown: () => this.videoDropdown,
            getCommunicationDrvList: () => this.communicationDrvList
        }
    },
    watch: {},
    computed: {},
    data() {
        return {
            waveFilenames: [],
            videoDropdown: [],
            communicationDrvList: [],
            planNoDropdown: [],
            copyObject: {},
            batchEquip: true,
            saveLoading: false,
            isEquipNew: false,
            showEquipDialog: false,
            showDeleteEquipDialog: false,
            equipInfo: {
                parameters: [],
                enableEquip: false,
                equipNo: '',
                equipNm: '',
                equipDetail: '',
                ziChanId: '',
                ziChanName: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                procAdvice: '',
                rawEquipNo: '',
                tabName: '',
                staIp: '',
                attrib: '',
                accCyc: 1,
                communicationDrv: '',
                communicationParam: '',
                communicationTimeParam: '',
                contacted: '',
                outOfContact: '',
                alarmScheme: 0,
                alarmRiseCycle: '',
                eventWav: '',
                safeTime: '',
                localAddr: '',
                equipAddr: '',
                staNo: 1,
                backup: '',
                groupId: ''
            }
        }
    },

    mounted() {},

    methods: {
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getwaveFilenames() {
            this.$api.getwaveFilenames().then(res => {
                const { code, data } = res?.data || {}
                if (code == 200) {
                    if (data && data instanceof Array) {
                        data.forEach(item => {
                            this.waveFilenames.push({
                                label: item,
                                value: item
                            })
                        })
                    }
                }
            })
        },
        closeDialog(type) {
            this[type] = false
            this.batchEquip = true
        },
        // 获取预案号
        getPlanList() {
            let data = {
                pageNo: 1,
                pageSize: 100,
                planName: ''
            }
            this.$api
                .getPlanList(data)
                .then(res => {
                    const { code, data } = res?.data || {}
                    if (code == 200) {
                        let planList = data?.rows || []
                        planList.forEach(planItem => {
                            if (planItem.planStatus == 1) {
                                const { planName, planId } = planItem
                                this.planNoDropdown.push({
                                    label: planName,
                                    value: planId.toString()
                                })
                            }
                        })
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                })
        },
        // 保存设备信息
        saveEquip(equipData) {
            this.saveLoading = true
            delete equipData.groupId
            this.$api
                .postUpdateEquip(JSON.stringify(equipData))
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code == 200) {
                        this.$message.success(this.$tt('publics.tips.editSuccess'))
                        this.batchEquip = false
                        this.selecteItem.equipName = equipData.equipNm
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(er => {
                    this.$message.error(er?.data, er)
                })
                .finally(() => {
                    this.saveLoading = false
                })
        },
        getEditEquipDetail() {
            let obj = {}
            obj.equipNos = [this.selecteItem.equipNo]
            this.isEquipNew = false
            // 获取设备列表
            this.$api
                .getEquipTree(obj)
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        let rows = data?.rows || []
                        rows.forEach(item => {
                            item.staNo = item.staN
                            if (item.relatedVideo) {
                                item.relatedVideo = parseInt(item.relatedVideo.split('|')[1])
                            } else {
                                item.relatedVideo = item.relatedVideo
                            }
                        })

                        this.copyObject = JSON.parse(JSON.stringify(this.equipInfo))
                        for (let i in this.copyObject) {
                            if (rows[0][i]) {
                                this.copyObject[i] = rows[0][i]
                            }
                        }
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                })
                .finally(() => {
                    this.equipLoading = false
                })
        },
        confirmDeleteEquip() {
            this.saveLoading = true
            let equipNo = this.selecteItem.equipNo
            this.$api
                .deleteEquip({
                    staNo: this.selecteItem.staNo,
                    equipNo: equipNo
                })
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message.success(this.$tt('publics.tips.deleteSuccess'))
                        this.showDeleteEquipDialog = false
                        window.parent.postMessage({ deleteEquip: true }, '*')
                        this.selecteItem = { equipNo: 0 }
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                })
                .finally(() => {
                    this.saveLoading = false
                })
        },
        // 新增设备--设备删除
        deleteEquip() {
            this.showDeleteEquipDialog = true
        },
        // 编辑设备框
        editEquip() {
            this.getEditEquipDetail()

            // //  获取驱动文件
            this.getCommunicationDrv()

            // 获取可关联视频列表
            this.getVideoList()

            // 给报警显示设置赋值

            this.isEquipNew = false
            this.showEquipDialog = true
        },
        //  获取动态库文件
        getCommunicationDrv() {
            this.$api
                .getCommunicationDrv()
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.communicationDrvList = data || []
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)
                })
        },
        // 获取视频可关联项
        getVideoList() {
            this.$api
                .getVideoAllInfo({ equip_no: 100 })
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code === 200) {
                        this.videoDropdown = data || []
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err.data, err)
                    console.log(err)
                })
        }
    }
}
