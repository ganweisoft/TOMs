import equipDialog from '../components/dialog/equip/index.vue';
import yc from '../components/dialog/yc/index.vue';
import yx from '../components/dialog/yx/index.vue';
import set from '../components/dialog/set/index.vue';
import batchExport from '../components/dialog/export/index.vue';
// 防抖
function debounce (func, wait = 1000) {
    let timeout;
    return function (event) {
        clearTimeout(timeout);
        timeout = setTimeout(() => {
            func.call(this, event);
        }, wait);
    };
}
export default {
    components: { equipDialog, yc, yx, set, batchExport },
    data () {
        return {
            scrollWidth: false,
            headers: {
                'Access-Control-Allow-Origin': this.$api.getSignalrHttp(),
            },
            fileList: [],
            minNum: -2147483647,
            maxNum: 2147483647,

            // 搜索框
            searchEquip: '',
            searchTableEquip: '',

            // 选中的标签页
            activeYcYxSet: 'first',

            // 设备列表
            nowEquipList: [],

            // 当前选中的设备信息
            selectEquipNo: '',
            selectStaNo: null,
            selectEquipNm: '',

            selectEquipDeleteNo: null,
            selectEquipDeleteNm: null,
            selectStaDeleteNo: null,

            // 当前是新增或者是编辑
            isEquipNew: false,
            isYcNew: false,
            isYxNew: false,
            isSetNew: false,

            showYcDialog: false,
            showYxDialog: false,
            showSetDialog: false,

            // 删除哪个（设备、yc、yx、设置）
            deleteOperate: '',

            // 选中设备ycyxset数量
            ycNum: '0',
            yxNum: '0',
            setNum: '0',
            ycName: 'templateManage.tab.yc',
            yxName: 'templateManage.tab.yx',
            setName: 'templateManage.tab.set',
            productAttrName: 'templateManage.tab.productAttr',

            // 表格数据
            ycTable: [],
            yxTable: [],
            setTable: [],

            // 表格数据条数
            ycTotals: 0,
            yxTotals: 0,
            setTotals: 0,

            equipLoading: false,

            saveLoading: false,

            delEquipLoading: false,

            pageSizeEquip: 20,
            currentPageEquip: 1,
            totalPageEquip: 0,

            pageSize: 20,
            pageNo: 1,
            totalPage: 0,

            // 弹出框
            equipDialog: false,
            ycDialog: false,
            yxDialog: false,
            setDialog: false,
            newEquipDialog: false,
            newYcDialog: false,
            newYxDialog: false,
            newSetDialog: false,

            showEquipDialog: false,
            copyObject: {},

            // 设备表单
            templateManage: {
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
                equipconntype: 0
            },

            equipAlarm: {
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },

            // yc表单
            ycInfo: {
                parameters: [],
                equipNo: '',
                expression: '',
                ycNo: 1,
                ycNm: '',
                unit: '',
                valTrait: 0,
                ziChanId: '',
                ziChanName: '',
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
                alarmScheme: 0,
                procAdvice: '',
                waveFile: '',
                alarmShield: '',
                alarmRiseCycle: '',
                safeBgn: '',
                safeEnd: '',
                safeTime: '',
                staNo: 1,
                relatedVideoName: '',
                isGenerateWO: false,
                ycCode: '',
                dataType: null
            },

            yxInfo: {
                expression: '',
                parameters: [],
                equipNo: '',
                yxNo: 1,
                yxNm: '',
                ziChanId: '',
                ziChanName: '',
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
                alarmScheme: 0,
                waveFile: '',
                alarmRiseCycle: '',
                alarmShield: '',
                staNo: 1,
                relatedVideoName: '',
                isGenerateWO: false,
                yxCode: '',
                dataType: null
            },

            yxAlarm: {
                emailAlarm: 0,
                messageAlarm: 0,
                isMarkAlarm: 0,
                isAlarm: 0
            },

            setInfo: {
                parameters: [],
                equipNo: '',
                setNo: 1,
                setNm: '',
                setType: '',
                mainInstruction: '',
                minorInstruction: '',
                record: false,
                action: '',
                value: '',
                voiceKeys: '',
                enableVoice: false,
                canexecution: 0,
                staNo: 0,
                setCode: ''
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
            activeNames: [],

            action: `${this.$api.getSignalrHttp()}/IoT/api/v3/SystemConfig/BatchImportTemplate`,
            selectItem: {},
            activeTabName: 'templateManage.tab.yc',
            searchActiveTabName: 'templateManage.input.searchYc',

            searchStatus: false,
            exportLoading: false,
            dataLoading: false,

            // 产品属性
            hasproductAttr: false,
            attrId: '',
            productAttrInfo: {
                attr: [
                    {
                        key: 'productName',
                        name: 'templateManage.productAttr.productName',
                        value: ''
                    },
                    {
                        key: 'productType',
                        name: 'templateManage.productAttr.productType',
                        value: ''
                    },
                    {
                        key: 'tradeName',
                        name: 'templateManage.productAttr.tradeName',
                        value: ''
                    },
                    {
                        key: 'protocolType',
                        name: 'templateManage.productAttr.protocolType',
                        value: ''
                    },
                    {
                        key: 'softwareVersion',
                        name: 'templateManage.productAttr.softwareVersion',
                        value: ''
                    },
                    {
                        key: 'hardwareVersion',
                        name: 'templateManage.productAttr.hardwareVersion',
                        value: ''
                    },
                    {
                        key: 'appearanceProp',
                        name: 'templateManage.productAttr.appearanceProp',
                        value: ''
                    },
                    {
                        key: 'objectModelInfo',
                        name: 'templateManage.productAttr.objectModelInfo',
                        value: ''
                    }
                ]
            },
            equipTypes: [
                {
                    value: 0,
                    label: 'equipTypes.direct'
                },
                {
                    value: 1,
                    label: 'equipTypes.nonDirect'
                },
                {
                    value: 2,
                    label: 'equipTypes.virtual'
                }
            ],
            tableHeight: null,
            uploadForm: [],
            waveFilenames: [],
            setTypeOptions: [
                {
                    label: 'templateManage.setTypeOptions.parameterSettings',
                    value: 'V'
                },
                {
                    label: 'templateManage.setTypeOptions.signalingSettings',
                    value: 'X'
                },
                {
                    label: 'templateManage.setTypeOptions.telemetrySettings',
                    value: 'C'
                },
                {
                    label: 'templateManage.setTypeOptions.systemSettings',
                    value: 'S'
                },
                {
                    label: 'templateManage.setTypeOptions.sceneSettings',
                    value: 'J'
                },
            ]
        };
    },

    watch: {
        searchEquip (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.changeEquipList(null, true)
            }
        },
        searchTableEquip (val, oldValue) {
            if (!val && oldValue.trim()) {
                this.changeTableList(null, true)
            }
        },
        nowEquipList (newVal) {
            this.$nextTick(() => {
                let equipListMainHeight = document.getElementById("equipListMain").offsetHeight
                let equipContainerHeight = document.getElementById("equipContainer").offsetHeight
                if (equipContainerHeight >= equipListMainHeight) {
                    this.scrollWidth = true
                } else {
                    this.scrollWidth = false
                }

            })
        }
    },

    mounted () {
        let that = this;
        let eqTable = document.getElementById('eqTable');
        if (eqTable) {
            that.tableHeight = eqTable.offsetHeight - 15;
        }
        window.onresize = function windowResize () {
            eqTable = document.getElementById('eqTable');
            if (eqTable) {
                that.tableHeight = eqTable.offsetHeight - 15;
            }
        };

        this.getModelList();

        // this.getwaveFilenames()

    },

    provide () {
        return {
            swit: this.swit,
            swit1: this.swit1,
            alarmDropdown: this.alarmDropdown,
            getVideoDropdown: () => this.videoDropdown,
            getZcDropdown: () => this.zcDropdown,
            getPlanNoDropdown: () => this.planNoDropdown,
            getCommunicationDrvList: () => this.communicationDrvList,
            getEquipList: () => [],
            getMoveGroups: () => { },
            haveChangeItem: () => { },
            getWaveFilenames: () => this.waveFilenames,
            equipTypes: this.equipTypes
        }
    },
    methods: {
        copyEquip (item) {
            const data = {
                equipNo: item.equipNo
            }
            this.$api.copyEquip(data).then(res => {
                if (res.data.code === 200) {
                    this.$message.success(res.data.message);
                    this.getModelList()
                } else {
                    this.$message.error(res.data.message, res);
                }
            })
        },

        copyYcp (item) {
            const data = {
                equipNo: item.equipNo,
                no: item.ycNo
            }
            this.$api.copyYcp(data).then(res => {
                if (res.data.code === 200) {
                    this.$message.success(res.data.message);
                    this.activeYcYxSetList()
                } else {
                    this.$message.error(res.data.message, res);
                }
            })
        },

        copyYxp (item) {
            const data = {
                equipNo: item.equipNo,
                no: item.yxNo
            }
            this.$api.copyYxp(data).then(res => {
                if (res.data.code === 200) {
                    this.$message.success(res.data.message);
                    this.activeYcYxSetList()
                } else {
                    this.$message.error(res.data.message, res);
                }
            })
        },

        copySetparm (item) {
            const data = {
                equipNo: item.equipNo,
                no: item.setNo
            }
            this.$api.copySetparm(data).then(res => {
                if (res.data.code === 200) {
                    this.$message.success(res.data.message);
                    this.activeYcYxSetList()
                } else {
                    this.$message.error(res.data.message, res);
                }
            })
        },

        getSetTypeName (value) {
            let name = ''
            this.setTypeOptions.forEach(item => {
                if (item.value == value) {
                    name = item.label
                }
            })
            return name
        },
        getModelList (isChangeSel) {
            let obj = {};
            if (isChangeSel == 'del') {
                if (this.nowEquipList.length == 1 && this.currentPageEquip != 1) {
                    this.currentPageEquip--;
                }
            }
            obj.pageNo = this.currentPageEquip;
            obj.pageSize = this.pageSizeEquip;
            obj.equipName = this.searchEquip;
            this.equipLoading = true;

            // 获取设备列表
            this.$api
                .getModelEquipTree(obj)
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.nowEquipList = data?.rows || [];
                        if (this.nowEquipList instanceof Array) {
                            this.nowEquipList.forEach(item => {
                                item.staNo = item.staN
                                if (item.relatedVideo) {
                                    item.relatedVideo = parseInt(item.relatedVideo.split('|')[1]);
                                }
                            })
                        }
                        this.totalPageEquip = data?.total || 0;
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.equipLoading = false;
                });
        },
        getwaveFilenames () {
            this.$api.getwaveFilenames().then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    if (data && data instanceof Array) {
                        data.forEach(item => {
                            this.waveFilenames.push({
                                label: item,
                                value: item
                            })
                        })
                    } else {
                        this.$message.error(message);
                    }
                }
            }).catch((err) => {
                this.$message.error(err?.data, err);
                console.log(err);
            });
        },
        fileListChange (file, fileList) {
            const isLt2M = file.size / 1024 / 1024 < 2;
            if (!isLt2M) {
                return this.$message.error(this.$t('templateManage.uploads.limitSize', ['2MB']));
            }
            let formData = new FormData();
            formData.append('file', file.raw);
            this.$api.uploadTemplate(formData).then((res) => {
                const { code, message } = res?.data || {}
                if (code === 200) {
                    this.$message.success(this.$t('templateManage.publics.tips.importSuccess'));
                    this.uploadForm = []
                    this.getModelList();
                } else {
                    this.$message.error(message);
                }
            }).catch(er => {
                this.$message.error(er?.data, er);
            })
        },
        exportFile () {
            if (this.nowEquipList && this.nowEquipList.length > 0) {
                this.exportLoading = true;
                this.$api.DownloadTemplateFile().then(res => {
                    if (res?.status === 200) {
                        this.myUtils.download(res?.data || '', 'Template' + '.xlsx')
                    } else {
                        this.$message.error(this.$t('templateManage.publics.tips.exportFail'));
                    }
                    this.exportLoading = false
                }).catch(er => {
                    this.$message.error(er?.data, er);
                })
            } else {
                this.$message.warning(this.$t('equipInfo.tips.noDataToExport'));
            }
        },
        ycVideoChange (val) {
            this.ycInfo.relatedVideoName = val.channelName;
            this.ycInfo.relatedVideo = val.id;
        },

        yxVideoChange (val) {
            this.yxInfo.relatedVideoName = val.channelName;
            this.yxInfo.relatedVideo = val.id;
        },

        // 获取预案号
        getPlanList () {
            let data = {
                pageNo: 1,
                pageSize: 100,
                planName: ''
            };
            this.$api.getPlanList(data).then((res) => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    let planList = data?.rows || [];
                    planList.forEach(planItem => {
                        if (planItem.planStatus == 1) {
                            const { planName, planId } = planItem;
                            this.planNoDropdown.push({ label: planName, value: planId.toString() });
                        }
                    })
                } else {
                    this.$message.error(message);
                }
            }).catch(er => {
                this.$message.error(er?.data, er);
                console.log(er);
            })
        },

        // 匹配视频名称
        getVideoName (key) {
            let name = '';
            this.videoDropdown.forEach((item) => {
                if (item.id === key) {
                    name = item.channelName;
                }
            });
            return name;
        },

        // 获取视频可关联项
        getVideoList () {
            this.$api
                .getVideoAllInfo({ equip_no: 100 })
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code === 200) {
                        let dataList = data || [];
                        if (dataList instanceof Array) {
                            this.videoDropdown = dataList;
                        }
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                });
        },


        // 获取指定设备项信息
        getEquipListItem (equipNo) {
            let item = null;
            if (equipNo === '') {
                item = this.nowEquipList[0];
            } else {
                for (let i = 0; i < this.nowEquipList.length; i++) {
                    if (equipNo === this.nowEquipList[i].equipNo) {
                        item = this.nowEquipList[i];
                        break;
                    }
                }
            }
            return item;
        },

        getCommunicationDrv () {
            this.$api
                .getCommunicationDrv()
                .then((res) => {
                    const { code, data, message } = res?.data || {}
                    if (code == 200) {
                        this.communicationDrvList = data || [];
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                });
        },

        // 搜索设备
        changeEquipList (e, change) {
            this.searchEquip = this.searchEquip.trim()
            if (this.searchEquip || change) {
                this.currentPageEquip = 1;
                this.equipListApi();
            }

        },

        equipListApi: debounce(function (e) {
            this.getModelList('change');
        }, 10),

        // 搜索ycyx设置
        changeTableList (e, change) {
            this.searchTableEquip = this.searchTableEquip.trim()
            this.searchStatus = !!this.searchTableEquip;

            if (this.searchStatus || change) {
                if (this.searchStatus) {
                    this.pageNo = 1
                }
                this.ycYxSetApi();
            }

        },

        ycYxSetApi: debounce(function (e) {
            this.activeYcYxSetList();
        }, 10),

        tabClick (index) {
            this.searchTableEquip = '';
            this.searchStatus = false;
            if (this.selectEquipNo == '') {
                this.$message.warning(this.$t('templateManage.tips.selectADevice'));
            };
            if (index == 1) {
                this.activeYcYxSet = 'first'
            }
            if (index == 2) {
                this.activeYcYxSet = 'second'
            }
            if (index == 3) {
                this.activeYcYxSet = 'third'
            }
            if (index == 4) {
                this.activeYcYxSet = 'four'
            }
            if (this.activeYcYxSet == 'first') {
                this.activeTabName = 'templateManage.tab.yc';
                this.searchActiveTabName = 'templateManage.input.searchYc'
            } else if (this.activeYcYxSet == 'second') {
                this.activeTabName = 'templateManage.tab.yx'
                this.searchActiveTabName = 'templateManage.input.searchYx'
            } else if (this.activeYcYxSet == 'third') {
                this.activeTabName = 'templateManage.tab.set'
                this.searchActiveTabName = 'templateManage.input.searchSet'
            }
            this.pageNo = 1;

            this.activeYcYxSetList();
        },

        // 判断当前是yc、yx还是设置表
        activeYcYxSetList () {
            let data = {
                equipNo: Number(this.selectEquipNo),
                pageSize: this.pageSize,
                pageNo: this.pageNo,
                searchName: this.searchTableEquip
            };
            this.dataLoading = true
            if (this.activeYcYxSet == 'first') {
                this.$api
                    .getModelEquipYCPConf(data)
                    .then((res) => {
                        const { code, data, message } = res?.data || {}
                        if (code == 200) {
                            let rows = data?.rows || [];
                            for (let i = 0; i < rows.length; i++) {
                                rows[i].staNo = rows[i].staN

                                if (rows[i].relatedVideo) {
                                    rows[i].relatedVideoName = this.getVideoName(
                                        parseInt(rows[i].relatedVideo.split('|')[1])
                                    )
                                    rows[i].relatedVideo = parseInt(rows[i].relatedVideo.split('|')[1])
                                }

                                if (rows[i].curveRcd) {
                                    rows[i].curveRcdStr = 'templateManage.dialog.selectYes';
                                } else {
                                    rows[i].curveRcdStr = 'templateManage.dialog.selectNo';
                                }
                            }

                            this.ycTable = rows;

                            this.ycTotals = data?.totalCount || 0;
                        } else {
                            this.ycTable = [];
                            this.ycTotals = 0;
                            this.$message.error(message);
                        }

                        // 赋值总页数
                        if (!this.searchStatus) {
                            this.ycNum = this.ycTotals;
                        }

                        this.totalPage = this.ycTotals;
                        this.dataLoading = false
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);
                        this.dataLoading = false
                    });
            } else if (this.activeYcYxSet == 'second') {
                this.$api
                    .getModelEquipYXPConf(data)
                    .then((res) => {
                        const { code, data, message } = res?.data || {}
                        if (code == 200) {
                            let rows = data?.rows || [];
                            for (let i = 0; i < rows.length; i++) {
                                rows[i].staNo = rows[i].staN
                                if (rows[i].relatedVideo) {
                                    rows[i].relatedVideoName = this.getVideoName(
                                        parseInt(rows[i].relatedVideo.split('|')[1])
                                    )
                                    rows[i].relatedVideo = parseInt(rows[i].relatedVideo.split('|')[1])
                                }

                                if (rows[i].curveRcd) {
                                    rows[i].curveRcdStr = 'templateManage.dialog.selectYes';
                                } else {
                                    rows[i].curveRcdStr = 'templateManage.dialog.selectNo';
                                }
                            }
                            this.yxTable = rows;

                            this.yxTotals = data?.totalCount || 0;
                        } else {
                            this.$message.error(message);
                            this.yxTable = [];
                            this.yxTotals = 0;
                        }
                        if (!this.searchStatus) {
                            this.yxNum = this.yxTotals;
                        }

                        this.totalPage = this.yxTotals;
                        this.dataLoading = false
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);
                        this.dataLoading = false
                        console.log(err);
                    });
            } else if (this.activeYcYxSet == 'third') {
                this.$api
                    .getModelEquipSetParmConf(data)
                    .then((res) => {
                        const { code, data, message } = res?.data || {}
                        if (code == 200) {
                            let rows = data?.rows || [];
                            if (rows.length > 0) {
                                rows.forEach((item, i) => {
                                    rows[i].staNo = rows[i].staN
                                    if (item.record) {
                                        item.recordStr = 'templateManage.dialog.selectYes';
                                    } else {
                                        item.recordStr = 'templateManage.dialog.selectNo';
                                    }
                                })
                            }
                            this.setTable = rows;
                            this.setTotals = data?.totalCount || 0;

                        } else {
                            this.$message.error(message);
                            this.setTable = [];
                            this.setTotals = 0;
                        }
                        if (!this.searchStatus) {
                            this.setNum = this.setTotals;
                        }

                        this.totalPage = this.setTotals;
                        this.dataLoading = false
                    })
                    .catch((err) => {
                        this.$message.error(err.data, err);
                        this.dataLoading = false
                    });
            } else if (this.activeYcYxSet == 'four') {
                this.getProductProperty();
            }
        },

        select (value) {
            this.pageNo = 1;
            this.changeRadio(value)
        },

        // 选中设备
        changeRadio (value) {
            this.selectItem = value;
            this.selectEquipNo = value.equipNo;
            this.searchTableEquip = '';
            this.selectStaNo = value.staNo;
            this.selectEquipNm = value.equipNm;

            this.ycTable = [];
            this.yxTable = [];
            this.setTable = [];

            this.dataLoading = true
            if (!value.equipNo) {
                this.ycNum = '0';
                this.yxNum = '0';
                this.setNum = '0';
                this.totalPage = 0;
                this.dataLoading = false
                return;
            }

            this.getModelYcYxSetNum(this.selectEquipNo)
            this.activeYcYxSetList();

            // 获取产品属性
            // this.getProductProperty();

            if (this.$refs['productAttrInfoForm']) {
                this.$refs['productAttrInfoForm'].resetFields();
            }

        },

        getModelYcYxSetNum (equipNo) {
            this.$api.getModelYcYxSetNum({ equipNo }).then(res => {
                const { code, data, message } = res?.data || {}
                if (code == 200) {
                    const { ycNum, yxNum, setNum } = data || {}
                    this.setNum = setNum || 0
                    this.ycNum = ycNum || 0
                    this.yxNum = yxNum || 0
                } else {
                    this.$message.error(message)
                }
            }).catch(err => {
                this.$message.error(err?.data, err)
            })
        },

        newequip () {
            this.copyObject = JSON.parse(JSON.stringify(this.templateManage));
            this.getCommunicationDrv();
            this.getVideoList()
            this.isEquipNew = true;
            this.showEquipDialog = true;
        },

        newYc () {
            this.isYcNew = true;
            if (!this.selectEquipNo) {
                this.$message.warning(this.$t('templateManage.tips.selectADevice'));
                return
            }
            this.getVideoList()
            this.copyObject = JSON.parse(JSON.stringify(this.ycInfo));
            this.copyObject.equipNo = this.selectEquipNo;
            this.copyObject.staNo = this.selectStaNo;
            this.showYcDialog = true;
        },

        newYx () {
            this.isYxNew = true;
            if (!this.selectEquipNo) {
                this.$message.warning(this.$t('templateManage.tips.selectADevice'));
                return
            }
            this.getVideoList()
            this.copyObject = JSON.parse(JSON.stringify(this.yxInfo));
            this.copyObject.equipNo = this.selectEquipNo;
            this.copyObject.staNo = this.selectStaNo;
            this.showYxDialog = true;
        },

        newSet () {
            if (!this.selectEquipNo) {
                this.$message.warning(this.$t('templateManage.tips.selectADevice'));
                return
            }
            this.isSetNew = true;

            this.copyObject = JSON.parse(JSON.stringify(this.setInfo));
            this.copyObject.equipNo = this.selectEquipNo;

            this.copyObject.staNo = this.selectStaNo
            this.showSetDialog = true;
        },

        // 编辑设备框
        editEquip (value) {
            this.isEquipNew = false;
            this.getCommunicationDrv();
            this.getVideoList()

            this.copyObject = JSON.parse(JSON.stringify(this.templateManage));
            this.copyObject.tabName = ''
            for (let i in this.copyObject) {
                this.copyObject[i] = value[i];
            }

            this.showEquipDialog = true;
        },

        closeDialog (dialog) {
            this[dialog] = false
        },

        // 编辑遥测
        editYc (row) {
            this.isYcNew = false;
            this.getVideoList()
            this.copyObject = JSON.parse(JSON.stringify(this.ycInfo))
            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showYcDialog = true;
        },

        // 编辑遥信
        editYx (row) {
            this.isYxNew = false;
            this.getVideoList()
            this.copyObject = JSON.parse(JSON.stringify(this.yxInfo))
            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showYxDialog = true;
        },

        // 编辑设置
        editSet (row) {
            this.isSetNew = false;
            this.copyObject = JSON.parse(JSON.stringify(this.setInfo))
            for (let i in this.copyObject) {
                if (row[i] != null && row[i] != undefined) {
                    this.copyObject[i] = row[i];
                }
            }
            this.showSetDialog = true
        },

        // 保存修改信息到接口
        saveYcYxSet (formName, apiName, dialogName) {
            let data = JSON.stringify(formName);
            this.saveLoading = true;

            // apiName为接口名称
            this.$api[apiName](data)
                .then((res) => {
                    const { code, message } = res?.data || {}
                    if (code == 200) {
                        this.changeRadio(this.selectItem);
                        this.$message.success(this.$t('templateManage.publics.tips.saveSuccess'));
                        this[dialogName] = false;
                    } else {
                        this.$message.error(message);
                    }
                })
                .catch((err) => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.saveLoading = false;
                });
        },

        // 删除设备
        deleteEquip (item) {
            this.selectEquipDeleteNo = item.equipNo;
            this.selectStaDeleteNo = item.staNo;
            this.selectEquipDeleteNm = item.equipNm;
            this.deleteOperate = 'equip';
            this.deleteDialog();
        },

        // 删除弹窗
        deleteDialog () {
            this.$msgbox({
                title: this.$t('templateManage.tips.tip'),
                message: this.$t('templateManage.tips.sureToDelete') + `：${this.selectEquipDeleteNm}`,
                showCancelButton: true,
                type: 'warning',
                confirmButtonText: this.$t('templateManage.publics.button.confirm'),
                cancelButtonText: this.$t('templateManage.publics.button.cancel'),
                beforeClose: (action, instance, done) => {
                    if (action === 'confirm') {
                        instance.confirmButtonLoading = true;
                        setTimeout(() => {
                            this.confirmDelete(() => {
                                instance.confirmButtonLoading = false;
                                done();
                            });
                        }, 120);
                    } else { done() }
                }
            }).then().catch(() => {

            })
        },

        deleteYc (data) {
            this.deleteOperate = 'yc';
            this.selectEquipDeleteNm = data.ycNm;
            this.copyObject = JSON.parse(JSON.stringify(data))
            this.deleteDialog();
        },

        deleteYx (data) {
            this.deleteOperate = 'yx';
            this.selectEquipDeleteNm = data.yxNm;
            this.copyObject = JSON.parse(JSON.stringify(data))
            this.deleteDialog();
        },

        deleteSet (data) {
            this.deleteOperate = 'set';
            this.selectEquipDeleteNm = data.setNm;
            this.copyObject = JSON.parse(JSON.stringify(data))
            this.deleteDialog();
        },

        // 确定删除
        confirmDelete (cb) {
            if (this.delEquipLoading) {
                return;
            }
            this.delEquipLoading = true;

            // 判断当前删除的是设备、yc、yx还是设置
            if (this.deleteOperate == 'equip') {
                this.$api
                    .deleteModelEquip({
                        staNo: this.selectStaDeleteNo,
                        equipNo: Number(this.selectEquipDeleteNo)
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.deleteSuccess'))
                            // 修改后重新获取列表
                            this.getModelList('del');
                            this.ycTable = [];
                            this.yxTable = [];
                            this.setTable = [];
                            this.selectItem = '';
                            this.selectEquipNo = '';
                            this.selectStaNo = '';
                            this.selectEquipNm = '';
                            this.ycNum = '0';
                            this.yxNum = '0';
                            this.setNum = '0';
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);
                    }).finally(() => {
                        this.delEquipLoading = false;
                        cb()
                    });
            } else if (this.deleteOperate == 'yc') {
                this.$api
                    .deleteModelYc({
                        staNo: this.copyObject.staNo,
                        equipNo: Number(this.copyObject.equipNo),
                        ycYxSetNo: this.copyObject.ycNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.deleteSuccess'))

                            // 修改后重新获取列表
                            this.tabClick('', '');
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);

                    }).finally(() => {
                        cb()
                        this.delEquipLoading = false;
                    });
            } else if (this.deleteOperate == 'yx') {
                this.$api
                    .deleteModelYx({
                        staNo: this.copyObject.staNo,
                        equipNo: Number(this.copyObject.equipNo),
                        ycYxSetNo: this.copyObject.yxNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.deleteSuccess'))
                            // 修改后重新获取列表
                            this.tabClick('', '');
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);

                    }).finally(() => {
                        cb()
                        this.delEquipLoading = false;
                    });
            } else if (this.deleteOperate == 'set') {
                this.$api
                    .deleteModelSet({
                        staNo: this.copyObject.staNo,
                        equipNo: Number(this.copyObject.equipNo),
                        ycYxSetNo: this.copyObject.setNo
                    })
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.deleteSuccess'))
                            // 修改后重新获取列表
                            this.tabClick('', '');
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {
                        this.$message.error(err?.data, err);

                    }).finally(() => {
                        cb()
                        this.delEquipLoading = false;
                    });
            }
        },

        // 设备页码改变事件
        equipCurrentChange (pageNo) {
            this.currentPageEquip = pageNo;
            this.getModelList('change');
        },

        // 页码大小改变时触发事件
        handleSizeChange (pageSize) {
            this.pageSize = pageSize;
            this.pageNo = 1;
            if (this.selectItem.equipNo) {
                this.changeRadio(this.selectItem);
            }
        },

        // 当前页码改变时触发
        handleCurrentChange (pageNo) {
            this.pageNo = pageNo;
            if (this.selectItem.equipNo) {
                this.changeRadio(this.selectItem);
            }
        },

        // 将多个数值拼接转化成10进制数
        toDecimalSystem (obj) {
            let str = '';
            for (let i in obj) {
                str += obj[i];
            }
            return parseInt(str, 2);
        },

        // 保存编辑设备
        saveEquip (equipData) {
            delete equipData.groupId;
            equipData.tabname = equipData.tabName;
            delete equipData.tabName;
            this.saveLoading = true;
            if (this.isEquipNew) {
                this.$api
                    .postAddModelEquip(JSON.stringify(equipData))
                    .then((res) => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.addSuccess'))
                            this.showEquipDialog = false;
                            // 修改后重新获取列表
                            this.getModelList();
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {

                        this.$message.error(err?.data, err);
                    }).finally(() => {
                        this.saveLoading = false;
                    });
            } else {
                this.$api
                    .postUpdateModelEquip(JSON.stringify(equipData))
                    .then(res => {
                        const { code, message } = res?.data || {}
                        if (code == 200) {
                            this.$message.success(this.$t('templateManage.publics.tips.editSuccess'));
                            equipData.tabName = equipData.tabname;
                            if (equipData.relatedVideo) {
                                equipData.relatedVideo = parseInt(equipData.relatedVideo.split('|')[1]);
                            } else {
                                equipData.relatedVideo = equipData.relatedVideo;
                            }
                            this.selectItem = JSON.parse(JSON.stringify(equipData));
                            this.showEquipDialog = false;
                            this.selectEquipNm = equipData.equipNm

                            // 修改后重新获取列表
                            this.getModelList();
                        } else {
                            this.$message.error(message);
                        }
                    })
                    .catch((err) => {

                        this.$message.error(err.data, err);
                    }).finally(() => {
                        this.saveLoading = false;
                    });
            }
        },

        // 获取产品属性
        getProductProperty () {
            if (this.selectEquipNo == '') {
                this.dataLoading = true;
                return;
            };
            this.$api.getProductProperty({ id: Number(this.selectEquipNo) })
                .then(res => {
                    const { code, data, message } = res?.data || {};
                    if (code === 200) {
                        if (data) {
                            this.hasproductAttr = true;
                            this.attrId = data?.id || '';
                            let propertyJosn = JSON.parse(data?.propertyJosn);
                            if (propertyJosn instanceof Array) {
                                for (let propItem of propertyJosn) {
                                    for (let attrItem of this.productAttrInfo.attr) {
                                        if (attrItem.key === propItem.key) {
                                            attrItem.value = propItem.value;
                                        }
                                    }
                                }
                            }
                        } else {
                            let productArr = this.productAttrInfo.attr;
                            productArr.forEach(item => {
                                item.value = '';
                            })
                            this.hasproductAttr = false;
                        }
                    } else {
                        this.$message.error(message);
                    }
                }).catch(err => {
                    this.$message.error(err?.data, err);
                }).finally(() => {
                    this.dataLoading = false;
                })
        },

        // 设置产品属性
        setProductProperty () {
            if (this.selectEquipNo == '') {
                this.$message.warning(this.$t('templateManage.tips.selectADevice'));
                return;
            };
            this.$refs['productAttrInfoForm'].validate((valid) => {
                if (valid) {
                    if (this.hasproductAttr) {
                        let data = {
                            id: this.attrId,
                            productId: this.selectEquipNo,
                            propertyList: this.productAttrInfo.attr
                        };
                        this.$api.updatedProductProperty(data)
                            .then(res => {
                                const { code, message } = res?.data || {};
                                if (code === 200) {
                                    this.$message.success(this.$t('templateManage.publics.tips.editSuccess'));
                                    this.getProductProperty();
                                } else {
                                    this.$message.error(message);
                                }
                            }).catch(err => {
                                this.$message.error(err?.data, err);
                            })
                    } else {
                        let data = {
                            productId: this.selectEquipNo,
                            propertyList: this.productAttrInfo.attr
                        };
                        this.$api.setProductProperty(data)
                            .then(res => {
                                const { code, message } = res?.data || {};
                                if (code === 200) {
                                    this.$message.success(this.$t('templateManage.publics.tips.saveSuccess'));
                                } else {
                                    this.$message.error(message);
                                }
                            }).catch(err => {
                                this.$message.error(err?.data, err);
                            })
                    }
                }
            });
        },

        cancelProductProperty () {
            this.$refs['productAttrInfoForm'].resetFields();
            this.getProductProperty();
        },

        batchExport () {
            this.$refs.batchExport.openDialog()
            this.$refs.batchExport.checkedList = []
            this.$refs.batchExport.isIndeterminate = false
            this.$refs.batchExport.checkAll = false
            this.$refs.batchExport.exportPage = 1
            this.$refs.batchExport.exportTotal = 0
            this.$refs.batchExport.exportSearch = ''
            this.$refs.batchExport.getTemplateList()
        }
    }

}
