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
            this.showBatEditYcDialog = val
            if (!val) {
                this.batchYcSelect.selectVal = []
                this.alarms.emailAlarm = 0
                this.alarms.messageAlarm = 0
                this.alarms.isMarkAlarm = 0
                this.alarms.isAlarm = 0
            } else {
                this.batchYcForm = JSON.parse(JSON.stringify(this.formData))
            }
        },
        'batchYcSelect.selectVal': function (newVal, oldVal) {
            if (newVal.includes('parameters') && !oldVal.includes('parameters')) {
                this.getCustomPropData = {
                    ids: this.getSelected(),
                    operateEnum: 1
                }
            }
        }
    },

    data () {
        return {
            showBatEditYcDialog: false,
            dataTypeOption: [
                {
                    label: 'equipInfo.dataType.double',
                    value: null
                },
                {
                    label: 'equipInfo.dataType.string',
                    value: 'S'
                },
                {
                    label: 'equipInfo.dataType.twoTuples',
                    value: 'SEQ2'
                },
                {
                    label: 'equipInfo.dataType.threeTuples',
                    value: 'SEQ3'
                },
                {
                    label: 'equipInfo.dataType.fourTuples',
                    value: 'SEQ4'
                },
                {
                    label: 'equipInfo.dataType.fiveTuples',
                    value: 'SEQ5'
                },
                {
                    label: 'equipInfo.dataType.sixTuples',
                    value: 'SEQ6'
                },
                {
                    label: 'equipInfo.dataType.sevenTuples',
                    value: 'SEQ7'
                }
            ],

            // 批量修改yc
            batchYcSelect: {
                selectVal: []
            },

            ycSelectList: [
                { label: 'equipInfo.dataType.title', value: 'dataType' },
                { label: 'equipInfo.yc.LowerLimitingValue', value: 'valMin' },
                { label: 'equipInfo.yc.ReplyLowerLimitingValue', value: 'restoreMin' },
                { label: 'equipInfo.yc.ReplyUpperLimitValue', value: 'restoreMax' },
                { label: 'equipInfo.yc.UpperLimitValue', value: 'valMax' },
                { label: 'equipInfo.yc.Unit', value: 'unit' },
                { label: 'equipInfo.yc.LinkagePage', value: 'relatedPic' },
                { label: 'equipInfo.yc.LinkageVideo', value: 'relatedVideo' },
                { label: 'equipInfo.yc.NameOfAsset', value: 'ziChanId' },
                { label: 'equipInfo.yc.PlanNumber', value: 'planNo' },
                { label: 'equipInfo.yc.CurveRecord', value: 'curveRcd' },
                { label: 'equipInfo.yc.CurveRecordThreshold', value: 'curveLimit' },
                { label: 'equipInfo.yc.OverTheCeilingEvent', value: 'outmaxEvt' },
                { label: 'equipInfo.yc.OfflineIncident', value: 'outminEvt' },
                { label: 'equipInfo.dialog.alarm', value: 'alarmScheme' },
                { label: 'equipInfo.yc.LevelOfAlarm', value: 'lvlLevel' },
                { label: 'equipInfo.yc.ScalingTransformation', value: 'mapping' },
                { label: 'equipInfo.yc.Minimum', value: 'physicMin' },
                { label: 'equipInfo.yc.Maximum', value: 'physicMax' },
                { label: 'equipInfo.yc.ActualMinimum', value: 'ycMin' },
                { label: 'equipInfo.yc.ActualMaximum', value: 'ycMax' },
                { label: 'equipInfo.yc.OperatingCommand', value: 'mainInstruction' },
                { label: 'equipInfo.yc.OperatingParameter', value: 'minorInstruction' },
                { label: 'equipInfo.yc.ExcessDelayTime', value: 'alarmAcceptableTime' },
                { label: 'equipInfo.yc.RestoreDelayTime', value: 'restoreAcceptableTime' },
                { label: 'equipInfo.yc.RepeatedAlarmTime', value: 'alarmRepeatTime' },
                { label: 'equipInfo.yc.AlarmUpgradePeriod', value: 'alarmRiseCycle' },
                { label: 'equipInfo.yc.AlarmBlock', value: 'alarmShield' },
                { label: 'equipInfo.yc.SafetyPeriod', value: 'safeTime' },
                { label: 'equipInfo.common.expression', value: 'expression' },
                { label: 'equipInfo.common.customAttribute', value: 'parameters' },
            ],
            formData: {
                expression: '',
                unit: '',
                valTrait: 0,
                ziChanId: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                valMax: 999999,
                valMin: 0,
                physicMin: 0,
                physicMax: 999999,
                restoreMax: 999999,
                restoreMin: 0,
                ycMin: 0,
                ycMax: 999999,
                mainInstruction: '',
                minorInstruction: '',
                outminEvt: '',
                outmaxEvt: '',
                mapping: false,
                curveRcd: false,
                curveLimit: '',
                lvlLevel: 3,
                restoreAcceptableTime: '',
                alarmAcceptableTime: '',
                alarmRepeatTime: '',
                alarmScheme: '',
                waveFile: '',
                alarmShield: '',
                alarmRiseCycle: '',
                safeBgn: '',
                safeEnd: '',
                safeTime: '',
                dataType: null,
                parameters: []
            },
            batchYcForm: {},
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
            otherMinNum: -2147483647,
            expressionTips: '',

            getCustomPropData: {
                ids: [],
                operateEnum: 1
            }
        }
    },
    inject: ['getSelected', 'swit', 'swit1', 'alarmDropdown', 'getVideoDropdown', 'getZcDropdown', 'haveChangeItem', 'getPlanNoDropdown', 'getWaveFilenames'],
    computed: {
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
                selectVal: [{ type: 'array', required: true, message: this.$t('equipInfo.tips.select') + this.$t('equipInfo.yc.YcParameters') }]
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
            if (this.batchYcForm[item]) {
                if (!reg.test(this.batchYcForm[item])) {
                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.batchYcForm[item] = parseInt(this.batchYcForm[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.batchYcForm[item] = 0
                })
            }
        },

        // 保存批量修改yc
        saveBatchYc () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    let data = this.haveChangeItem(this.batchYcSelect.selectVal, this.batchYcForm, this.alarms, this.labelEquip, true)
                    if (!data) return
                    this.saveData(data, 'batchEditYc', 'showBatEditYcDialog', true)
                }
            })
        },
        resetAndClose () {
            this.$nextTick(() => {
                this.$refs.form.clearValidate()
            })
            this.closeDialog('showBatEditYcDialog')
        }
    }
}
