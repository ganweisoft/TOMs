import customAttribute from '../../customAttribute/index.vue'
export default {
    components: { customAttribute },
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
            this.showBatEditSetDialog = val
            if (!val) {
                this.batchSetSelect.selectVal = []
            } else {
                this.batchSetForm = JSON.parse(JSON.stringify(this.formData))
            }
        },
        'batchSetSelect.selectVal': function (newVal, oldVal) {
            if (newVal.includes('parameters') && !oldVal.includes('parameters')) {
                this.getCustomPropData = {
                    ids: this.getSelected(),
                    operateEnum: 3
                }
            }
        }
    },
    data () {
        return {
            setTypeOptions: [
                {
                    label: 'equipInfo.setTypeOptions.parameterSettings',
                    value: 'V'
                },
                {
                    label: 'equipInfo.setTypeOptions.signalingSettings',
                    value: 'X'
                },
                {
                    label: 'equipInfo.setTypeOptions.telemetrySettings',
                    value: 'C'
                },
                {
                    label: 'equipInfo.setTypeOptions.systemSettings',
                    value: 'S'
                },
                {
                    label: 'equipInfo.setTypeOptions.sceneSettings',
                    value: 'J'
                },
            ],
            showBatEditSetDialog: false,
            // 批量修改set
            batchSetSelect: {
                selectVal: []
            },

            setSelectList: [
                { label: 'equipInfo.set.value', value: 'value' },
                { label: 'equipInfo.set.typeOfSetting', value: 'setType' },
                { label: 'equipInfo.set.action', value: 'action' },
                { label: 'equipInfo.set.OperatingCommand', value: 'mainInstruction' },
                { label: 'equipInfo.set.OperatingParameter', value: 'minorInstruction' },
                { label: 'equipInfo.set.record', value: 'record' },
                { label: 'equipInfo.set.VoiceControlCharacter', value: 'voiceKeys' },
                { label: 'equipInfo.set.VoiceControlOrNot', value: 'enableVoice' },
                { label: 'equipInfo.set.offline', value: 'offline' },
                { label: 'equipInfo.set.enable', value: 'enableSetParm' },
                { label: 'equipInfo.common.customAttribute', value: 'parameters' },
            ],
            formData: {
                offline: false,
                enableSetParm: false,
                parameters: [],
                setType: '',
                mainInstruction: '',
                minorInstruction: '',
                record: false,
                action: '',
                value: '',
                voiceKeys: '',
                enableVoice: false,
                canexecution: 0,
            },
            batchSetForm: {},

            getCustomPropData: {
                ids: [],
                operateEnum: 3
            }
        }
    },
    computed: {
        rules () {
            return {
                selectVal: [{ type: 'array', required: true, message: this.$t('equipInfo.tips.select') + this.$t('equipInfo.set.setParameters') }]
            }
        }
    },
    inject: ['getSelected', 'swit1', 'haveChangeItem'],
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
        // 保存批量修改set
        saveBatchSet () {
            this.$refs.form.validate(valid => {
                if (valid) {
                    let data = this.haveChangeItem(this.batchSetSelect.selectVal, this.batchSetForm)
                    if (!data) return
                    this.saveData(data, 'batchEditSet', 'showBatEditSetDialog')
                }
            })
        },
        resetAndClose () {
            this.$nextTick(() => {
                this.$refs.form.clearValidate()
            })
            this.closeDialog('showBatEditSetDialog')
        }
    }
}
