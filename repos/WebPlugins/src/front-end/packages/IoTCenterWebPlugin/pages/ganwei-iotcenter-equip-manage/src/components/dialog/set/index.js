export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },
        closeDialog: {
            type: Function,
            default: () => { }
        },
        info: {
            type: Object,
            default: () => { }
        },
        isNew: {
            type: Boolean,
            default: false
        },
        saveLoading: {
            type: Boolean,
            default: false
        },
        saveData: {
            type: Function,
            default: () => { }
        },
        page: {
            type: String,
            default: 'equipInfo'
        }
    },
    inject: ['swit', 'swit1'],
    watch: {
        showDialog: function (val) {
            if (val) {
                this.activeNames = [''];
                this.setInfo = JSON.parse(JSON.stringify(this.info))
                if (this.setInfo?.parameters) {
                    this.setInfo.parameters.forEach((item, index) => {
                        item.id = index + 1;
                        item.repeatTip = '',
                            item.nullTip = ''
                    })
                } else {
                    this.setInfo.parameters = []
                }
            }
        }
    },
    data () {
        return {
            setInfo: {},
            activeNames: [''],
            setTypeOptions: [
                {
                    label: 'setTypeOptions.parameterSettings',
                    value: 'V'
                },
                {
                    label: 'setTypeOptions.signalingSettings',
                    value: 'X'
                },
                {
                    label: 'setTypeOptions.telemetrySettings',
                    value: 'C'
                },
                {
                    label: 'setTypeOptions.systemSettings',
                    value: 'S'
                },
                {
                    label: 'setTypeOptions.sceneSettings',
                    value: 'J'
                },
            ],
        }
    },
    computed: {
        setInfoRules () {
            return {
                setCode: [
                    {
                        required: true,
                        message: this.getPlaceholder('set.setCode', 'input'),
                        trigger: 'blur'
                    }
                ],
                value: [
                    {
                        required: true,
                        message: this.getPlaceholder('set.value', 'input'),
                        trigger: 'blur'
                    }
                ],
                setType: [
                    {
                        required: true,
                        message: this.getPlaceholder('set.typeOfSetting', 'select'),
                        trigger: 'change'
                    }
                ],
                setNm: [
                    {
                        required: true,
                        message: this.getPlaceholder('set.settingName', 'input'),
                        trigger: 'blur'
                    }
                ],
                mainInstruction: [
                    {
                        required: true,
                        message: this.getPlaceholder('set.OperatingCommand', 'input'),
                        trigger: 'blur'
                    }
                ],
                voiceKeys: [
                    {
                        validator: (rule, value, callback) => {
                            if (!value && this.setInfo.enableVoice) {
                                callback(new Error(this.$tt('set.VoiceControlCharacterTips')));
                            }
                            callback()
                        },
                        trigger: 'blur'
                    }
                ]
            }
        },
        customAttributeRules () {
            return {
                keyRules: [{
                    required: true, trigger: 'blur',
                    validator: (rule, value, callback) => {
                        if (!value) {
                            callback(this.getPlaceholder('common.parameterLabel', 'input'));
                            return;
                        }
                        let isHaveCtrl = this.setInfo.parameters.filter(item => item.key == value).length > 1;
                        if (isHaveCtrl) {
                            callback(this.$tt('common.repeatTip'))
                            return
                        }
                        callback()
                    }
                }],
                valueRules: [{ required: true, message: this.getPlaceholder('common.parameterValue', 'input'), trigger: 'blur' }]
            }
        }
    },
    methods: {
        inputKeyBlur (item) {
            item.key = item.key.replace(/[^a-zA-Z0-9]/g, '')
            this.setInfo.parameters.forEach((item, index) => {
                this.$refs.customAttribute.validateField(`parameters[${index}][key]`, () => { })
            })
        },
        addParameter () {
            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    this.setInfo.parameters.push({ key: '', value: '', repeatTip: '', nullTip: '', id: new Date().getTime() })
                }
            })
        },
        deleteParameter (index) {
            this.setInfo.parameters.splice(index, 1)
        },

        clearParameter (arr) {
            arr.forEach(item => {
                delete item.id;
                delete item.nullTip;
                delete item.repeatTip;
            })
        },
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getPlaceholder (label, type) {
            if (type == 'select') {
                return this.$tt('tips.select') + this.$tt(label)
            }
            return this.$tt('tips.input') + this.$tt(label)
        },
        // 保存编辑设置
        saveSet () {

        // 新增默认值
        this.setInfo.voiceKeys = "打开大屏"
            let setInfoValid = false,
                setInfoSeniorValid = false, customAttributeValid = false;
            this.$refs['setInfoForm'].validate(valid => {
                if (valid) {
                    setInfoValid = true
                }
            })
            this.$refs['setInfoSeniorForm'].validate(valid => {
                if (valid) {
                    setInfoSeniorValid = true
                } else {
                    this.activeNames = ['1']
                }
            })

            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    customAttributeValid = true
                } else {
                    this.activeNames = ["2"];
                }
            })

            if (setInfoValid && setInfoSeniorValid && customAttributeValid) {
                let setData = JSON.parse(JSON.stringify(this.setInfo))
                setData.setNo = Number(this.setInfo.setNo)
                setData.enableVoice = Boolean(this.setInfo.enableVoice)
                setData.canexecution = Boolean(this.setInfo.canexecution)
                setData.record = Boolean(this.setInfo.record)
                this.clearParameter(setData.parameters)
                if (this.isNew) {
                    delete setData.setNo
                    if (this.page === 'equipInfo') {
                        this.saveData(setData, 'postAddSet', 'showSetDialog')
                    } else {
                        this.saveData(setData, 'postAddModelSet', 'showSetDialog')
                    }
                } else {
                    if (this.page === 'equipInfo') {
                        this.saveData(setData, 'postUpdateSetParmEquip', 'showSetDialog')
                    } else {
                        this.saveData(setData, 'postUpdateModelSetParmEquip', 'showSetDialog')
                    }
                }
            }
        }
    }
}
