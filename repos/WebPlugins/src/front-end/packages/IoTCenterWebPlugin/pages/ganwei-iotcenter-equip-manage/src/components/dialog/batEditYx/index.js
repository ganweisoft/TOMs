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
            this.showBatEditYxDialog = val
            if (!val) {
                this.batchYxSelect.selectVal = []
                this.alarms.emailAlarm = 0
                this.alarms.messageAlarm = 0
                this.alarms.isMarkAlarm = 0
                this.alarms.isAlarm = 0
            } else {
                this.batchYxForm = JSON.parse(JSON.stringify(this.formData))
            }
        },
        'batchYxSelect.selectVal': function (newVal, oldVal) {
            if (newVal.includes('parameters') && !oldVal.includes('parameters')) {
                this.getCustomPropData = {
                    ids: this.getSelected(),
                    operateEnum: 2
                }
            }
        }
    },
    data () {
        return {
            showBatEditYxDialog: false,
            dataTypeOption: [
                {
                    label: 'equipInfo.dataType.bool',
                    value: null
                },
                {
                    label: 'equipInfo.dataType.string',
                    value: 'S'
                }
            ],

            // 批量修改yx
            batchYxSelect: {
                selectVal: []
            },

            yxSelectList: [
                { label: 'equipInfo.dataType.title', value: 'dataType' },
                { label: 'equipInfo.yx.zeroToOneEcent', value: 'evt01' },
                { label: 'equipInfo.yx.oneToZeroEvent', value: 'evt10' },
                { label: 'equipInfo.yx.LinkagePage', value: 'relatedPic' },
                { label: 'equipInfo.yx.LinkageVideo', value: 'relatedVideo' },
                { label: 'equipInfo.yx.NameOfAsset', value: 'ziChanId' },
                { label: 'equipInfo.yx.PlanNumber', value: 'planNo' },
                { label: 'equipInfo.yx.CurveRecord', value: 'curveRcd' },
                { label: 'equipInfo.dialog.alarm', value: 'alarmScheme' },
                { label: 'equipInfo.yx.ReverseOrNot', value: 'inversion' },
                { label: 'equipInfo.yx.SuggestionForZeroOne', value: 'procAdviceR' },
                { label: 'equipInfo.yx.SuggestionForOneZero', value: 'procAdviceD' },
                { label: 'equipInfo.yx.levelOfZeroOne', value: 'levelR' },
                { label: 'equipInfo.yx.levelOfOneZero', value: 'levelD' },
                { label: 'equipInfo.yx.OperatingCommand', value: 'mainInstruction' },
                { label: 'equipInfo.yx.OperatingParameter', value: 'minorInstruction' },
                { label: 'equipInfo.yx.alarmDelayTime', value: 'alarmAcceptableTime' },
                { label: 'equipInfo.yx.RestoreDelayTime', value: 'restoreAcceptableTime' },
                { label: 'equipInfo.yx.RepeatedAlarmTime', value: 'alarmRepeatTime' },
                { label: 'equipInfo.yx.AlarmBlock', value: 'alarmShield' },
                { label: 'equipInfo.yx.AlarmUpgradePeriod', value: 'alarmRiseCycle' },
                { label: 'equipInfo.yx.SafetyPeriod', value: 'safeTime' },
                { label: 'equipInfo.common.expression', value: 'expression' },
                { label: 'equipInfo.common.customAttribute', value: 'parameters' },
            ],
            formData: {
                expression: '',
                ziChanId: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                valTrait: 0,
                procAdviceR: '',
                procAdviceD: '',
                curveRcd: false,
                levelR: 2,
                levelD: 3,
                evt01: '',
                evt10: '',
                mainInstruction: '',
                minorInstruction: '',
                inversion: false,
                initval: 0,
                safeBgn: '',
                safeEnd: '',
                safeTime: '',
                alarmAcceptableTime: '',
                alarmRepeatTime: '',
                restoreAcceptableTime: '',
                alarmScheme: '',
                waveFile: '',
                alarmRiseCycle: '',
                alarmShield: '',
                dataType: null,
                parameters: []
            },
            batchYxForm: {},
            alarms: {
                isGenerateWO: 0,
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            labelEquip: 100,
            minNum: 0,
            maxNum: 2147483647,
            expressionTips: '',

            getCustomPropData: {
                ids: [],
                operateEnum: 2
            }
        }
    },
    inject: ['getSelected', 'swit', 'swit1', 'alarmDropdown', 'getVideoDropdown', 'getZcDropdown', 'haveChangeItem', 'getPlanNoDropdown', 'getWaveFilenames'],
    computed: {
        waveFilenames () {
            return this.getWaveFilenames()
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
        rules () {
            return {
                selectVal: [{ type: 'array', required: true, message: this.$t('equipInfo.tips.select') + this.$t('equipInfo.yx.YxParameters') }]
            }
        },
    },
    mounted () {
        setTimeout(() => {
            this.expressionTips = this.$t('equipInfo.common.expressionTips')
        }, 1000)
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
            if (this.batchYxForm[item]) {
                if (!reg.test(this.batchYxForm[item])) {
                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.batchYxForm[item] = parseInt(this.batchYxForm[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.batchYxForm[item] = 0
                })
            }
        },

        // 保存批量修改yx
        saveBatchYx () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    let data = this.haveChangeItem(this.batchYxSelect.selectVal, this.batchYxForm, this.alarms, this.labelEquip, true)
                    if (!data) return
                    this.saveData(data, 'batchEditYx', 'showBatEditYxDialog', true)
                }
            })
        },
        resetAndClose () {
            this.$nextTick(() => {
                this.$refs.form.clearValidate()
            })
            this.closeDialog('showBatEditYxDialog')
        }
    }
}
