const tree = () => import('gw-base-components-plus/treeV2');
const manage = () => import('../components/manage/manage.vue');
const moveEquip = () => import('../components/dialog/moveEquip/index.vue');
const modelNew = () => import('../components/dialog/modelNew/index.vue');
const group = () => import('../components/dialog/group/index.vue');
const deleteGroup = () => import('../components/dialog/deleteGroup/index.vue');
const batMoveEquip = () => import('../components/dialog/batMoveEquip/index.vue');
const batEditEquip = () => import('../components/dialog/batEditEquip/index.vue');
const batEditYc = () => import('../components/dialog/batEditYc/index.vue');
const batEditYx = () => import('../components/dialog/batEditYx/index.vue');
const batEditSet = () => import('../components/dialog/batEditSet/index.vue');
const widthSetting = () => import('gw-base-components-plus/widthSetting/widthSetting.vue');

import equipUtils from '../utils/util.js';
export default {
    components: {
        tree,
        moveEquip,
        modelNew,
        group,
        deleteGroup,
        batMoveEquip,
        batEditEquip,
        batEditYc,
        batEditYx,
        batEditSet,
        widthSetting,
        manage
    },
    data () {
        return {
            headers: {
                Authorization: window.sessionStorage.getItem('accessToken'),
                'Access-Control-Allow-Origin': this.$api.getSignalrHttp()
            },
            action: '/IoT/api/v3/SystemConfig/BatchImportTemplate',

            showSelectGroupDialog: false,
            showDeleteGroup: false,
            deleteGroupInfo: {
                groupName: '',
                groupId: -1,
                deleteChild: false
            },

            eqPage: {
                pageNo: 1,
                pageSize: 25,
                total: 1
            },

            minNum: -2147483647,
            maxNum: 2147483647,

            // 搜索框
            searchEquip: '',
            searchTableEquip: '',

            // 设备列表
            equipList: [],
            haveEquipData: false,

            selectEquipDeleteNo: null,
            selectEquipDeleteNm: null,
            selectStaDeleteNo: null,

            equipLoading: true,

            // 弹出框
            newEquipDialog: false,

            showModelNewDialog: false,
            showBatEditEquipDialog: false,
            showBatEditYcDialog: false,
            showBatEditYxDialog: false,
            showBatEditSetDialog: false,
            showRelateEquipDialog: false,
            selectedGrouId: '',

            // 设备表单
            equipInfo: {
                parameters: [],
                enableEquip: false,
                equipNo: '',
                equipNm: '',
                equipDetail: '',
                ziChanId: '',
                ziChanName: '',
                planNo: '',
                relatedPic: '',
                relatedVideo: '',
                procAdvice: '',
                rawEquipNo: '',
                tabName: '',
                staIp: '',
                attrib: '',
                accCyc: 1,
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
                staNo: 1,
                backup: '',
                groupId: ''
            },

            swit: [
                {
                    value: 1,
                    label: 'dialog.selectYes'
                },
                {
                    value: 0,
                    label: 'dialog.selectNo'
                }
            ],
            swit1: [
                {
                    value: true,
                    label: 'dialog.selectYes'
                },
                {
                    value: false,
                    label: 'dialog.selectNo'
                }
            ],
            videoDropdown: [],
            zcDropdown: [],
            planNoDropdown: [],
            communicationDrvList: [],

            alarmDropdown: [
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
            ],

            parentSeleceed: [],

            // 子组件元素
            saveItem: null,
            saveChildItem: null,

            equipSelected: [{ equipNo: 0 }],
            moveGroups: [], // 移动设备所选分组
            showMoveGroupsDialog: false,
            alarms: {
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },
            batchEquip: true,

            saveLoading: false,
            selecteItem: {}, // 当前选中设备及分组信息

            // 分组新增及编辑
            groupData: {},
            showGroupDialog: false,
            isGroupNew: false,
            containSelected: false,
            moveEquips: [],
            equipObject: {},
            nodeObject: {},
            defaultExpandedKeys: [],
            showAdd: false, // 是否显示“手动添加”功能
            activeStep: 1,

            addForm: {
                type: '',
                groupIndex: ''
            },

            addFileLists: {
                name: ''
            },
            importFile: '',
            waveFilenames: [],
            treeUpdate: false,
            refreshId: '',
            equipNumber: 0,
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
        };
    },
    provide () {
        return {
            getSelected: this.getSelected,
            swit: this.swit,
            swit1: this.swit1,
            alarmDropdown: this.alarmDropdown,
            getVideoDropdown: () => this.videoDropdown,
            getZcDropdown: () => this.zcDropdown,
            getPlanNoDropdown: () => this.planNoDropdown,
            getCommunicationDrvList: () => this.communicationDrvList,

            getEquipList: () => this.equipList,
            getMoveGroups: () => this.moveGroups,
            haveChangeItem: this.haveChangeItem,
            equipTypes: [],
            getWaveFilenames: () => this.waveFilenames,
            setTypeOptions: this.setTypeOptions
        };
    },

    watch: {
        searchEquip (val) {
            if (!val) {
                this.$refs.myTree.filterMethod()
            }
        }
    },

    methods: {

        arraySpanMethod ({ row, column, rowIndex, columnIndex }) {
            if (columnIndex === 0) {
                if (rowIndex % 5 === 0) {
                    return {
                        rowspan: 5,
                        colspan: 1
                    };
                } else {
                    return {
                        rowspan: 0,
                        colspan: 0
                    };
                }
            }
        },







        getTotal (equipNumber) {
            this.equipNumber = equipNumber
        },
        getwaveFilenames () {
            this.$api.getwaveFilenames().then(res => {
                const { code, data } = res?.data || {}
                if (code == 200) {
                    if (data && data instanceof Array) {
                        data.forEach(item => {
                            this.waveFilenames.push({
                                label: item,
                                value: item
                            });
                        });
                    }
                }
            });
        },

        closeAddPanelVisible (isOpen, saveLoading) {
            this.saveLoading = saveLoading;
        },

        addEquips () {
            this.getParentSeleceed()
            this.showModelNewDialog = true
        },

        // 关联设备
        relateEquip () {
            // this.getParentSeleceed()
            this.showRelateEquipDialog = true
        },

        // 获取预案号
        getPlanList () {
            let data = {
                pageNo: 1,
                pageSize: 100,
                planName: ''
            };
            this.$api.getPlanList(data).then(res => {
                const { code, data } = res?.data || {}
                if (code == 200) {
                    let planList = data?.rows || [];
                    planList.forEach(planItem => {
                        if (planItem.planStatus == 1) {
                            const { planName, planId } = planItem;
                            this.planNoDropdown.push({
                                label: planName,
                                value: planId.toString()
                            });
                        }
                    });
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            })
        },

        getParentSeleceed () {
            this.parentSeleceed = []
            if (window.top.groupList_manageMent) {
                window.top.groupList_manageMent.forEach(item => {
                    this.parentSeleceed.push({
                        groupName: item.name,
                        groupId: item.id
                    });
                })
            }
        },

        // 左侧选择设备
        getItem (data) {
            if (!data.isGroup) {
                this.selecteItem = {
                    equipNo: data.key,
                    equipName: data.title,
                    groupName: data.groupName,
                    groupId: data.groupId,
                    staNo: data.staNo
                };
                this.equipSelected = [
                    {
                        equipNo: data.key
                    }
                ];
            }
        },

        closeDialog (type) {
            this[type] = false;
            this.batchEquip = true;
        },

        // 分组编辑和新增
        groupEditAndNew (data) {
            const { isGroupNew, node } = data
            if (node.isGroup) {
                this.groupData = {
                    groupId: node.key,
                    groupName: node.title
                };
                this.showGroupDialog = true;
                this.isGroupNew = isGroupNew;
            }
        },

        // 分组删除
        deleteGroup (data) {
            if (data.isGroup) {
                this.deleteGroupInfo.groupName = data.title;
                this.deleteGroupInfo.groupId = data.key;
                this.deleteGroupInfo.deleteChild = false;
                this.showDeleteGroup = true;
                this.containSelected = true;
            }
        },


        // 当前分组存在当前选中设备的情况（回调函数：改变equipNo，用于删除右侧相关设备信息）
        deleteGroupCallBack (deleteChild) {
            if (deleteChild && this.containSelected) {
                this.selecteItem.equipNo = 0;
                this.selecteItem.equipName = '';
            }
        },

        moveEquip () {
            this.moveGroups = [];
            this.moveGroups = JSON.parse(JSON.stringify(this.parentSeleceed));
            this.showMoveGroupsDialog = true;
        },

        confirmMoveEquip (groupId) {
            this.saveLoading = true;
            let data = {
                equipNoList: [this.selecteItem.equipNo],
                newGroupId: groupId
            };
            this.$api
                .moveToNewGroup(data)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code === 200) {
                        this.$message.success(
                            this.$t('equipInfo.tips.moveSuccess')
                        );
                        this.showMoveGroupsDialog = false;
                        this.selecteItem.groupName = this.getGroupName(groupId);
                        this.selecteItem.groupId = groupId;
                        this.equipSelected = [
                            { equipNo: this.selecteItem.equipNo }
                        ];
                    } else {
                        this.$message.error(message)
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err)

                }).finally(() => {
                    this.saveLoading = false;
                });
        },

        // 批量删除
        batchDelEquip () {
            let selects = this.getSelected();
            if (selects.length == 0) {
                this.$message.warning(this.$t('equipInfo.tips.selectADevice'));
            } else {
                this.$msgbox({
                    title: this.$t('equipInfo.tips.tip'),
                    message: this.$t(
                        'equipInfo.tips.sureToDeleteSelectedDevices'
                    ),
                    dangerouslyUseHTMLString: true,
                    showCancelButton: true,
                    type: "warning",
                    confirmButtonText: this.$t(
                        'equipInfo.publics.button.confirm'
                    ),
                    cancelButtonText: this.$t(
                        'equipInfo.publics.button.cancel'
                    ),
                    beforeClose: (action, instance, done) => {
                        if (action === 'confirm') {
                            instance.confirmButtonLoading = true;
                            setTimeout(() => {
                                this.$api
                                    .batchDelEquip({
                                        staN: 1,
                                        ids: selects
                                    })
                                    .then(res => {
                                        const { code, message } = res?.data || {}
                                        if (code === 200) {
                                            this.$message.success(
                                                this.$t(
                                                    'equipInfo.publics.tips.deleteSuccess'
                                                )
                                            );
                                            if (this.searchEquip) {
                                                this.searchEquipName = ''
                                                this.searchEquip = ''
                                            }
                                            this.isIndeterminate = false;
                                            if (
                                                selects.indexOf(
                                                    this.selecteItem.equipNo
                                                ) != -1
                                            ) {
                                                this.selecteItem = {
                                                    equipNo: 0
                                                };
                                            }
                                        } else {
                                            this.$message.error(message);
                                        }
                                    })
                                    .catch((err) => {
                                        this.$message.error(err?.data, err);
                                    }).finally(() => {
                                        instance.confirmButtonLoading = false;
                                        done();
                                    });
                            }, 120);
                        } else {
                            done();
                        }
                    }
                })
                    .then()
                    .catch(() => {

                        // console.log(err);
                    });
            }
        },

        // 批量参数
        batchEditEquip () {
            let equipNos = this.getSelected();
            if (equipNos.length === 0) {
                this.$message.warning(this.$t('equipInfo.tips.selectADevice'));
            } else {
                this.getCommunicationDrv();
                this.showBatEditEquipDialog = true;
            }
        },

        // 导出文件
        exportFile () {
            let equipNos = this.getSelected();
            if (equipNos.length === 0) {
                this.$message.warning(this.$t('equipInfo.tips.selectADevice'));
            } else {
                this.$api
                    .exportEquip(equipNos)
                    .then(res => {
                        if (res?.status === 200 && res?.data) {
                            let date = new Date();
                            let dateStr = `${date.getFullYear()}-${date.getMonth() +
                                1}-${date.getDate()}`;
                            this.myUtils.download(res.data || '', `${dateStr}-${this.$t('equipInfo.tips.divice')}.xlsx`)
                        } else {
                            this.$message.error(
                                this.$t('equipInfo.publics.tips.exportFail'),
                                res
                            );
                        }
                    })
                    .catch(err => {
                        this.$message.error(
                            err?.data,
                            err
                        );
                    });
            }
        },

        getSelected () {
            return this.$refs.myTree.getEquipSelectd();
            // return this.$refs.myTree.getChecked().equips;
        },


        // 搜索设备
        changeEquipList () {
            this.searchEquip = this.searchEquip.trim();
            this.$refs.myTree.filterMethod(this.searchEquip)
        },

        yxVideoChange (val) {
            this.yxInfo.relatedVideoName = val.channelName;
            this.yxInfo.relatedVideo = val.id;
        },

        //  获取动态库文件
        getCommunicationDrv () {
            this.$api
                .getCommunicationDrv()
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.communicationDrvList = data || [];
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch(err => {
                    this.$message.error(err?.data, err);
                });
        },

        // 列表选中
        isCheckList (dialogName) {
            let equipNos = this.getSelected();
            if (equipNos.length === 0) {
                this.$message.warning(this.$t('equipInfo.tips.selectADevice'));
            } else {
                this[dialogName] = true;
            }
        },

        // 批量遥测
        batchEditYc () {
            this.isCheckList('showBatEditYcDialog');
        },

        // 批量遥信
        batchEditYx () {
            this.isCheckList('showBatEditYxDialog');
        },

        // 批量设置
        batchEditSet () {
            this.isCheckList('showBatEditSetDialog');
        },

        // 获取视频可关联项
        getVideoList () {
            this.$api
                .getVideoAllInfo({ equip_no: 100 })
                .then(res => {
                    const { code, data, message } = res?.data || {}
                    if (code === 200) {
                        this.videoDropdown = data || [];
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch(err => {
                    this.$message.error(err.data, err);
                    console.log(err);
                });
        },

        getGroupName (groupId) {
            let groupName = '';
            this.parentSeleceed.forEach(item => {
                if (item.groupId === groupId) {
                    groupName = item.groupName;
                }
            });
            return groupName;
        },

        // 批量移动
        moveClick () {
            let equipNos = this.getSelected();
            if (equipNos.length === 0) {
                this.$message.warning(this.$t('equipInfo.tips.selectADevice'));
                return;
            }
            this.getParentSeleceed()
            this.moveEquips = equipNos;
            this.showSelectGroupDialog = true;
        },

        // 保存修改信息到接口
        saveData (formName, apiName, dialogName, batYcYx) {
            formName.ids = this.getSelected();
            let data = JSON.stringify(formName);

            this.saveLoading = true;

            // apiName为接口名称
            this.$api[apiName](data)
                .then(res => {
                    const { code, message } = res?.data || {}
                    if (code == 200) {
                        this.$message.success(
                            this.$t('equipInfo.publics.tips.saveSuccess')
                        );
                        this[dialogName] = false;
                    } else {
                        this.$message.error(message);
                    }
                    if (
                        batYcYx &&
                        this.selecteItem.equipNo &&
                        this.selecteItem.equipNo !== 0
                    ) {
                        this.$refs.YcYxSet.getItemData();
                    }
                })
                .catch(er => {
                    this.$message.error(er?.data, er);
                }).finally(() => {
                    this.saveLoading = false;
                });
        },

        haveChangeItem (list, form, alarms, labelEquip, isYcYx) {
            if (list.length === 0) {
                this.$message.warning(this.$t('equipInfo.tips.noSelectItem'));
                return false;
            }

            // 获取选中当前修改项
            let obj = {};
            let isVerificationOk = true;
            for (let i in form) {
                list.map(item => {
                    if (item === i) {
                        if (item === 'alarmScheme') {
                            obj[i] = equipUtils.toDecimalSystem(alarms);
                            if (isYcYx) {
                                obj.isGenerateWO = form.isGenerateWO;
                            }
                        } else if (item === 'relatedVideo') {
                            obj[i] = form[i] ? labelEquip + '|' + form[i] : '';
                        } else if (item === 'safeTime') {
                            let isVer = equipUtils.verificationSateTime(
                                form[i]
                            );
                            if (!isVer.pass) {
                                this.$message.warning(this.$t(isVer.warning));
                                isVerificationOk = false;
                            } else {
                                obj[i] = form[i];
                            }
                        } else if (item === 'alarmShield') {
                            if (form[i]) {
                                let reg = /^\d+$/

                                let alarmItems = form[i].split('+')
                                alarmItems.forEach(item => {
                                    let strs = item.split(',')
                                    if (strs.length != 2 || !reg.test(strs[0]) || !reg.test(strs[1])) {
                                        isVerificationOk = false
                                    }
                                })
                                if (!isVerificationOk) {
                                    this.$message.warning(
                                        this.$t('equipInfo.tips.alarmBlockTips')
                                    );
                                } else {
                                    obj[i] = form[i];
                                }
                            } else {
                                obj[i] = form[i];
                            }

                        } else if (item == 'dataType') {
                            obj[i] = form[i] ? form[i] : null
                        } else {
                            obj[i] = form[i];
                        }
                    }
                });
            }
            if (isVerificationOk) {
                return {
                    staN: 1,
                    dicts: obj
                };
            }
            return false;
        }

    },

};
