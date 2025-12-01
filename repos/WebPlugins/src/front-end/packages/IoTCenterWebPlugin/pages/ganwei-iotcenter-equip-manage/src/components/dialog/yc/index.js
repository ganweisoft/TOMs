import equipUtils from '../../../utils/util.js'
import ziChanSelect from '../../UI/ziChanSelect/index.vue'
export default {
    components: { ziChanSelect },
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
    watch: {
        showDialog: function (val) {
            if (val) {
                this.ycInfo = JSON.parse(JSON.stringify(this.info));
                this.alarms.isGenerateWO = (this.ycInfo.alarmScheme & 16) === 16 ? 1 : 0
                this.alarms.isAlarm = (this.ycInfo.alarmScheme & 1) === 1 ? 1 : 0;
                this.alarms.isMarkAlarm = (this.ycInfo.alarmScheme & 2) === 2 ? 1 : 0;
                this.alarms.messageAlarm = (this.ycInfo.alarmScheme & 4) === 4 ? 1 : 0;
                this.alarms.emailAlarm = (this.ycInfo.alarmScheme & 8) === 8 ? 1 : 0;

                if (this.ycInfo?.parameters) {
                    this.ycInfo.parameters.forEach((item, index) => {
                        item.id = index + 1;
                        item.repeatTip = '',
                            item.nullTip = ''
                    })
                } else {
                    this.ycInfo.parameters = []
                }

                if (this.ycInfo.lvlLevel > 9) {
                    this.LevelOfAlarm = 10
                } else {
                    this.LevelOfAlarm = this.ycInfo.lvlLevel
                }
                if (this.ycInfo.ziChanId) {
                    this.currentSelectZiChan = {
                        ziChanId: this.ycInfo.ziChanId,
                        ziChanName: this.ycInfo.ziChanName,
                    }
                } else {
                    this.currentSelectZiChan = {}
                }
            } else {
                this.alarms.emailAlarm = 0;
                this.alarms.messageAlarm = 0;
                this.alarms.isMarkAlarm = 0;
                this.alarms.isAlarm = 0;
                this.alarms.isGenerateWO = 0;
                this.activeNames = [''];
                this.currentSelectZiChan = {}
            }
        }
    },
    inject: ['swit', 'swit1', 'getZcDropdown', 'getVideoDropdown', 'getPlanNoDropdown', 'getWaveFilenames'],
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
            return this.getPlanNoDropdown();
        },
        ycInfoRules () {
            return {
                ycCode: [
                    {
                        required: true,
                        message: this.getPlaceholder('yc.ycCode', 'input'),
                        trigger: 'blur'
                    }
                ],
                ycNm: [
                    {
                        required: true,
                        message: this.getPlaceholder('yc.TelemeteringName', 'input'),
                        trigger: 'blur'
                    }
                ],
                valMin: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputLowerVal'),
                        trigger: 'blur'
                    }
                ],
                restoreMin: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputRepLowerVal'),
                        trigger: 'blur'
                    }
                ],
                restoreMax: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputRepUpperVal'),
                        trigger: 'blur'
                    }
                ],
                valMax: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputUpperVal'),
                        trigger: 'blur'
                    }
                ],
                curveLimit: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputRecordThreshold'),
                        trigger: 'blur'
                    }
                ],
                ycMin: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputYcMin'),
                        trigger: 'blur'
                    }
                ],
                ycMax: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputYcMax'),
                        trigger: 'blur'
                    }
                ],
                physicMin: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputPhysicMin'),
                        trigger: 'blur'
                    }
                ],
                physicMax: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputPhysicMax'),
                        trigger: 'blur'
                    }
                ],
                mainInstruction: [
                    {
                        required: true,
                        message: this.getPlaceholder('yc.OperatingCommand', 'input'),
                        trigger: 'blur'
                    }
                ],
                minorInstruction: [
                    {
                        required: true,
                        message: this.getPlaceholder('yc.OperatingParameter', 'input'),
                        trigger: 'blur'
                    }
                ],
                alarmAcceptableTime: [
                    {
                        required: true,
                        message: this.$tt('ycRule.inputAlarmAcceptableTime'),
                        trigger: 'blur'
                    }
                ],
                restoreAcceptableTime: [
                    {
                        required: true,
                        message: this.$tt('rules.inputRestoreAcceptableTime'),
                        trigger: 'blur'
                    }
                ],
                alarmRepeatTime: [
                    {
                        required: true,
                        message: this.$tt('rules.inputAlarmRepeatTime'),
                        trigger: 'blur'
                    }
                ]
            }
        },
        alarmDropdown () {
            return [
                {
                    name: '0',
                    value: 0
                },
                {
                    name: '1',
                    value: 1
                },
                {
                    name: '2',
                    value: 2
                },
                {
                    name: '3',
                    value: 3
                },
                {
                    name: '4',
                    value: 4
                },
                {
                    name: '5',
                    value: 5
                },
                {
                    name: '6',
                    value: 6
                },
                {
                    name: '7',
                    value: 7
                },
                {
                    name: '8',
                    value: 8
                },
                {
                    name: '9',
                    value: 9
                }
            ]
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
                        let isHaveCtrl = this.ycInfo.parameters.filter(item => item.key == value).length > 1;
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

    mounted () {
        setTimeout(() => {
            this.expressionTips = this.$tt('common.expressionTips')
        }, 1000)
    },
    data () {
        return {
            expressionTips: '',
            dataTypeOption: [
                {
                    label: 'dataType.double',
                    value: null
                },
                {
                    label: 'dataType.string',
                    value: 'S'
                },
                {
                    label: 'dataType.twoTuples',
                    value: 'SEQ2'
                },
                {
                    label: 'dataType.threeTuples',
                    value: 'SEQ3'
                },
                {
                    label: 'dataType.fourTuples',
                    value: 'SEQ4'
                },
                {
                    label: 'dataType.fiveTuples',
                    value: 'SEQ5'
                },
                {
                    label: 'dataType.sixTuples',
                    value: 'SEQ6'
                },
                {
                    label: 'dataType.sevenTuples',
                    value: 'SEQ7'
                }
            ],
            ycInfo: {},
            alarms: {
                isGenerateWO: 0,
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            minNum: 0,
            maxNum: 2147483647,
            otherMinNum: -2147483647,
            activeNames: [''],
            labelEquip: 100,
            LevelOfAlarm: '',
            currentSelectZiChan: {},
        }
    },

    methods: {
        inputKeyBlur (item) {
            item.key = item.key.replace(/[^a-zA-Z0-9]/g, '')
            this.ycInfo.parameters.forEach((item, index) => {
                this.$refs.customAttribute.validateField(`parameters[${index}][key]`, () => { })
            })
        },
        addParameter () {
            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    this.ycInfo.parameters.push({ key: '', value: '', repeatTip: '', nullTip: '', id: new Date().getTime() })
                }
            })
        },
        deleteParameter (index) {
            this.ycInfo.parameters.splice(index, 1)
        },

        clearParameter (arr) {
            arr.forEach(item => {
                delete item.id;
                delete item.nullTip;
                delete item.repeatTip;
            })
        },
        rePlaceNoNum (val) {
            return val.replaceAll(/\D/g, "").trim()
        },
        $tt (string) {
            return this.$t(this.$route.name + '.' + string)
        },
        getPlaceholder (label, type) {
            if (type == 'select') {
                return this.$tt('tips.select') + this.$tt(label)
            }
            return this.$tt('tips.input') + this.$tt(label)
        },

        // 保存编辑遥测量
        saveYc () {
            let regu = /^(\-|\+)?\d+(\.\d+)?$/;

            if (this.ycInfo.ycNm.replace(/(^\s*)|(\s*$)/g, '').length > 80) {
                this.$message.warning(this.$tt('tips.telemetryNameCannotExceed'));
                return;
            }
            this.ycInfo.ycNm = this.ycInfo.ycNm.replace(/(^\s*)|(\s*$)/g, '');

            if (
                !regu.test(this.ycInfo.valMax) ||
                this.ycInfo.valMax > 2147483647 ||
                this.ycInfo.valMax < -2147483647
            ) {
                this.$message.warning(this.$tt('tips.upperLimitCannotExceed'));
                return;
            }
            if (
                !regu.test(this.ycInfo.valMin) ||
                this.ycInfo.valMin > 2147483647 ||
                this.ycInfo.valMin < -2147483647
            ) {
                this.$message.warning(this.$tt('tips.lowerLimitCannotExceed'));
                return;
            }
            if (
                !regu.test(this.ycInfo.restoreMin) ||
                this.ycInfo.restoreMin > 2147483647 ||
                this.ycInfo.restoreMin < -2147483647
            ) {
                this.$message.warning(this.$tt('tips.RepLowerLimitCannotExceed'));
                return;
            }
            if (
                !regu.test(this.ycInfo.restoreMax) ||
                this.ycInfo.restoreMax > 2147483647 ||
                this.ycInfo.restoreMax < -2147483647
            ) {
                this.$message.warning(this.$tt('tips.RepUpperLimitCannotExceed'));
                return;
            }

            //
            // 1.下限值，回复下限值，上限值，回复上限值只能填写-2147483647—2147483647的数字类型（负数，小数点，整数），
            // 超过提示只能填写-2147483647—2147483647的数字类型

            if (this.ycInfo.valMin > this.ycInfo.restoreMin) {
                this.$message.warning(this.$tt('tips.lowerLimitCannotRep'));
                return;
            }

            if (this.ycInfo.restoreMax > this.ycInfo.valMax) {
                this.$message.warning(this.$tt('tips.repUpperLimitCannotUpper'));
                return;
            }

            if (this.ycInfo.restoreMin >= this.ycInfo.restoreMax) {
                this.$message.warning(this.$tt('tips.repLowerCannotRepUpper'));
                return;
            }

            if (this.ycInfo.valMin >= this.ycInfo.valMax) {
                this.$message.warning(this.$tt('tips.lowerLimitCannotUpperLimit'));
                return;
            }

            // 2.下限值不能大于回复下限值

            // 2.回复上限值不能大于上限值

            // 3.回复下限值不能大于等于回复上限值

            // 4.下限值不能大于等于上限值

            // 验证安全时段
            if (this.ycInfo.safeTime) {
                let isVer = equipUtils.verificationSateTime(this.ycInfo.safeTime);
                if (!isVer.pass) {
                    this.$message.warning(this.$tt(isVer.warning))
                    return;
                }
            }
            if (this.ycInfo.alarmShield) {
                let reg = /[^0-9,+]/;  // 匹配只能填写数字和,和+
                let reg1 = /^[0-9]/;   // 匹配只能以数字开头   /^[0-9][^0-9,+][0-9]$/
                let reg2 = /[0-9]$/;   // 匹配只能以数字结尾

                if (this.ycInfo.alarmShield.match(reg) == null && this.ycInfo.alarmShield.match(reg1) != null && this.ycInfo.alarmShield.match(reg2) != null) {

                    // 不做处理
                } else {
                    this.$message.warning(this.$tt('tips.alarmBlockTips'));
                    return;
                }


                // 校验是否输入，+连续重复
                if (this.ycInfo.alarmShield.indexOf(' ') > -1 || this.ycInfo.alarmShield.match(/[+,]{2,}/)) {
                    this.$message.warning(this.$tt('tips.alarmBlockTips'));
                    return;
                }

                let alarmShieldArray = this.ycInfo.alarmShield.match(/\d+/g);
                let pass = true

                alarmShieldArray.forEach(item => {
                    if (parseInt(item) > 2147483647 || parseInt(item) == 0) {
                        pass = false
                    }
                })
                if (!pass) {
                    this.$message.warning(this.$tt('tips.alarmBlockTips'));
                    return;
                }
            }

            if (!this.ycInfo.lvlLevel && this.LevelOfAlarm == 10) {
                this.$message.warning(this.$tt('tips.inputLevelOfAlarm'))
                return;
            }


            let ycInfoValid = false,
                ycInfoSeniorValid = false, customAttributeValid = false,
                ycInfoAlarmForm = false, ycInfoCurveForm = false, ycInfoMappingForm = false, ycInfoAlarmThresholdForm = false, ycInfoPreventAlarmVForm = false;
            this.$refs['ycInfoForm'].validate((valid) => {
                if (valid) {
                    ycInfoValid = true;
                }
            });
            this.$refs['ycInfoSeniorForm'].validate((valid) => {
                if (valid) {
                    ycInfoSeniorValid = true;
                } else {
                    this.activeNames = ['1'];
                }
            });

            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    customAttributeValid = true
                } else {
                    this.activeNames = ["2"];
                }
            })
            this.$refs.ycInfoAlarmForm.validate((valid) => {
                if (valid) {
                    ycInfoAlarmForm = true
                } else {
                    this.activeNames = ["3"];
                }
            })
            this.$refs.ycInfoCurveForm.validate((valid) => {
                if (valid) {
                    ycInfoCurveForm = true
                } else {
                    this.activeNames = ["4"];
                }
            })
            this.$refs.ycInfoMappingForm.validate((valid) => {
                if (valid) {
                    ycInfoMappingForm = true
                } else {
                    this.activeNames = ["5"];
                }
            })
            this.$refs.ycInfoAlarmThresholdForm.validate((valid) => {
                if (valid) {
                    ycInfoAlarmThresholdForm = true
                } else {
                    this.activeNames = ["6"];
                }
            })
            this.$refs.ycInfoPreventAlarmVForm.validate((valid) => {
                if (valid) {
                    ycInfoPreventAlarmVForm = true
                } else {
                    this.activeNames = ["7"];
                }
            })

            if (ycInfoValid && ycInfoSeniorValid && customAttributeValid
                && ycInfoAlarmForm && ycInfoCurveForm && ycInfoMappingForm
                && ycInfoAlarmThresholdForm && ycInfoPreventAlarmVForm) {
                let ycData = JSON.parse(JSON.stringify(this.ycInfo));
                ycData.alarmScheme = equipUtils.toDecimalSystem(this.alarms);
                ycData.ycNo = Number(this.ycInfo.ycNo);
                ycData.lvlLevel = Number(this.ycInfo.lvlLevel);
                ycData.safeBgn = '00:00:00';
                ycData.safeEnd = '00:00:00';
                ycData.relatedVideo = ycData.relatedVideo ? this.labelEquip + '|' + ycData.relatedVideo : '';
                ycData.dataType = ycData.dataType ? ycData.dataType : null
                this.clearParameter(ycData.parameters)
                delete ycData.relatedVideoName;
                if (this.page != 'equipInfo') {
                    delete ycData.expression
                }
                if (this.isNew) {
                    delete ycData.ycNo;
                    let url = this.page === 'equipInfo' ? 'postAddYc' : 'postAddModelYc'
                    this.saveData(ycData, url, 'showYcDialog');
                } else {
                    let url = this.page === 'equipInfo' ? 'postUpdateYCPEquip' : 'postUpdateModelYCPEquip'
                    this.saveData(ycData, url, 'showYcDialog');
                }
            }
        },

        toInteger (item) {
            let reg = /^[0-9]+$/
            if (this.ycInfo[item]) {
                if (!reg.test(this.ycInfo[item])) {

                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.ycInfo[item] = parseInt(this.ycInfo[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.ycInfo[item] = 0
                })
            }

        },

        ycVideoChange (val) {
            this.ycInfo.relatedVideoName = val.channelName;
            this.ycInfo.relatedVideo = val.id;
        }
    }
}
