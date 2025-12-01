/*
 * @Author: Copyright (c) 2020-2025 Shenzhen Ganwei Software Technology Co., Ltd
 * @Date: 2022-08-16 13:14:43
 * @Description: 这是默认设置,请设置`customMade`, 打开koroFileHeader查看配置 进行设置:
 */
export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
    },
    data() {
        return {
            exportType:3,
            exportLoading:false
        }
    },
    computed: {
        typeList() {
            return [
                {
                    name: 'equipListsIot.excelName.allDevice',
                    value: 3
                },
                {
                    name: 'equipListsIot.excelName.normalDevice',
                    value: 1
                },
                {
                    name: 'equipListsIot.excelName.alarmDevice',
                    value: 2
                },
                {
                    name: 'equipListsIot.excelName.outLineDevice',
                    value: 0
                }
            ]
        },
    },
    methods: {

        confirmExport() {
            this.exportLoading = true
            this.$api
                .exportAbnormalRecord({
                    deviceStatus: this.exportType == 3 ? -1 : this.exportType
                })
                .then((res) => {
                    this.exportLoading = false
                    if (res.status === 200) {
                        let url = window.URL.createObjectURL(new Blob([res.data], { type: res.data.type }))
                        let link = document.createElement('a')
                        link.style.display = 'none'
                        link.href = url
                        let date = new Date()
                        let dateStr = `${date.getFullYear()}-${date.getMonth() + 1
                            }-${date.getDate()}`
                        let excelName = ''
                        switch (this.exportType) {
                            case 0:
                                excelName = this.$t('equipListsIot.excelName.outLineDevice')
                                break // 离线
                            case 1:
                                excelName = this.$t('equipListsIot.excelName.normalDevice')
                                break // 正常
                            case 2:
                                excelName = this.$t('equipListsIot.excelName.alarmDevice')
                                break // 告警
                            case 3:
                                excelName = this.$t('equipListsIot.excelName.allDevice')
                                break // 全部
                            default:
                                break
                        }
                        link.setAttribute('download', dateStr + '-' + excelName)
                        document.body.appendChild(link)
                        link.click()
                        this.closeDialog()
                    } else {
                        this.$message.error(this.$t('equipListsIot.publics.tips.exportFail'), res)
                    }
                })
                .catch((err) => {
                    console.log(err);
                    this.exportLoading = false
                    let reader = new FileReader()
                    reader.readAsText(err.data, 'utf-8')
                    reader.onload =  (e)=> {
                        let content = JSON.parse(reader.result)
                        this.$message.error(content.message, err)
                    }
                })
        },

        closeDialog(){
            this.$emit('closeDialog','showExportEquipDialog')
        }

    }
}