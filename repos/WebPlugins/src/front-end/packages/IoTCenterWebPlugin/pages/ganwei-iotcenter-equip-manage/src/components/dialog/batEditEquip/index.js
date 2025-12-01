import ziChanSelect from '../../UI/ziChanSelect/index.vue'
import customAttribute from '../../customAttribute/index.vue'
export default {
    components: { ziChanSelect, customAttribute },
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        saveData: {
            type: Function,
            default: () => { }
        },
        saveLoading: {
            type: Boolean,
            default: false
        }
    },
    watch: {
        showDialog: function (val) {
            this.showBatEditEquipDialog = val
            if (!val) {
                this.batchEquipSelect.selectVal = []
                this.alarms.emailAlarm = 0
                this.alarms.messageAlarm = 0
                this.alarms.isMarkAlarm = 0
                this.alarms.isAlarm = 0
            } else {
                this.batchEquipForm = JSON.parse(JSON.stringify(this.formData))
            }
        },
        'batchEquipSelect.selectVal': function (newVal, oldVal) {
            if (newVal.includes('parameters') && !oldVal.includes('parameters')) {
                this.getCustomPropData = {
                    ids: this.getSelected(),
                    operateEnum: 0
                }
            }
        }
    },
    data () {
        return {
            showBatEditEquipDialog: false,

            // 批量修改设备
            batchEquipSelect: {
                selectVal: []
            },

            equipSelectList: [
                { label: 'equipInfo.equip.DeviceProperties', value: 'equipDetail' },
                { label: 'equipInfo.equip.LinkageVideo', value: 'relatedVideo' },
                { label: 'equipInfo.equip.NameOfAsset', value: 'ziChanId' },
                { label: 'equipInfo.equip.PlanNumber', value: 'planNo' },
                { label: 'equipInfo.equip.LinkagePage', value: 'relatedPic' },
                { label: 'equipInfo.dialog.alarm', value: 'alarmScheme' },
                { label: 'equipInfo.equip.HotStandby', value: 'backup' },
                { label: 'equipInfo.equip.CommunicationRefreshPeriod', value: 'accCyc' },
                { label: 'equipInfo.equip.CommunicationFaultHandlingSuggestion', value: 'procAdvice' },
                { label: 'equipInfo.equip.BreakdownInfo', value: 'outOfContact' },
                { label: 'equipInfo.equip.FailoverPrompt', value: 'contacted' },
                { label: 'equipInfo.equip.AlarmVoiceFile', value: 'eventWav' },
                { label: 'equipInfo.equip.DriverFile', value: 'communicationDrv' },
                { label: 'equipInfo.equip.CommunicationPort', value: 'localAddr' },
                { label: 'equipInfo.equip.AddressOfDevice', value: 'equipAddr' },
                { label: 'equipInfo.equip.ParameterOfCommunication', value: 'communicationParam' },
                { label: 'equipInfo.equip.TimeParameterOfCommunication', value: 'communicationTimeParam' },
                { label: 'equipInfo.equip.AlarmUpgradePeriod', value: 'alarmRiseCycle' },
                { label: 'equipInfo.equip.SafetyPeriod', value: 'safeTime' },
                { label: 'equipInfo.equip.NameOfAttachedList', value: 'tabname' },
                { label: 'equipInfo.equip.equipStartStop', value: 'enableEquip' },
                { label: 'equipInfo.common.customAttribute', value: 'parameters' },
            ],
            formData: {
                enableEquip: false,
                equipDetail: '',
                ziChanId: '',
                planNo: '',
                backup: '',
                relatedPic: '',
                relatedVideo: '',
                procAdvice: '',
                rawEquipNo: '',
                tabname: '',
                staIp: '',
                attrib: '',
                accCyc: '',
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
                gatewayId: '',
                serialNumber: '',
                model: '',
                deviceType: '',
                mac: '',
                hwVersion: '',
                swVersion: '',
                fwVersion: '',
                protocolType: '',
                sigVersion: '',
                manufacturerId: '',
                manufacturerName: '',
                measure: 1,
                reportData: '',
                interval: '',
                mute: 0,
                location: '',
                latitude: '',
                longitude: '',
                height: '',
                parameters: []
            },
            batchEquipForm: {},
            alarms: {
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            labelEquip: 100,
            minNum: 0,
            maxNum: 2147483647,

            getCustomPropData: {
                ids: [],
                operateEnum: 0
            }
        }
    },
    inject: ['getSelected', 'swit', 'alarmDropdown', 'getZcDropdown', 'getVideoDropdown', 'getCommunicationDrvList', 'getEquipList', 'haveChangeItem', 'getPlanNoDropdown', 'getWaveFilenames'],
    computed: {
        waveFilenames () {
            return this.getWaveFilenames()
        },
        rules () {
            return {
                selectVal: [{ type: 'array', required: true, message: this.$t('equipInfo.tips.noSelectItem') }]
            }
        },
        zcDropdown () {
            return this.getZcDropdown()
        },
        videoDropdown () {
            return this.getVideoDropdown()
        },
        planNoDropdown () {
            return this.getPlanNoDropdown()
        },
        communicationDrvList () {
            return this.getCommunicationDrvList()
        },
        equipList () {
            return this.getEquipList()
        }
    },
    methods: {
        $tt (string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getPlaceholder (label, type) {
            if (type == 'select') {
                return this.$t('equipInfo.tips.select') + this.$t(label)
            }
            return this.$t('equipInfo.tips.input') + this.$t(label)
        },
        toInteger (item) {
            let reg = /^[0-9]+$/
            if (this.batchEquipForm[item]) {
                if (!reg.test(this.batchEquipForm[item])) {
                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.batchEquipForm[item] = parseInt(this.batchEquipForm[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.batchEquipForm[item] = 0
                })
            }
        },

        filterOtherWords (value) {
            let reg = /[\-\,\!\|\~\`\(\)\#\$\%\^\&\*\{\}\:\;\"\L\<\>\?\]\[\.\@\￥\\\/\·\！\+\-\…\】\【\、]/g
            return value.replaceAll(reg, '').trim()
        },
        // 保存批量修改设备
        saveBatchEquip () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    if (this.batchEquipForm.tabname) {
                        this.batchEquipForm.tabname = this.filterOtherWords(this.batchEquipForm.tabname)
                    }
                    let data = this.haveChangeItem(this.batchEquipSelect.selectVal, this.batchEquipForm, this.alarms, this.labelEquip)
                    if (!data) return
                    this.saveData(data, 'batchEditEquip', 'showBatEditEquipDialog')
                }
            })
        },
        resetAndClose () {
            this.$nextTick(() => {
                this.$refs.form.clearValidate()
            })
            this.closeDialog('showBatEditEquipDialog')
        }
    }
}
