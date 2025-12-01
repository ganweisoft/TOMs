import equipUtils from '../../../utils/util.js'
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
        batchEquip: {
            type: Boolean,
            default: false
        },
        page: {
            type: String,
            default: 'equipInfo'
        },
        active: {
            type: Number,
            default: 0,
        }
    },

    data () {
        return {
            equipInfo: {},
            alarms: {
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            planNoDropdown: [],
            activeNames: [],
            minNum: 0,
            maxNum: 2147483647,
            equipModel: '',
            equipSelectedList: [],
            showBatSetEquipDialog: false,
            loading: false,
            selectedModeDialog: 0,
            labelEquip: 100,
            dialogLoading: true
        }
    },
    inject: ['swit', 'alarmDropdown', 'getZcDropdown', 'getVideoDropdown', 'getCommunicationDrvList', 'getEquipList'],
    computed: {
        zcDropdown () {
            return this.getZcDropdown()
        },
        videoDropdown () {
            return this.getVideoDropdown()
        },
        communicationDrvList () {
            return this.getCommunicationDrvList()
        },
        equipList () {
            return this.getEquipList()
        },
        equipInfoRules () {
            return {
                groupId: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputGroupName'),
                        trigger: 'blur'
                    }
                ],
                equipNm: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputEquipName'),
                        trigger: 'blur'
                    }
                ],
                accCyc: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputAccCyc'),
                        trigger: 'blur'
                    }
                ],
                outOfContact: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputOutOfContact'),
                        trigger: 'blur'
                    }
                ],
                contacted: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputContacted'),
                        trigger: 'blur'
                    }
                ],
                communicationDrv: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputCommunicationDrv'),
                        trigger: 'blur'
                    }
                ],
                localAddr: [
                    {
                        required: true,
                        message: '请输入通讯端口号',
                        trigger: 'blur'
                    }
                ],
                communicationTimeParam: [
                    {
                        required: true,
                        message: this.$t('equipInfo.equipRule.inputCommunicationTimeParam'),
                        trigger: 'blur'
                    }
                ],
                alarmRiseCycle: [
                    {
                        required: true,
                        message: this.$t('equipInfo.rules.inputAlarmRiseCycle'),
                        trigger: 'blur'
                    }
                ]
            }
        }
    },
    watch: {
        info: function (val) {
            if (val) {
                this.equipInfo = JSON.parse(JSON.stringify(this.info));
                this.alarms.isAlarm = (this.equipInfo.alarmScheme & 1) === 1 ? 1 : 0;
                this.alarms.isMarkAlarm = (this.equipInfo.alarmScheme & 2) === 2 ? 1 : 0;
                this.alarms.messageAlarm = (this.equipInfo.alarmScheme & 4) === 4 ? 1 : 0;
                this.alarms.emailAlarm = (this.equipInfo.alarmScheme & 8) === 8 ? 1 : 0;
            } else {
                this.alarms.emailAlarm = 0;
                this.alarms.messageAlarm = 0;
                this.alarms.isMarkAlarm = 0;
                this.alarms.isAlarm = 0;
                this.activeNames = [];
                this.equipModel = ''
            }
            this.dialogLoading = false
        }

        // showDialog: function (val) {

        // }
    },

    methods: {
        $tt(string) {
            return this.$t(this.$route.name + '.' + string)
        },

        // 保存编辑设备
        saveEquipData () {
            this.equipInfo.equipNm = this.equipInfo.equipNm.replace(/(^\s*)|(\s*$)/g, '');

            let reg = new RegExp('^[0-9]*$');
            if (!reg.test(this.equipInfo.alarmRiseCycle) || this.equipInfo.alarmRiseCycle > 2147483647) {
                this.$message.warning(this.$t('equipInfo.tips.AlarmUpgradeCannotExceed'));
                return;
            }

            // 验证安全时段
            if (this.equipInfo.safeTime) {
                let isVer = equipUtils.verificationSateTime(this.equipInfo.safeTime);
                if (!isVer.pass) {
                    this.$message.warning(this.$t(isVer.warning))
                    return;
                }
            }

            // 验证通讯时间参数
            // if (this.equipInfo.communicationTimeParam){
            //     let reg = /[-#$@\\()<>{}[\]-_;:'"?？‘’“”：；【】=——、|。》《*&……%￥！!.，]/gi
            //     if (reg.test(this.equipInfo.communicationTimeParam) || this.equipInfo.communicationTimeParam.split('/').length < 4){
            //         this.$message.warning(this.$t('equipInfo.equip.communicationTimeParamErr'));
            //         return;
            //     }
            // }

            if (this.equipInfo.backup && this.equipInfo.backup.length > 255) {
                this.$message.warning(this.$t('equipInfo.tips.back'));
                return;
            }
            let equipInfoValid = true,
                equipInfoSeniorValid = true;
            // this.$refs['equipInfoForm'].validate((valid) => {
            //     if (valid) {
            //         equipInfoValid = true;
            //     }
            // });
            // this.$refs['equipInfoSeniorForm'].validate((valid) => {
            //     if (valid) {
            //         equipInfoSeniorValid = true;
            //     } else {
            //         this.activeNames = ['1'];
            //     }
            // });
            if (equipInfoValid == true && equipInfoSeniorValid == true) {

                // 转化后赋值给alarmScheme
                let equipData = JSON.parse(JSON.stringify(this.equipInfo));
                equipData.alarmScheme = equipUtils.toDecimalSystem(this.alarms);
                equipData.staNo = Number(equipData.staNo);
                equipData.equipNo = Number(equipData.equipNo);
                equipData.rawEquipNo = Number(equipData.rawEquipNo);
                equipData.attrib = Number(equipData.attrib);
                equipData.alarmRiseCycle = Number(equipData.alarmRiseCycle);
                equipData.relatedVideo = equipData.relatedVideo ? this.labelEquip + '|' + equipData.relatedVideo : '';
                this.saveData(equipData)
            }
        },

        toInteger (item) {
            let reg = /^[0-9]+$/
            if (this.equipInfo[item]) {
                if (!reg.test(this.equipInfo[item])) {

                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.equipInfo[item] = parseInt(this.equipInfo[item])
                    })
                }
            } else {
                this.$nextTick(() => {
                    this.equipInfo[item] = 0
                })
            }

        },

        // 批量应用
        bactchSetEquip (key, item) {
            this.showBatSetEquipDialog = true;
            this.selectEquipType = key;
        },

        closeBatchSetEquip () {
            // $('.set-equip .checkInput-active').removeClass('checkInput-active');
            this.selectedAll = false;
            this.newGroupId = '';
        }

    }
}