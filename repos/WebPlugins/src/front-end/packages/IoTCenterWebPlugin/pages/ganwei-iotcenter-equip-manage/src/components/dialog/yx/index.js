import equipUtils from '../../../utils/util.js'
import ziChanSelect from '../../UI/ziChanSelect/index.vue'
export default {
    components: { ziChanSelect },
    props: {
        showDialog: {
            type: Boolean,
            default: false,
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
            default: false,
        },
        saveLoading: {
            type: Boolean,
            default: false,
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
                    label: 'dataType.bool',
                    value: null
                },
                {
                    label: 'dataType.string',
                    value: 'S'
                }
            ],
            yxInfo: {},
            alarms: {
                isGenerateWO: 0,
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            minNum: 0,
            maxNum: 2147483647,
            activeNames: [''],
            labelEquip: 100,
            LevelOfAlarmZeroOne: 0,
            LevelOfAlarmOneZero: 0,
            currentSelectZiChan: {},
        }
    },
    watch: {
        showDialog: function (val) {
            if (val) {
                this.yxInfo = JSON.parse(JSON.stringify(this.info))
                this.alarms.isGenerateWO = (this.yxInfo.alarmScheme & 16) === 16 ? 1 : 0
                this.alarms.isAlarm = (this.yxInfo.alarmScheme & 1) === 1 ? 1 : 0;
                this.alarms.isMarkAlarm = (this.yxInfo.alarmScheme & 2) === 2 ? 1 : 0;
                this.alarms.messageAlarm = (this.yxInfo.alarmScheme & 4) === 4 ? 1 : 0;
                this.alarms.emailAlarm = (this.yxInfo.alarmScheme & 8) === 8 ? 1 : 0;
                if (this.yxInfo?.parameters) {
                    this.yxInfo.parameters.forEach((item, index) => {
                        item.id = index + 1;
                        item.repeatTip = '';
                        item.nullTip = ''
                    })
                } else {
                    this.yxInfo.parameters = []
                }
                if (this.yxInfo.ziChanId) {
                    this.currentSelectZiChan = {
                        ziChanId: this.yxInfo.ziChanId,
                        ziChanName: this.yxInfo.ziChanName,
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
        zcDropdown () {
            return this.getZcDropdown()
        },
        videoDropdown () {
            return this.getVideoDropdown()
        },
        planNoDropdown () {
            return this.getPlanNoDropdown();
        },
        yxInfoRules () {
            return {
                yxCode: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.yxCode', 'input'),
                        trigger: 'blur'
                    }
                ],
                yxNm: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.TeleindicationName', 'input'),
                        trigger: 'blur'
                    }
                ],
                evt01: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.zeroToOneEcent', 'input'),
                        trigger: 'blur'
                    }
                ],
                evt10: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.oneToZeroEvent', 'input'),
                        trigger: 'blur'
                    }
                ],
                mainInstruction: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.OperatingCommand', 'input'),
                        trigger: 'blur'
                    }
                ],
                minorInstruction: [
                    {
                        required: true,
                        message: this.getPlaceholder('yx.OperatingParameter', 'input'),
                        trigger: 'blur'
                    }
                ],
                alarmAcceptableTime: [
                    {
                        required: true,
                        message: this.$tt('yxRule.inputDelayAlarm'),
                        trigger: 'blur'
                    }
                ],
                restoreAcceptableTime: [
                    {
                        required: true,
                        message: this.$tt('yxRule.inputRestoreDelay'),
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
        customAttributeRules () {
            return {
                keyRules: [{
                    required: true, trigger: 'blur',
                    validator: (rule, value, callback) => {
                        if (!value) {
                            callback(this.getPlaceholder('common.parameterLabel', 'input'));
                            return;
                        }
                        let isHaveCtrl = this.yxInfo.parameters.filter(item => item.key == value).length > 1;
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
            this.yxInfo.parameters.forEach((item, index) => {
                this.$refs.customAttribute.validateField(`parameters[${index}][key]`, () => { })
            })
        },
        addParameter () {
            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    this.yxInfo.parameters.push({ key: '', value: '', repeatTip: '', nullTip: '', id: new Date().getTime() })
                }
            })
        },
        deleteParameter (index) {
            this.yxInfo.parameters.splice(index, 1)
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
        // 保存编辑遥信量
        saveYx () {
            let reg = new RegExp('^[0-9]*$');

            if (this.yxInfo.yxNm.replace(/(^\s*)|(\s*$)/g, '').length > 80) {
                this.$message.warning(this.$tt('tips.yxNameCannotExceed'));
                return;
            }

            if (this.yxInfo.relatedPic.length > 255) {
                this.$message.warning(this.$tt('tips.linkagePageCannotExceed'));
                return;
            }

            if (this.yxInfo.waveFile.length > 64) {
                this.$message.warning(this.$tt('tips.soundFileCannotExceed'));
                return;
            }

            if (!reg.test(this.yxInfo.alarmRiseCycle) ||
                Number(this.yxInfo.alarmRiseCycle) > 2147483647 ||
                Number(this.yxInfo.alarmRiseCycle) < 0
            ) {
                this.$message.warning(this.$tt('tips.AlarmUpgradeCannotExceed'));
                return;
            }

            // 验证安全时段
            if (this.yxInfo.safeTime) {
                let isVer = equipUtils.verificationSateTime(this.yxInfo.safeTime);
                if (!isVer.pass) {
                    this.$message.warning(this.$tt(isVer.warning))
                    return;
                }
            }

            if (this.yxInfo.alarmShield) {
                let reg = /[^0-9,+]/;  //匹配只能填写数字和,和+
                let reg1 = /^[0-9]/;   //匹配只能以数字开头   /^[0-9][^0-9,+][0-9]$/
                let reg2 = /[0-9]$/;   //匹配只能以数字结尾

                if (this.yxInfo.alarmShield.match(reg) == null && this.yxInfo.alarmShield.match(reg1) != null && this.yxInfo.alarmShield.match(reg2) != null) {
                    //不做处理
                } else {
                    this.$message.warning(this.$tt('tips.alarmBlockTips'));
                    return;
                }

                // 校验是否输入，+连续重复
                if (this.yxInfo.alarmShield.indexOf(' ') > -1 || this.yxInfo.alarmShield.match(/[+,]{2,}/)) {
                    this.$message.warning(this.$tt('tips.alarmBlockTips'));
                    return;
                }

                let alarmShieldArray = this.yxInfo.alarmShield.match(/\d+/g);
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


            let yxInfoValid = false,
                yxInfoSeniorValid = false, customAttributeValid = false,
                yxInfoAlarmForm = false, yxInfoCurveForm = false, yxInfoAlarmSettingForm = false, yxInfoPreventAlarmVForm = false;
            this.$refs['yxInfoForm'].validate((valid) => {
                if (valid) {
                    yxInfoValid = true;
                }
            });
            this.$refs['yxInfoSeniorForm'].validate((valid) => {
                if (valid) {
                    yxInfoSeniorValid = true;
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
            this.$refs.yxInfoAlarmForm.validate((valid) => {
                if (valid) {
                    yxInfoAlarmForm = true
                } else {
                    this.activeNames = ["3"];
                }
            })
            this.$refs.yxInfoCurveForm.validate((valid) => {
                if (valid) {
                    yxInfoCurveForm = true
                } else {
                    this.activeNames = ["4"];
                }
            })
            this.$refs.yxInfoAlarmSettingForm.validate((valid) => {
                if (valid) {
                    yxInfoAlarmSettingForm = true
                } else {
                    this.activeNames = ["5"];
                }
            })
            this.$refs.yxInfoPreventAlarmVForm.validate((valid) => {
                if (valid) {
                    yxInfoPreventAlarmVForm = true
                } else {
                    this.activeNames = ["6"];
                }
            })

            if (yxInfoValid && yxInfoSeniorValid && customAttributeValid
                && yxInfoAlarmForm && yxInfoCurveForm && yxInfoAlarmSettingForm && yxInfoPreventAlarmVForm) {
                let yxData = JSON.parse(JSON.stringify(this.yxInfo));
                yxData.alarmScheme = equipUtils.toDecimalSystem(this.alarms);
                yxData.yxNo = Number(this.yxInfo.yxNo);
                yxData.levelR = Number(this.yxInfo.levelR);
                yxData.levelD = Number(this.yxInfo.levelD);
                yxData.relatedVideo = yxData.relatedVideo ? this.labelEquip + '|' + yxData.relatedVideo : '';
                yxData.dataType = yxData.dataType ? yxData.dataType : null
                delete yxData.relatedVideoName;
                yxData.safeBgn = '00:00:00';
                yxData.safeEnd = '00:00:00';
                this.clearParameter(yxData.parameters)
                if (this.page != 'equipInfo') {
                    delete yxData.expression
                }
                if (this.isNew) {
                    delete yxData.yxNo;
                    let url = this.page === 'equipInfo' ? 'postAddYx' : 'postAddModelYx'
                    this.saveData(yxData, url, 'showYxDialog');
                } else {
                    let url = this.page === 'equipInfo' ? 'postUpdateYXPEquip' : 'postUpdateModelYXPEquip'
                    this.saveData(yxData, url, 'showYxDialog');
                }
            }
        },

        toInteger (item) {
            let reg = /^[0-9]+$/
            if (this.yxInfo[item]) {
                if (!reg.test(this.yxInfo[item])) {
                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.yxInfo[item] = parseInt(this.yxInfo[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.yxInfo[item] = 0
                })
            }

        },

        yxVideoChange (val) {
            this.yxInfo.relatedVideoName = val.channelName;
            this.yxInfo.relatedVideo = val.id;
        },

    }
}
