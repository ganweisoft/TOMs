import tree from "gw-base-components-plus/treeV2";
import equipUtils from "../../../utils/util.js";
import ziChanSelect from '../../UI/ziChanSelect/index.vue'
import { valid } from "semver";
export default {
    components: { tree, ziChanSelect },
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
            default: "equipInfo"
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
            activeNames: [""],
            minNum: 0,
            maxNum: 2147483647,
            equipModel: "",
            equipSelectedList: [],
            showBatSetEquipDialog: false,
            loading: false,
            selectedModeDialog: 0,
            labelEquip: 100,
            dialogLoading: true,
            equipType: 0,
            equipList: [],
            currentSelectZiChan: {}
        };
    },
    inject: [
        "swit",
        "alarmDropdown",
        "getVideoDropdown",
        "getCommunicationDrvList",
        "getEquipList",
        "equipTypes",
        "getPlanNoDropdown",
        "getWaveFilenames"
    ],
    computed: {
        waveFilenames () {
            return this.getWaveFilenames()
        },
        videoDropdown () {
            return this.getVideoDropdown();
        },
        planNoDropdown () {
            return this.getPlanNoDropdown();
        },
        communicationDrvList () {
            return this.getCommunicationDrvList();
        },

        equipInfoRules () {
            return {
                groupId: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputGroupName"),
                        trigger: "blur"
                    }
                ],
                equipNm: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputEquipName"),
                        trigger: "blur"
                    }
                ],
                accCyc: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputAccCyc"),
                        trigger: "blur"
                    }
                ],
                contacted: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputContacted"),
                        trigger: "blur"
                    }
                ],
                communicationDrv: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputCommunicationDrv"
                        ),
                        trigger: "blur"
                    }
                ],
                communicationTimeParam: [
                    {
                        required: true,
                        message: this.$tt("equipRule.inputCommunicationTimeParam"
                        ),
                        trigger: "blur"
                    }
                ],
                alarmRiseCycle: [
                    {
                        required: true,
                        message: this.$tt("rules.inputAlarmRiseCycle"
                        ),
                        trigger: "blur"
                    }
                ],
                equipconntype: [
                    {
                        required: true,
                        message: this.$tt("equipRule.selectEquipType"
                        ),
                        trigger: "blur"
                    }
                ]
            };
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
                        let isHaveCtrl = this.equipInfo.parameters.filter(item => item.key == value).length > 1;
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
    watch: {
        showDialog: function (val) {
            if (!val) {
                this.activeNames = [""];
            }
        },

        info: function (val) {
            if (val) {
                this.equipInfo = JSON.parse(JSON.stringify(this.info));
                this.alarms.isAlarm =
                    (this.equipInfo.alarmScheme & 1) === 1 ? 1 : 0;
                this.alarms.isMarkAlarm =
                    (this.equipInfo.alarmScheme & 2) === 2 ? 1 : 0;
                this.alarms.messageAlarm =
                    (this.equipInfo.alarmScheme & 4) === 4 ? 1 : 0;
                this.alarms.emailAlarm =
                    (this.equipInfo.alarmScheme & 8) === 8 ? 1 : 0;
                if (this.equipInfo.ziChanId) {
                    this.currentSelectZiChan = {
                        ziChanId: this.equipInfo.ziChanId,
                        ziChanName: this.equipInfo.ziChanName,
                    }
                } else {
                    this.currentSelectZiChan = {}
                }
                if (this.equipInfo?.parameters) {
                    this.equipInfo.parameters.forEach((item, index) => {
                        item.id = index + 1;
                        item.repeatTip = '',
                            item.nullTip = ''
                    })
                } else {
                    this.equipInfo.parameters = []
                }
            } else {
                this.alarms.emailAlarm = 0;
                this.alarms.messageAlarm = 0;
                this.alarms.isMarkAlarm = 0;
                this.alarms.isAlarm = 0;

                this.equipModel = "";
                this.currentSelectZiChan = {}
            }
            this.dialogLoading = false;
        }
    },

    methods: {
        $tt (string) {
            return this.$t(this.$route.name + '.' + string)
        },
        inputKeyBlur (item) {
            item.key = item.key.replace(/[^a-zA-Z0-9]/g, '')
            this.equipInfo.parameters.forEach((item, index) => {
                this.$refs.customAttribute.validateField(`parameters[${index}][key]`, () => { })
            })
        },
        addParameter () {
            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    this.equipInfo.parameters.push({ key: '', value: '', repeatTip: '', nullTip: '', id: new Date().getTime() })
                }
            })
        },
        deleteParameter (index) {
            this.equipInfo.parameters.splice(index, 1)
        },

        clearParameter (arr) {
            arr.forEach(item => {
                delete item.id;
                delete item.nullTip;
                delete item.repeatTip;
            })
        },

        getPlaceholder (label, type) {
            if (type == 'select') {
                return this.$tt('tips.select') + this.$tt(label)
            }
            return this.$tt('tips.input') + this.$tt(label)
        },

        filterOtherWords (value) {
            let reg = /[\-\,\!\|\~\`\(\)\#\$\%\^\&\*\{\}\:\;\"\L\<\>\?\]\[\.\@\￥\\\/\·\！\+\-\…\】\【\、]/g
            return value.replaceAll(reg, '').trim()
        },

        // 保存编辑设备
        saveEquipData () {
            this.equipInfo.equipNm = this.equipInfo.equipNm.replace(
                /(^\s*)|(\s*$)/g,
                ""
            );

            let reg = new RegExp("^[0-9]*$");
            if (
                !reg.test(this.equipInfo.alarmRiseCycle) ||
                this.equipInfo.alarmRiseCycle > 2147483647
            ) {
                this.$message.warning(
                    this.$tt("tips.AlarmUpgradeCannotExceed")
                );
                return;
            }

            // 验证安全时段
            if (this.equipInfo.safeTime) {
                let isVer = equipUtils.verificationSateTime(
                    this.equipInfo.safeTime
                );
                if (!isVer.pass) {
                    this.$message.warning(this.$tt(isVer.warning));
                    return;
                }
            }

            if (this.equipInfo.backup && this.equipInfo.backup.length > 255) {
                this.$message.warning(this.$tt("tips.back"));
                return;
            }

            let equipInfoValid = false,
                equipInfoSeniorValid = false, customAttributeValid = false,
                equipInfoAlarmFormValid = false, equipInfoAlarmSettingFormValid = false;
            this.$refs["equipInfoForm"].validate(valid => {
                if (valid) {
                    equipInfoValid = true;
                }
            });
            this.$refs["equipInfoSeniorForm"].validate(valid => {
                if (valid) {
                    equipInfoSeniorValid = true;
                } else {
                    this.activeNames = ["1"];
                }
            });
            this.$refs["equipInfoAlarmForm"].validate(valid => {
                if (valid) {
                    equipInfoAlarmFormValid = true;
                } else {
                    this.activeNames = ["3"];
                }
            });
            this.$refs["equipInfoAlarmSettingForm"].validate(valid => {
                if (valid) {
                    equipInfoAlarmSettingFormValid = true;
                } else {
                    this.activeNames = ["4"];
                }
            });

            this.$refs.customAttribute.validate((valid) => {
                if (valid) {
                    customAttributeValid = true
                } else {
                    this.activeNames = ["2"];
                }
            })

            if (equipInfoValid && equipInfoSeniorValid && customAttributeValid && equipInfoAlarmFormValid && equipInfoAlarmSettingFormValid) {
                // 转化后赋值给alarmScheme
                let equipData = JSON.parse(JSON.stringify(this.equipInfo));
                equipData.alarmScheme = equipUtils.toDecimalSystem(this.alarms);
                equipData.staNo = Number(equipData.staNo);
                equipData.equipNo = Number(equipData.equipNo);
                equipData.rawEquipNo = Number(equipData.rawEquipNo);
                equipData.attrib = Number(equipData.attrib);
                equipData.alarmRiseCycle = Number(equipData.alarmRiseCycle);
                equipData.relatedVideo = equipData.relatedVideo
                    ? this.labelEquip + "|" + equipData.relatedVideo
                    : "";
                this.clearParameter(equipData.parameters)
                if (this.page != 'equipInfo') {
                    delete equipData.enableEquip
                }
                this.saveData(equipData);
            }
        },

        toInteger (item) {
            let reg = /^[0-9]+$/;
            if (this.equipInfo[item]) {
                if (!reg.test(this.equipInfo[item])) {
                    // 用以在dom渲染挂载后重新触发dom渲染挂载
                    this.$nextTick(() => {
                        this.equipInfo[item] = parseInt(this.equipInfo[item]);
                    });
                }
            } else {
                this.$nextTick(() => {
                    this.equipInfo[item] = 0;
                });
            }
        },

        // 批量应用
        bactchSetEquip (key, item) {
            this.showBatSetEquipDialog = true;
            this.selectEquipType = key;

        },
        getSelected () {
            return this.$refs.myTrees.getEquipSelectd();
        },

        // 保存设备批量应用
        saveBatchSetEquip () {
            let targetArr = this.getSelected();
            if (targetArr.length === 0) {
                this.$message.warning(this.$tt("tips.selectADevice"));
                return;
            }
            this.equipSelectedList = targetArr;
            this.loading = true;
            this.$api
                .BatchModifyFromEquip({
                    sourceEquipNo: this.equipInfo.equipNo,
                    targetEquipNos: targetArr
                })
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.showBatSetEquipDialog = false;
                        this.$message.success(
                            this.$tt("tips.applySuccess")
                        );
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.loading = false;
                });
        },


        closeBatchSetEquip () {
            // $('.set-equip .checkInput-active').removeClass('checkInput-active');
            this.selectedAll = false;
            this.newGroupId = "";
            this.currentSelectZiChan = []

        }
    }
};
